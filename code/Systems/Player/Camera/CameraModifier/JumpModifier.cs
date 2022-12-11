using Facepunch.Gunfight.Mechanics;
using Sandbox;
using Sandbox.UI;
using Sandbox.Utility;

namespace Facepunch.Gunfight.CameraModifiers;

public class JumpModifier : CameraModifier
{
	float Pitch = 20f;
	float Time = 0.2f;

	TimeSince lifeTime = 0;

	public JumpModifier( float pitch = 5f, float time = 0.2f )
	{
		Time = time;
		Pitch = pitch;
	}

	public override bool Update()
	{
		var pl = Game.LocalPawn as Player;
		var ctrl = pl.Controller;
		if ( ctrl == null ) return false;

		var jumpMechanic = ctrl.GetMechanic<Jump>();

		Camera.Rotation *= Rotation.From( jumpMechanic.WindupComplete.Fraction * Pitch, 0, 0 );
		var delta = ((float)jumpMechanic.TimeSinceDeactivated).LerpInverse( 0, Time / 2, true );

		if ( !ctrl.GroundEntity.IsValid() )
		{
			Camera.Rotation *= Rotation.From( delta * -Pitch, 0, 0 );
		}

		return lifeTime < Time;
	}
}
