using Facepunch.Gunfight.Mechanics;
using Sandbox;
using System.Collections.Generic;
using System.Linq;
using Sandbox.Systems.Util;

namespace Facepunch.Gunfight;

public partial class PlayerController : EntityComponent<Player>, ISingletonComponent
{
	public Vector3 LastVelocity { get; set; }
	public Entity LastGroundEntity { get; set; }
	public Entity GroundEntity { get; set; }
	public Vector3 BaseVelocity { get; set; }
	public Vector3 GroundNormal { get; set; }
	public float CurrentGroundAngle { get; set; }

	public Player Player => Entity;

	/// <summary>
	/// A list of mechanics used by the player controller.
	/// </summary>
	public IEnumerable<PlayerControllerMechanic> Mechanics => Entity.Components.GetAll<PlayerControllerMechanic>();

	/// <summary>
	/// Position accessor for the Player.
	/// </summary>
	public Vector3 Position
	{
		get => Player.Position;
		set => Player.Position = value;
	}

	public Vector3 Velocity
	{
		get => Player.Velocity;
		set => Player.Velocity = value;
	}

	/// <summary>
	/// This'll set LocalEyePosition when we Simulate.
	/// </summary>
	public float EyeHeight => BestMechanic?.EyeHeight ?? 64f;

	[Net, Predicted] public float CurrentEyeHeight { get; set; } = 64f;

	public Vector3 MoveInputScale => BestMechanic?.MoveInputScale ?? Vector3.One;

	/// <summary>
	/// The "best" mechanic is the mechanic with the highest priority, defined by SortOrder.
	/// </summary>
	public PlayerControllerMechanic BestMechanic => Mechanics.OrderByDescending( x => x.SortOrder ).FirstOrDefault( x => x.IsActive );

	[ConVar.Replicated( "playercontroller_debug" )]
	public static bool Debug { get; set; } = false;

	public float BodyGirth => 32f;

	/// <summary>
	/// The player's hull, we'll use this to calculate stuff like collision.
	/// </summary>
	public BBox Hull
	{
		// FIXME how to calculate the hull to work with different angles?
		get
		{
			var girth = BodyGirth * 0.5f;
			var baseHeight = CurrentEyeHeight;

			var mins = new Vector3( -girth, -girth, 0 );
			var maxs = new Vector3( +girth, +girth, baseHeight * 1.1f );

			return new BBox( mins, maxs );
		}
	}

	public T GetMechanic<T>() where T : PlayerControllerMechanic
	{
		foreach ( var mechanic in Mechanics )
		{
			if ( mechanic is T val ) return val;
		}

		return null;
	}

	public bool IsMechanicActive<T>() where T : PlayerControllerMechanic
	{
		return GetMechanic<T>()?.IsActive ?? false;
	}

	protected void SimulateEyes()
	{
		Player.EyeRotation = Rotation * Rotation.FromPitch( Player.LookInput.ToRotation().Pitch() );
		Player.EyeLocalPosition = Vector3.Up * CurrentEyeHeight;
	}

	protected void SimulateMechanics()
	{
		foreach ( var mechanic in Mechanics )
		{
			mechanic.TrySimulate( this );
		}

		var target = EyeHeight;
		// Magic number :sad:
		var trace = TraceBBox( Position, Position, 0, 10f );
		if ( trace.Hit && target > CurrentEyeHeight )
		{
			// We hit something, that means we can't increase our eye height because something's in the way.
		}
		else
		{
			CurrentEyeHeight = CurrentEyeHeight.LerpTo( target, Time.Delta * 10f );
		}
	}

	public virtual void Simulate( IClient cl )
	{
		SimulateRotation();
		SimulateEyes();
		ProcessVelocity();
		SimulateMechanics();

		if ( Debug )
		{
			var hull = Hull;
			DebugOverlay.Box( Position, hull.Mins, hull.Maxs, Color.Red );
			DebugOverlay.Box( Position, hull.Mins, hull.Maxs, Color.Blue );

			var lineOffset = 50;

			DebugOverlay.ScreenText( $"Player Controller", ++lineOffset );
			DebugOverlay.ScreenText( $"       Position: {Position}", ++lineOffset );
			DebugOverlay.ScreenText( $"       Rotation: {Rotation}", ++lineOffset );
			DebugOverlay.ScreenText( $"        Velocity: {Velocity}", ++lineOffset );
			DebugOverlay.ScreenText( $"    BaseVelocity: {BaseVelocity}", ++lineOffset );
			DebugOverlay.ScreenText( $"    GroundEntity: {GroundEntity} [{GroundEntity?.Velocity}]", ++lineOffset );
			DebugOverlay.ScreenText( $"           Speed: {Velocity.Length}", ++lineOffset );

			++lineOffset;
			DebugOverlay.ScreenText( $"Mechanics", ++lineOffset );
			foreach ( var mechanic in Mechanics )
			{
				DebugOverlay.ScreenText( $"{mechanic}", ++lineOffset );
			}
		}
	}

