using System;
using System.Numerics;
using Sandbox;
using Sandbox.Systems.Util;

namespace Facepunch.Gunfight;

public partial class PlayerController
{
	
	public Rotation LastInputRotation = Rotation.Identity;
	
	private Rotation Rotation
	{
		get => Player.Rotation;
		set => Player.Rotation = value;
	}
	
	private void SimulateRotation()
	{
		var gravityDirection = Player.GravityDirection;
		// Base rotation
		Rotation = QuaternionUtils.FromToRotation( Rotation.Up, gravityDirection ) * Rotation;

		// Update character yaw direction
		var inputRotation = Player.LookInput.ToRotation();
		var deltaInputRotation = Rotation.Difference(LastInputRotation,  inputRotation);
		Rotation *= Rotation.FromYaw(deltaInputRotation.Yaw());
		LastInputRotation = inputRotation;
	}

}
