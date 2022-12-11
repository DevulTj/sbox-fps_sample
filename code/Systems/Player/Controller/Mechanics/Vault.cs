using Sandbox;

namespace Facepunch.Gunfight.Mechanics;

public partial class Vault : BaseMechanic
{
	public override string Name => "Vault";
	public override int SortOrder => 99;

	public float MinVaultHeight => 30f;
	public float MaxVaultHeight => 80f;

	bool Lock = false;
	private TimeSince timeSinceVault;
	private Vector3 vaultEnd;

	public bool CanActivate( bool assignValues = false )
	{
		var wall = GetWallInfo( Controller.Player.Rotation.Forward );

		if ( !wall.Hit ) return false;
		if ( wall.Height == 0 ) return false;
		if ( wall.Distance > Controller.BodyGirth * 1 ) return false;
		if ( Vector3.Dot( Controller.GetWishVelocity().Normal, wall.Normal ) > 0.0f ) return false;

		var posFwd = Controller.Position - wall.Normal * (Controller.BodyGirth + wall.Distance);
		var floorTraceStart = posFwd.WithZ( wall.AbsoluteHeight );
		var floorTraceEnd = posFwd.WithZ( Controller.Position.z );
		var floorTrace = Controller.TraceBBox( floorTraceStart, floorTraceEnd );

		if ( !floorTrace.Hit ) return false;
		if ( floorTrace.StartedSolid ) return false;

		var vaultHeight = floorTrace.EndPosition.z - Controller.Position.z;
		if ( vaultHeight < MinVaultHeight ) return false;
		if ( vaultHeight > MaxVaultHeight ) return false;

		if ( assignValues )
		{
			timeSinceVault = 0;
			vaultEnd = floorTrace.EndPosition.WithZ( floorTrace.EndPosition.z + 6.8f );
			Controller.Velocity = Controller.Velocity.WithZ( 0 );
		}

		return true;
	}

	protected override bool ShouldActivate()
	{
		if ( Lock ) return true;
		if ( !Input.Pressed( InputButton.Jump ) && Controller.GroundEntity.IsValid() ) return false;

		return CanActivate( true );
	}

	protected override void OnActivate()
	{
		Lock = true;
		Controller.Player.PlaySound( "sounds/footsteps/footstep-concrete-jump.sound" );
	}

	protected bool CloseEnough()
	{
		if ( Controller.Position.Distance( vaultEnd ) < 10f )
			return true;
		return false;
	}

	protected bool ReachedZ()
	{
		return vaultEnd.z.AlmostEqual( Controller.Position.z, 10f );
	}

	protected bool IsStuck( Vector3 testpos )
	{
		var result = Controller.TraceBBox( testpos, testpos );
		return result.StartedSolid;
	}

	protected void Stop()
	{
		Lock = false;
	}

	public Vector3 GetNextStepPos()
	{
		Controller.Velocity = Controller.Velocity.WithZ( 0 ); // Remove gravity.

		if ( !ReachedZ() )
			return Controller.Position.LerpTo( Controller.Position.WithZ( vaultEnd.z ), Time.Delta * 7f );

		return Controller.Position.LerpTo( vaultEnd, Time.Delta * 10f );
	}

	protected override void Simulate()
	{
		if ( timeSinceVault > 1f )
			Stop();

		if ( !CloseEnough() )
		{
			var nextPos = GetNextStepPos();
			Controller.Position = nextPos;
			Controller.Velocity = Vector3.Zero;

			return;
		}

		Stop();
	}
}
