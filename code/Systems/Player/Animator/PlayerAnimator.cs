using Sandbox;
using System;

namespace Facepunch.Gunfight;

public partial class PlayerAnimator : EntityComponent<Player>, ISingletonComponent
{
	public virtual void Simulate( IClient cl )
	{
		var player = Entity;
		var controller = player.Controller;
		CitizenAnimationHelper animHelper = new CitizenAnimationHelper( player );

		animHelper.WithWishVelocity( controller.GetWishVelocity() );
		animHelper.WithVelocity( controller.Velocity );
		animHelper.WithLookAt( player.EyePosition + player.EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		animHelper.AimAngle = player.EyeRotation;
		animHelper.FootShuffle = 0f;
		animHelper.DuckLevel = MathX.Lerp( animHelper.DuckLevel, 1 - controller.CurrentEyeHeight.Remap( 30, 72, 0, 1 ).Clamp( 0, 1 ), Time.Delta * 10.0f );
		animHelper.VoiceLevel = (Game.IsClient && cl.IsValid()) ? cl.Voice.LastHeard < 0.5f ? cl.Voice.CurrentLevel : 0.0f : 0.0f;
		animHelper.IsGrounded = controller.GroundEntity != null;
		animHelper.IsSwimming = player.GetWaterLevel() >= 0.5f;
		animHelper.IsWeaponLowered = false;

		var weapon = player.ActiveWeapon;
		if ( weapon.IsValid() )
		{
			player.SetAnimParameter( "holdtype", (int)weapon.WeaponData.HoldType );
			player.SetAnimParameter( "holdtype_handedness", (int)weapon.WeaponData.Handedness );
		}
	}

	public virtual void FrameSimulate( IClient cl )
	{
		//
	}
}
