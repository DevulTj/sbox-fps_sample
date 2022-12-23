using System;
using System.Numerics;
using Sandbox;

namespace Facepunch.Gunfight;

public partial class PlayerController
{
	
	private Transform Transform { get; set; }
	
	public Vector3 LocalVelocity { get; set; }
	
	public void FromLocalVelocity()
	{
		Velocity = FromLocalVelocity(LocalVelocity);
	}
	
	public Vector3 FromLocalVelocity(Vector3 localVelocity)
	{
		return Transform.NormalToWorld(localVelocity);
	}

	public void ToLocalVelocity()
	{
		LocalVelocity = Transform.NormalToLocal(Velocity);
	}

	private void ProcessVelocity()
	{
		Transform = new Transform( Position, Rotation );
	}
	
}
