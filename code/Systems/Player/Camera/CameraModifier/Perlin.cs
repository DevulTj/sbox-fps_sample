using Sandbox;
using Sandbox.Utility;

namespace Facepunch.Gunfight.CameraModifiers;

public class Perlin : CameraModifier
{
	float Length;
	float Speed;
	float Size;
	float RotationAmount;
	float NoiseZ;

	TimeSince lifeTime = 0;
	float pos = 0;

	public Perlin( float length = 1.0f, float speed = 1.0f, float size = 1.0f, float rotation = 0.6f )
	{
		Length = length;
		Speed = speed;
		Size = size;
		RotationAmount = rotation;
		NoiseZ = Game.Random.Float( -10000, 10000 );

		pos = Game.Random.Float( 0, 100000 );
	}

	public override bool Update( Player player )
	{
		var delta = ((float)lifeTime).LerpInverse( 0, Length, true );
		delta = Easing.EaseOut( delta );
		var invdelta = 1 - delta;

		pos += Time.Delta * 10 * invdelta * Speed;

		float x = Noise.Perlin( pos, 0, NoiseZ );
		float y = Noise.Perlin( pos, 3.0f, NoiseZ );

		Camera.Position += (Camera.Rotation.Right * x + Camera.Rotation.Up * y) * invdelta * Size;
		Camera.Rotation *= Rotation.FromAxis( Sandbox.Camera.Rotation.Up, x * Size * invdelta * RotationAmount );
		Camera.Rotation *= Rotation.FromAxis( Sandbox.Camera.Rotation.Right, y * Size * invdelta * RotationAmount );

		return lifeTime < Length;
	}
}
