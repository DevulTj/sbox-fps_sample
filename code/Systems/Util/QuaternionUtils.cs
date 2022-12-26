using System;

namespace Sandbox.Systems.Util;

public static class QuaternionUtils
{
	
	public static Rotation FromToRotation( Vector3 fromDirection, Vector3 toDirection )
	{
		var axis = Vector3.Cross( fromDirection, toDirection );
		var angle = Vector3.GetAngle( fromDirection, toDirection );
		return Rotation.FromAxis( axis.Normal, MathF.Sin(angle * 0.5f)  );
	}
	
}
