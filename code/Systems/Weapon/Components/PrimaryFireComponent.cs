using GameTemplate.Mechanics;

namespace GameTemplate.Weapons;

public partial class PrimaryFire : WeaponComponent, ISingletonComponent
{
	public ComponentData Data => Weapon.WeaponData.PrimaryFire;
	public TimeUntil TimeUntilCanFire { get; set; }

	protected override bool CanStart( Player player )
	{
		if ( TimeUntilCanFire > 0 ) return false;
		if ( !Input.Down( InputButton.PrimaryAttack ) ) return false;
		if ( Weapon.Tags.Has( "reloading" ) ) return false;
		if ( GetComponent<Ammo>() is Ammo ammo && !ammo.HasEnoughAmmo() ) return false; 

		return TimeSinceActivated > Data.FireDelay;
	}

	public override void OnGameEvent( string eventName )
	{
		if ( eventName == "sprint.stop" )
		{
			TimeUntilCanFire = 0.2f;
		}
		if ( eventName == "aim.start" )
		{
			TimeUntilCanFire = 0.15f;
		}
	}

	protected override void OnStart( Player player )
	{
		base.OnStart( player );

		player?.SetAnimParameter( "b_attack", true );

		// Send clientside effects to the player.
		if ( Game.IsServer )
		{
			player.PlaySound( Data.FireSound );
			DoShootEffects( To.Single( player ) );
		}

		ShootBullet( Data.BulletSpread, Data.BulletForce, Data.BulletSize, Data.BulletCount, Data.BulletRange );
	}

	[ClientRpc]
	public static void DoShootEffects()
	{
		Game.AssertClient();
		WeaponViewModel.Current?.SetAnimParameter( "fire", true );
	}

	public IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius )
	{
		var tr = Trace.Ray( start, end )
			.UseHitboxes()
			.WithAnyTags( "solid", "player", "glass" )
			.Ignore( Entity )
			.Size( radius )
			.Run();

		if ( tr.Hit )
		{
			yield return tr;
		}
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

			// Do we have recoil on this weapon?
			if ( recoil != null )
			{
				rot *= Rotation.From( new Angles( -recoil.CurrentRecoil.y, recoil.CurrentRecoil.x, 0 ) );
			}

			var forward = rot.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			var damage = Data.BaseDamage;

			foreach ( var tr in TraceBullet( Player.AimRay.Position, Player.AimRay.Position + forward * bulletRange, bulletSize ) )
			{
				tr.Surface.DoBulletImpact( tr );

				if ( !Game.IsServer ) continue;
				if ( !tr.Entity.IsValid() ) continue;

				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Player )
					.WithWeapon( Weapon );

				tr.Entity.TakeDamage( damageInfo );
			}
		}
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
