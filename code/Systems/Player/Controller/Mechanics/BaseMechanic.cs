using Sandbox;
using System;

namespace Facepunch.Gunfight.Mechanics;

public partial class BaseMechanic : BaseNetworkable
{
	/// <summary>
	/// Is this mechanic active?
	/// </summary>
	public bool IsActive { get; protected set; }

	/// <summary>
	/// An identifier for the mechanic
	/// </summary>
	public virtual string Name => "Mechanic";

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
				TimeSinceActivated = 0;
				OnActivate();
			}

			Simulate();
		}
		// Deactivate
		if ( before && !IsActive )
		{
			TimeSinceDeactivated = 0;
			OnDeactivate();
		}

		return IsActive;
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
		return $"{Name}: IsActive({IsActive})";
	}
}
