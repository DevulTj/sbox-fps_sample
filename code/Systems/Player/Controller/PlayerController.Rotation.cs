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
		var upDirection = -gravityDirection;
		// Update up direction if we're not close enough
		if (GetCapsuleAxisZ().Dot(upDirection) < 0.999999f)
		{
			var worldMatrix = MakeFromZx(upDirection, GetCapsuleAxisX());
			Matrix4x4.Decompose(worldMatrix, out var scale, rotation: out var newRotation, out var translation);
			Rotation = newRotation;
		}

		// Update character yaw direction
		var inputRotation = Player.LookInput.ToRotation();
		var deltaInputRotation = Rotation.Difference(LastInputRotation,  inputRotation);
		Rotation *= Rotation.FromYaw(deltaInputRotation.Yaw());
		LastInputRotation = inputRotation;
	}
	
	private Vector3 GetCapsuleAxisX()
	{
		// Fast simplification of FQuat::RotateVector() with FVector(1,0,0).
		var capsuleRotation = Rotation;
		var quatVector = new Vector3(capsuleRotation.x, capsuleRotation.y, capsuleRotation.z);

		return new Vector3(MathF.Pow(capsuleRotation.w, 2f) - quatVector.LengthSquared,
			capsuleRotation.z * capsuleRotation.w * 2.0f,
			capsuleRotation.y * capsuleRotation.w * -2.0f) + quatVector * (capsuleRotation.x * 2.0f);
	}

	
	private Vector3 GetCapsuleAxisZ()
	{
		// Fast simplification of FQuat::RotateVector() with FVector(0,0,1).
		var capsuleRotation = Rotation;
		var quatVector = new Vector3(capsuleRotation.x, capsuleRotation.y, capsuleRotation.z);

		return new Vector3(capsuleRotation.y * capsuleRotation.w * 2.0f, capsuleRotation.x * capsuleRotation.w * -2.0f,
			MathF.Pow(capsuleRotation.w, 2) - quatVector.LengthSquared) + quatVector * (capsuleRotation.z * 2.0f);
	}
	
	private Matrix4x4 MakeFromZx(Vector3 zAxis, Vector3 xAxis)
	{
		var newZ = zAxis.Normal;
		var norm = xAxis.Normal;

		// if they're almost same, we need to find arbitrary vector
		if (MathF.Abs(newZ.Dot(norm)).AlmostEqual(1f))
		{
			// make sure we don't ever pick the same as NewX
			norm = (MathF.Abs(newZ.z) < (1f - 0.0001)) ? Vector3.Up : Vector3.Forward;
		}

		var newY = newZ.Cross(norm).Normal;
		var newX = newY.Cross(newZ);

		return new Matrix4x4(
			newX.x, newX.y, newX.z, 0f,
			newY.x, newY.y, newY.z, 0f,
			newZ.x, newZ.y, newZ.z, 0f,
			0f, 0f, 0f, 1f
		);
	}

}
