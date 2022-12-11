using Facepunch.Gunfight.WeaponSystem;
using Sandbox;

namespace Facepunch.Gunfight;

public partial class Player : AnimatedEntity
{
	/// <summary>
	/// The controller is responsible for player movement and setting up EyePosition / EyeRotation.
	/// </summary>
	[Net, Predicted] public PlayerController Controller { get; set; }

	/// <summary>
	/// The animator is responsible for animating the player's current model.
	/// </summary>
	[Net, Predicted] public PlayerAnimator Animator { get; set; }

	/// <summary>
	/// The inventory is responsible for storing weapons for a player to use.
	/// </summary>
	[Net, Predicted] public Inventory Inventory { get; set; }

	/// <summary>
	/// Accessor for getting a player's active weapon.
	/// </summary>
	public Weapon ActiveWeapon => Inventory.ActiveWeapon;

	/// <summary>
	/// A camera is known only to the local client. This cannot be used on the server.
	/// </summary>
	public PlayerCamera PlayerCamera { get; set; }

	/// <summary>
	/// A cached model used for all players.
	/// </summary>
	public static Model PlayerModel = Model.Load( "models/citizen/citizen.vmdl" );

	/// <summary>
	/// When the player is first created. This isn't called when a player respawns.
	/// </summary>
	public override void Spawn()
	{
		Model = PlayerModel;

		// Default properties
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Tags.Add( "player" );
	}

	/// <summary>
	/// Called when a player respawns, think of this as a soft spawn - we're only reinitializing transient data here.
	/// </summary>
	public void Respawn()
	{
		Controller = new PlayerController();
		Animator = new CitizenAnimator();
		Inventory = new Inventory( this );

		Inventory.AddWeapon( WeaponData.CreateInstance( "AKM" ) );

		ClientRespawn( To.Single( Client ) );
	}

	/// <summary>
	/// Called clientside when the player respawns. Useful for adding components like the camera.
	/// </summary>
	[ClientRpc]
	public void ClientRespawn()
	{
		PlayerCamera = new PlayerCamera();
	}

	/// <summary>
	/// Called every server and client tick.
	/// </summary>
	/// <param name="cl"></param>
	public override void Simulate( IClient cl )
	{
		Rotation = LookInput.WithPitch( 0f ).ToRotation();

		Controller?.Simulate( this, cl );
		Animator?.Simulate( this, cl );

		// Simulate our active weapon if we can.
		Inventory?.ActiveWeapon?.Simulate( cl );
	}

	/// <summary>
	/// Entrypoint to update the player's camera.
	/// </summary>
	[Event.Client.PostCamera]
	protected void PostCameraUpdate()
	{
		PlayerCamera?.Update( this );

		// Apply camera modifiers after a camera update.
		CameraModifier.Apply();
	}

	/// <summary>
	/// Called every frame clientside.
	/// </summary>
	/// <param name="cl"></param>
	public override void FrameSimulate( IClient cl )
	{
		Rotation = LookInput.WithPitch( 0f ).ToRotation();

		Controller?.FrameSimulate( this, cl );
		Animator?.FrameSimulate( this, cl );

		// Simulate our active weapon if we can.
		Inventory?.ActiveWeapon?.FrameSimulate( cl );
	}
}
