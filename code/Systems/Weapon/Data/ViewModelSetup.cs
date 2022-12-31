namespace Facepunch.Gunfight;

public struct ViewModelData
{
	//// General
	public float OverallWeight { get; set; }
	public float WeightReturnForce { get; set; }
	public float WeightDamping { get; set; }
	public float AccelerationDamping { get; set; }
	public float VelocityScale { get; set; }
	public float RotationalPivotForce { get; set; }
	public float RotationalScale { get; set; }

	//// Walking & Bob
	public Vector3 WalkCycleOffset { get; set; }
	public Vector2 BobAmount { get; set; }

	//// Global
	public float GlobalLerpPower { get; set; }
	public Vector3 GlobalPositionOffset { get; set; }
	public Angles GlobalAngleOffset { get; set; }

	//// Crouching
	public Vector3 CrouchPositionOffset { get; set; }
	public Angles CrouchAngleOffset { get; set; }

	//// Avoidance
	/// <summary>
	/// The max position offset when avoidance comes into play.
	/// Avoidance is when something is in the way of the weapon.
	/// </summary>
	public Vector3 AvoidancePositionOffset { get; set; }

	/// <summary>
	/// The max angle offset when avoidance comes into play.
	/// Avoidance is when something is in the way of the weapon.
	/// </summary>
	public Angles AvoidanceAngleOffset { get; set; }

	//// Sprinting
	public Vector3 SprintPositionOffset { get; set; }
	public Angles SprintAngleOffset { get; set; }

	/// Aim Down Sight
	public Vector3 AimPositionOffset { get; set; }
	public Angles AimAngleOffset { get; set; }
}
