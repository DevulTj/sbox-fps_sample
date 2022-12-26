using System;
using System.Numerics;
using Sandbox;

namespace Facepunch.Gunfight;

public partial class PlayerController
{
	
	private Transform Transform { get; set; }
	
	public Vector3 MoveDir { get; set; } = Vector3.Zero;

	public Vector3 WorldToLocalVelocity( Vector3 worldVelocity )
	{
		return Transform.NormalToLocal( worldVelocity );
	}

	public Vector3 LocalToWorldVelocity(Vector3 localVelocity)
	{
		return Transform.NormalToWorld(localVelocity);
	}

	private void ProcessVelocity()
	{
		Transform = new Transform( Position, Rotation );
	}
	
}
