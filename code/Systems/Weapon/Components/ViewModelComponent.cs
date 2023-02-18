namespace GameTemplate.Weapons;

[Prefab]
public partial class ViewModelComponent : WeaponComponent, ISingletonComponent
{
	// I know that there's a metric fuck ton of Net properties here..
	// ideally, when the prefab gets set up, we'd send the client a message with the prefab's name
	// so we can populate all the Prefab marked properties with their defaults.

	//// General
	[Net, Prefab, ResourceType( "vmdl" )] public string ViewModelPath { get; set; }

	[Net, Prefab] public float OverallWeight { get; set; }
	[Net, Prefab] public float WeightReturnForce { get; set; }
	[Net, Prefab] public float WeightDamping { get; set; }
	[Net, Prefab] public float AccelerationDamping { get; set; }
	[Net, Prefab] public float VelocityScale { get; set; }

	//// Walking & Bob
	[Net, Prefab] public Vector3 WalkCycleOffset { get; set; }
	[Net, Prefab] public Vector2 BobAmount { get; set; }

	//// Global
	[Net, Prefab] public Vector3 GlobalPositionOffset { get; set; }
	[Net, Prefab] public Angles GlobalAngleOffset { get; set; }

	//// Crouching
	[Net, Prefab] public Vector3 CrouchPositionOffset { get; set; }
	[Net, Prefab] public Angles CrouchAngleOffset { get; set; }
}
