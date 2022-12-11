using Facepunch.Gunfight.Mechanics;
using Sandbox;
using System.Collections.Generic;

namespace Facepunch.Gunfight.WeaponSystem;

public partial class PrimaryFire : WeaponComponent, ISingletonComponent
{
	public ComponentData Data => Weapon.WeaponData.PrimaryFire;

	protected override bool UseLagCompensation => true;

	private Ammo AmmoComponent => Weapon.GetComponent<Ammo>();

	protected override bool CanActivate( Player player )
	{
		if ( !Input.Down( InputButton.PrimaryAttack ) ) return false;
		if ( player.Controller.IsMechanicActive<Sprint>() ) return false;
		if ( Weapon.Tags.Has( "reloading" ) ) return false;
		// Optional
		if ( AmmoComponent != null && !AmmoComponent.HasEnoughAmmo() ) return false;

		return TimeSinceActivated > Data.FireDelay;
	}

	protected override void OnActivated( Player player )
	{
		base.OnActivated( player );

		player?.SetAnimParameter( "b_attack", true );

		// Send clientside effects to the player.
		using ( Prediction.Off() )
		{
			player.PlaySound( Data.FireSound );
			DoShootEffects( To.Single( player ) );
		}

		ShootBullet( Data.BulletSpread, Data.BulletForce, Data.BulletSize, Data.BulletCount, Data.BulletRange );

		AmmoComponent?.TakeAmmo();

		Weapon.GetComponent<Recoil>()?.AddRecoil();
	}

	[ClientRpc]
	public static void DoShootEffects()
	{
		Game.AssertClient();
		WeaponViewModel.Current?.SetAnimParameter( "fire", true );
	}

	protected TraceResult DoTraceBullet( Vector3 start, Vector3 end, float radius )
	{
		return Trace.Ray( start, end )
			.UseHitboxes()
			.WithAnyTags( "solid", "player", "glass" )
			.Ignore( Entity )
			.Size( radius )
			.Run();
	}

	/// <summary>
	/// When penetrating a surface, this is the trace increment amount.
	/// </summary>
	protected float PenetrationIncrementAmount => 15f;

	/// <summary>
	/// How many increments?
	/// </summary>
	protected int PenetrationMaxSteps => 2;

	/// <summary>
	/// How many ricochet hits until we stop traversing
	/// </summary>
	protected float MaxAmtOfHits => 2f;

	/// <summary>
	/// Maximum angle in degrees for ricochet to be possible
	/// </summary>
	protected float MaxRicochetAngle => 45f;

	protected bool ShouldPenetrate()
	{
		return true;
	}

	protected bool ShouldBulletContinue( TraceResult tr, float angle, ref float damage )
	{
		float maxAngle = MaxRicochetAngle;

		if ( angle > maxAngle )
			return false;

		return true;
	}

	protected Vector3 CalculateRicochetDirection( TraceResult tr, ref float hits )
	{
		if ( tr.Entity is GlassShard )
		{
			// Allow us to do another hit
			hits--;
			return tr.Direction;
		}

		return Vector3.Reflect( tr.Direction, tr.Normal ).Normal;
	}

