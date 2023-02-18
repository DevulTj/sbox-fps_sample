namespace GameTemplate;

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
}
