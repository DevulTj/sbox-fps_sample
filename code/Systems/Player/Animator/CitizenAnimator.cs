using Facepunch.Gunfight.Mechanics;
using Sandbox;

namespace Facepunch.Gunfight;

/// <summary>
/// An extension of the player animator, fit for Facepunch's Citizen Anim Graph
/// </summary>
public partial class CitizenAnimator : PlayerAnimator
{
	public override void Simulate( Player player, IClient cl )
	{
		var controller = player.Controller;
		CitizenAnimationHelper animHelper = new CitizenAnimationHelper( player );

		animHelper.WithWishVelocity( controller.GetWishVelocity() );
		animHelper.WithVelocity( controller.Velocity );
		animHelper.WithLookAt( player.EyePosition + player.EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		animHelper.AimAngle = player.EyeRotation;
		animHelper.FootShuffle = 0f;
		animHelper.DuckLevel = MathX.Lerp( animHelper.DuckLevel, controller.GetMechanic<Crouch>().IsActive ? 1 : 0, Time.Delta * 10.0f );
		animHelper.VoiceLevel = (Game.IsClient && cl.IsValid()) ? cl.Voice.LastHeard < 0.5f ? cl.Voice.CurrentLevel: 0.0f : 0.0f;
		animHelper.IsGrounded = controller.GroundEntity != null;
		//animHelper.IsSitting = controller.HasTag( "sitting" );
		//animHelper.IsNoclipping = controller.HasTag( "noclip" );
		//animHelper.IsClimbing = controller.HasTag( "climbing" );
		animHelper.IsSwimming = player.GetWaterLevel() >= 0.5f;
		animHelper.IsWeaponLowered = false;

		var weapon = player.ActiveWeapon;
		if ( weapon.IsValid() )
		{
			player.SetAnimParameter( "holdtype", (int)weapon.WeaponData.HoldType );
			player.SetAnimParameter( "holdtype_handedness", (int)weapon.WeaponData.Handedness );
		}
	}
}