	public IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius, ref float damage )
	{
		float curHits = 0;
		var hits = new List<TraceResult>();

		while ( curHits < MaxAmtOfHits )
		{
			curHits++;

			var tr = DoTraceBullet( start, end, radius );
			if ( tr.Hit )
			{
				if ( curHits > 1 )
					damage *= 0.5f;
				hits.Add( tr );
			}

			var reflectDir = CalculateRicochetDirection( tr, ref curHits );
			var angle = reflectDir.Angle( tr.Direction );
			var dist = tr.Distance.Remap( 0, Data.BulletRange, 1, 0.5f ).Clamp( 0.5f, 1f );
			damage *= dist;

			start = tr.EndPosition;
			end = tr.EndPosition + (reflectDir * Data.BulletRange);

			var didPenetrate = false;
			if ( ShouldPenetrate() )
			{
				// Look for penetration
				var forwardStep = 0f;

				while ( forwardStep < PenetrationMaxSteps )
				{
					forwardStep++;

					var penStart = tr.EndPosition + tr.Direction * (forwardStep * PenetrationIncrementAmount);
					var penEnd = tr.EndPosition + tr.Direction * (forwardStep + 1 * PenetrationIncrementAmount);

					var penTrace = DoTraceBullet( penStart, penEnd, radius );
					if ( !penTrace.StartedSolid )
					{
						var newStart = penTrace.EndPosition;
						var newTrace = DoTraceBullet( newStart, newStart + tr.Direction * Data.BulletRange, radius );
						hits.Add( newTrace );
						didPenetrate = true;
						break;
					}
				}
			}

			if ( didPenetrate || !ShouldBulletContinue( tr, angle, ref damage ) )
				break;
		}

		return hits;
	}

	public void ShootBullet( float spread, float force, float bulletSize, int bulletCount = 1, float bulletRange = 5000f )
	{
		//
		// Seed rand using the tick, so bullet cones match on client and server
		//
		Game.SetRandomSeed( Time.Tick );

		var recoil = Weapon.GetComponent<Recoil>();

		for ( int i = 0; i < bulletCount; i++ )
		{
			var rot = Rotation.LookAt( Player.AimRay.Forward );
			rot *= Rotation.From( new Angles( -recoil.CurrentRecoil.y, recoil.CurrentRecoil.x, 0 ) );

			var forward = rot.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			var damage = Data.BaseDamage;

			Vector3 LastImpact = Vector3.Zero;
			int count = 0;
			foreach ( var tr in TraceBullet( Player.AimRay.Position, Player.AimRay.Position + forward * bulletRange, bulletSize, ref damage ) )
			{
				tr.Surface.DoBulletImpact( tr );

				if ( !Game.IsServer ) continue;
				if ( !tr.Entity.IsValid() ) continue;

				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Player )
					.WithWeapon( Weapon );

				tr.Entity.TakeDamage( damageInfo );

				DoTracer( To.Everyone, Weapon, tr.StartPosition, tr.EndPosition, tr.Distance, count );

				if ( count == 1 )
				{
					Particles.Create( "particles/gameplay/guns/trail/rico_trail_impact_spark.vpcf", LastImpact );
				}

				LastImpact = tr.EndPosition;
				count++;
			}
		}
	}

	[ClientRpc]
	public static void DoTracer( Weapon weapon, Vector3 from, Vector3 to, float dist, int bullet )
	{
		var path = "particles/gameplay/guns/trail/trail_smoke.vpcf";

		if ( bullet > 0 )
		{
			path = "particles/gameplay/guns/trail/rico_trail_smoke.vpcf";

			// Project backward
			Vector3 dir = (from - to).Normal;
			var tr = Trace.Ray( to, from + (dir * 50f) )
				.Radius( 1f )
				.Ignore( weapon )
				.Run();

			tr.Surface.DoBulletImpact( tr );
		}

		var system = Particles.Create( path );

		system?.SetPosition( 0, bullet == 0 ? weapon.EffectEntity.GetAttachment( "muzzle" )?.Position ?? from : from );
		system?.SetPosition( 1, to );
		system?.SetPosition( 2, dist );
	}

	/// <summary>
	/// Data asset information.
	/// </summary>
	public struct ComponentData
	{
		public float BaseDamage { get; set; }
		public float BulletRange { get; set; }
		public int BulletCount { get; set; }
		public float BulletForce { get; set; }
		public float BulletSize { get; set; }
		public float BulletSpread { get; set; }
		public float FireDelay { get; set; }

		[ResourceType( "sound" )]
		public string FireSound { get; set; }

		[ResourceType( "sound" )]
		public string DryFireSound { get; set; }
	}
}