	public virtual void FrameSimulate( IClient cl )
	{
		SimulateRotation();
		SimulateEyes();
	}

	/// <summary>
	/// Traces the bbox and returns the trace result.
	/// LiftFeet will move the start position up by this amount, while keeping the top of the bbox at the same 
	/// position. This is good when tracing down because you won't be tracing through the ceiling above.
	/// </summary>
	public virtual TraceResult TraceBBox( Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f, float liftHead = 0.0f )
	{
		var up = -Player.GravityDirection;
		if ( liftFeet > 0 )
		{
			start += up * liftFeet;
			// FIXME this breaks gravity checks at certain angles, how to fix?
			// maxs = maxs.WithZ( maxs.z - liftFeet );
		}

		if ( liftHead > 0 )
		{
			end += up * liftHead;
		}

		var tr = Trace.Ray( start, end )
					.Size( mins, maxs )
					.WithAnyTags( "solid", "playerclip", "passbullets", "player" )
					.WithoutTags( "prop" )
					.Ignore( Player )
					.Run();

		return tr;
	}

	/// <summary>
	/// This calls TraceBBox with the right sized bbox. You should derive this in your controller if you 
	/// want to use the built in functions
	/// </summary>
	public virtual TraceResult TraceBBox( Vector3 start, Vector3 end, float liftFeet = 0.0f, float liftHead = 0.0f )
	{
		var hull = Hull;
		return TraceBBox( start, end, hull.Mins, hull.Maxs, liftFeet, liftHead );
	}

	public Vector3 GetWishVelocity( bool zeroPitch = false )
	{
		var result = new Vector3( Player.MoveInput.x, Player.MoveInput.y, 0 );
		result *= MoveInputScale;

		var inSpeed = result.Length.Clamp( 0, 1 );
		result *= Player.LookInput.WithPitch( 0f ).ToRotation();

		if ( zeroPitch )
			result.z = 0;

		result = result.Normal * inSpeed;
		result *= GetWishSpeed();

		var ang = CurrentGroundAngle.Remap( 0, 45, 1, 0.6f );
		result *= ang;

		return result;
	}

	public virtual float GetWishSpeed()
	{
		return BestMechanic?.WishSpeed ?? 180f;
	}

	public Vector3 Accelerate( Vector3 wishdir, float wishspeed, float speedLimit, float acceleration )
	{
		if ( speedLimit > 0 && wishspeed > speedLimit )
			wishspeed = speedLimit;

		var currentspeed = Velocity.Dot( wishdir );
		var addspeed = wishspeed - currentspeed;

		if ( addspeed <= 0 )
			return Vector3.Zero;

		var accelspeed = acceleration * Time.Delta * wishspeed;

		if ( accelspeed > addspeed )
			accelspeed = addspeed;

		return wishdir * accelspeed;
	}

	public void ApplyFriction( float stopSpeed, float frictionAmount = 1.0f )
	{
		var speed = Velocity.Length;
		if ( speed.AlmostEqual( 0f ) ) return;

		if ( BestMechanic?.FrictionOverride != null )
			frictionAmount = BestMechanic.FrictionOverride.Value;

		var control = (speed < stopSpeed) ? stopSpeed : speed;
		var drop = control * Time.Delta * frictionAmount;

		// Scale the velocity
		float newspeed = speed - drop;
		if ( newspeed < 0 ) newspeed = 0;

		if ( newspeed != speed )
		{
			newspeed /= speed;
			Velocity *= newspeed;
		}
	}

	public void StepMove( float groundAngle = 46f, float stepSize = 18f )
	{
		var mover = new GravitationalMoveHelper( Position, Velocity, Player.GravityDirection );
		mover.Trace = mover.Trace.Size( Hull )
			.Ignore( Player )
			.WithoutTags( "player" );
		mover.MaxStandableAngle = groundAngle;

		mover.TryMoveWithStep( Time.Delta, stepSize );
		// FIXME this shouldn't be needed, player gets stuck if the gravity changes a lot (spherical world)
		mover.TryUnstuck();

		Position = mover.Position;
		Velocity = mover.Velocity;
	}

	public void Move( float groundAngle = 46f )
	{
		var mover = new GravitationalMoveHelper( Position, Velocity, Player.GravityDirection );
		mover.Trace = mover.Trace.Size( Hull )
			.Ignore( Player )
			.WithoutTags( "player" );
		mover.MaxStandableAngle = groundAngle;

		mover.TryMove( Time.Delta );
		// FIXME this shouldn't be needed, player gets stuck if the gravity changes a lot (spherical world)
		mover.TryUnstuck();

		Position = mover.Position;
		Velocity = mover.Velocity;
	}

	protected override void OnDeactivate()
	{
	}
}
