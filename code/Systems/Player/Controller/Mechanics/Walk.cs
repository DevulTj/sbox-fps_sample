using Sandbox;
using System.Diagnostics;

namespace Facepunch.Gunfight.Mechanics;

/// <summary>
/// The basic walking mechanic for the player.
/// </summary>
public partial class WalkMechanic : PlayerControllerMechanic
{
	public float StopSpeed => 150f;
	public float StepSize => 18.0f;
	public float GroundAngle => 46.0f;
	public float DefaultSpeed => 280f;
	public float WalkSpeed => 140f;
	public float GroundFriction => 4.0f;
	public float MaxNonJumpVelocity => 140.0f;
	public float SurfaceFriction { get; set; } = 1f;
	public float Acceleration => 6f;
	public float DuckAcceleration => 5f;

	protected override bool ShouldStart()
	{
		return true;
	}

	public override float? WishSpeed => 200f;

	protected override void Simulate()
	{
		if ( GroundEntity != null )
			WalkMove();

		CategorizePosition( Controller.GroundEntity != null );
	}

	/// <summary>
	/// Try to keep a walking player on the ground when running down slopes etc.
	/// </summary>
	private void StayOnGround()
	{
		var down = Player.GravityDirection;
		var up = -down;
		
		var start = Controller.Position + up * 2;
		var end = Controller.Position + down * StepSize;

		// See how far up we can go without getting stuck
		var trace = Controller.TraceBBox( Controller.Position, start );
		start = trace.EndPosition;

		// Now trace down from a known safe position
		trace = Controller.TraceBBox( start, end );

		if ( trace.Fraction <= 0 ) return;
		if ( trace.Fraction >= 1 ) return;
		if ( trace.StartedSolid ) return;
		if ( Vector3.GetAngle( up, trace.Normal ) > GroundAngle ) return;

		Controller.Position = trace.EndPosition;
	}

	private void WalkMove()
	{
		var ctrl = Controller;

		var wishVel = ctrl.GetWishVelocity( true );
		var wishspeed = wishVel.Length;
		var friction = GroundFriction * SurfaceFriction;
		
		ctrl.ApplyFriction( StopSpeed, friction );
		var localVelocity = DoGravitationalWalk(Player.MoveInput, wishspeed);
		Velocity += ctrl.LocalToWorldVelocity( localVelocity );
		ctrl.MoveDir = localVelocity.Normal;
		localVelocity = ctrl.Accelerate( ctrl.MoveDir, wishspeed, 0, Acceleration );
		Velocity += ctrl.LocalToWorldVelocity( localVelocity );

		// Add in any base velocity to the current velocity.
		ctrl.Velocity += ctrl.BaseVelocity;

		try
		{
			if ( ctrl.Velocity.Length < 1.0f )
			{
				ctrl.Velocity = Vector3.Zero;
				return;
			}

			var dest = (ctrl.Position + ctrl.Velocity * Time.Delta);
			var pm = ctrl.TraceBBox( ctrl.Position, dest );

			if ( pm.Fraction == 1 )
			{
				ctrl.Position = pm.EndPosition;
				StayOnGround();
				return;
			}

			ctrl.StepMove();
		}
		finally
		{
			ctrl.Velocity -= ctrl.BaseVelocity;
		}

		StayOnGround();
	}

	/// <summary>
	/// We're no longer on the ground, remove it
	/// </summary>
	public void ClearGroundEntity()
	{
		if ( GroundEntity == null ) return;

		LastGroundEntity = GroundEntity;
		GroundEntity = null;
		SurfaceFriction = 1.0f;
	}

	public void SetGroundEntity( Entity entity )
	{
		LastGroundEntity = GroundEntity;
		LastVelocity = Velocity;

		GroundEntity = entity;

		if ( GroundEntity != null )
		{
			var localVelocity = Controller.WorldToLocalVelocity(Velocity).WithZ( 0 );
			Velocity = Controller.LocalToWorldVelocity( localVelocity );
			Controller.BaseVelocity = GroundEntity.Velocity;
		}
	}

	public void CategorizePosition( bool bStayOnGround )
	{
		var down = Player.GravityDirection;
		var up = -down;
		
		SurfaceFriction = 1.0f;

		var point = Position + down * 2;
		var vBumpOrigin = Position;
		bool bMovingUpRapidly =  (Velocity * Player.GravityDirection).Length > MaxNonJumpVelocity;
		bool bMoveToEndPos = false;

		if ( GroundEntity != null )
		{
			bMoveToEndPos = true;
			point += down;
		}
		else if ( bStayOnGround )
		{
			bMoveToEndPos = true;
			point += down;
		}

		if ( bMovingUpRapidly )
		{
			ClearGroundEntity();
			return;
		}
		
		var pm = Controller.TraceBBox( vBumpOrigin, point, 4.0f );

		var angle = Vector3.GetAngle( up, pm.Normal );
		Controller.CurrentGroundAngle = angle;

		if ( pm.Entity == null || angle > GroundAngle )
		{
			ClearGroundEntity();
			bMoveToEndPos = false;

			if ( (Velocity * Player.GravityDirection).Length > 0 )
				SurfaceFriction = 0.25f;
		}
		else
		{
			UpdateGroundEntity( pm );
		}

		if ( bMoveToEndPos && !pm.StartedSolid && pm.Fraction > 0.0f && pm.Fraction < 1.0f )
		{
			Position = pm.EndPosition;
		}
	}

	private void UpdateGroundEntity( TraceResult tr )
	{
		Controller.GroundNormal = tr.Normal;

		SurfaceFriction = tr.Surface.Friction * 1.25f;
		if ( SurfaceFriction > 1 ) SurfaceFriction = 1;

		SetGroundEntity( tr.Entity );
	}
	
	private Vector3 DoGravitationalWalk( Vector2 moveInput, float acceleration )
	{
		var localVelocity = Vector3.Zero;
		var accelerationDir = (Vector3.Forward * moveInput.x) + (Vector3.Left * moveInput.y);
		if (!accelerationDir.IsNearZeroLength)
		{
			var WantedSpeed = Controller.GetWishSpeed();
			localVelocity += (accelerationDir * acceleration * Time.Delta).Clamp( -WantedSpeed,
				WantedSpeed );
		}

		if (accelerationDir.x.AlmostEqual(0))
		{
			localVelocity.x = localVelocity.x.Approach(0, acceleration * Time.Delta);
		}

		if (accelerationDir.y.AlmostEqual(0))
		{
			localVelocity.y = localVelocity.y.Approach(0, acceleration * Time.Delta);
		}
		return localVelocity;
	}
	
}
