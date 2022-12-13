using Facepunch.Gunfight.WeaponSystem;
using Sandbox;
using System;
using System.Dynamic;

namespace Facepunch.Gunfight.Mechanics;

public partial class BaseMechanic : BaseNetworkable
{
	/// <summary>
	/// Is this mechanic active?
	/// </summary>
	public bool IsActive { get; protected set; }

	/// <summary>
	/// How long has it been since we activated this mechanic?
	/// </summary>
	public TimeSince TimeSinceActivated { get; protected set; }

	/// <summary>
	/// How long has it been since we deactivated this mechanic?
	/// </summary>
	public TimeSince TimeSinceDeactivated { get; protected set; }

	/// <summary>
	/// Standard cooldown for mechanics.
	/// </summary>
	public TimeUntil TimeUntilCanNextActivate { get; protected set; }

	protected PlayerController Controller { get; set; }

	/// <summary>
	/// Accessor for the player.
	/// </summary>
	protected Player Player => Controller.Player;

	/// <summary>
	/// Used to dictate the most important mechanic to take information such as EyeHeight, WishSpeed.
	/// </summary>
	public virtual int SortOrder { get; set; } = 0;

	/// <summary>
	/// Override the current eye height.
	/// </summary>
	public virtual float? EyeHeight { get; set; } = null;

	/// <summary>
	/// Override the current wish speed.
	/// </summary>
	public virtual float? WishSpeed { get; set; } = null;

	/// <summary>
	/// Identifier for the Mechanic
	/// </summary>
	public virtual string Name => info.Name.Replace( " ", "" );

	public Vector3 Position
	{
		get => Controller.Position;
		set => Controller.Position = value;
	}

	public Vector3 Velocity
	{
		get => Controller.Velocity;
		set => Controller.Velocity = value;
	}

	public Vector3 LastVelocity
	{
		get => Controller.LastVelocity;
		set => Controller.LastVelocity = value;
	}

	public Entity GroundEntity
	{
		get => Controller.GroundEntity;
		set => Controller.GroundEntity = value;
	}

	public Entity LastGroundEntity
	{
		get => Controller.LastGroundEntity;
		set => Controller.LastGroundEntity = value;
	}

	/// <summary>
	/// Mechanics can override friction - the Walk mechanic drives this.
	/// </summary>
	public virtual float? FrictionOverride { get; set; } = null;

	public virtual Vector3? MoveInputScale { get; set; } = null;

	DisplayInfo info;
	public BaseMechanic()
	{
		info = DisplayInfo.For( this );
	}

	/// <summary>
	/// Called every time the controller simulates, for each mechanic.
	/// </summary>
	/// <param name="controller"></param>
	/// <returns></returns>
	public bool TrySimulate( PlayerController controller )
	{
		Controller = controller;

		var before = IsActive;
		IsActive = ShouldActivate();

		if ( IsActive )
		{
			if ( before != IsActive )
			{
				Activate();
			}

			Simulate();
		}
		// Deactivate
		if ( before && !IsActive )
		{
			Deactivate();
		}

		return IsActive;
	}

	protected void Activate()
	{
		TimeSinceActivated = 0;
		RunGameEvent( $"{Name}.activate" );
		OnActivate();
	}

	protected void Deactivate()
	{
		TimeSinceDeactivated = 0;
		RunGameEvent( $"{Name}.deactivate" );
		OnDeactivate();
	}

	/// <summary>
	/// Called when the mechanic deactivates. For example, when you stop crouching.
	/// </summary>
	protected virtual void OnDeactivate()
	{
		//
	}

	/// <summary>
	/// Called when the mechanic activates. For example, when you start sliding.
	/// </summary>
	protected virtual void OnActivate()
	{
		//
	}

	/// <summary>
	/// Returns whether or not this ability should activate and simulate this tick.
	/// By default, it's set to TimeUntilCanNextActivate, which you can set in your own mechanics.
	/// </summary>
	/// <returns></returns>
	protected virtual bool ShouldActivate()
	{
		return TimeUntilCanNextActivate;
	}

	/// <summary>
	/// A regular old simulation tick.
	/// </summary>
	protected virtual void Simulate()
	{
		//
	}

	public override string ToString()
	{
		return $"{info.Name}: IsActive({IsActive})";
	}

	protected WallInfo GetWallInfo( Vector3 direction )
	{
		var trace = Controller.TraceBBox( Controller.Position, Controller.Position + direction * 32f );
		if ( !trace.Hit ) return default;

		Vector3 tracePos;
		var height = ApproximateWallHeight( Controller.Position, trace.Normal, 500f, 32f, 128, out tracePos, out float absoluteHeight );

		return new WallInfo()
		{
			Hit = true,
			Height = height,
			AbsoluteHeight = absoluteHeight,
			Distance = trace.Distance,
			Normal = trace.Normal,
			Trace = trace,
			TracePos = tracePos,
		};
	}

	private static int MaxWallTraceIterations => 40;
	private static float ApproximateWallHeight( Vector3 startPos, Vector3 wallNormal, float maxHeight, float maxDist, int precision, out Vector3 tracePos, out float absoluteHeight )
	{
		tracePos = Vector3.Zero;
		absoluteHeight = startPos.z;

		var step = maxHeight / precision;

		float currentHeight = 0f;
		var foundWall = false;
		for ( int i = 0; i < Math.Min( precision, MaxWallTraceIterations ); i++ )
		{
			startPos.z += step;
			currentHeight += step;
			var trace = Trace.Ray( startPos, startPos - wallNormal * maxDist )
				.WorldOnly()
				.Run();

			if ( PlayerController.Debug )
				DebugOverlay.TraceResult( trace );

			if ( !trace.Hit && !foundWall ) continue;
			if ( trace.Hit )
			{
				tracePos = trace.HitPosition;

				foundWall = true;
				continue;
			}

			absoluteHeight = startPos.z;
			return currentHeight;
		}
		return 0f;
	}

	public virtual void OnGameEvent( string eventName )
	{
		//
	}

	public void RunGameEvent( string eventName )
	{
		Player?.RunGameEvent( eventName );
	}

	public struct WallInfo
	{
		public bool Hit;
		public float Distance;
		public Vector3 Normal;
		public float Height;
		public float AbsoluteHeight;
		public TraceResult Trace;
		public Vector3 TracePos;
	}
}
