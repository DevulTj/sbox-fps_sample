using Sandbox;

namespace Facepunch.Gunfight.Mechanics;

/// <summary>
/// The jump mechanic for players.
/// </summary>
public partial class JumpMechanic : PlayerControllerMechanic
{
	public override int SortOrder => 25;

	protected override bool ShouldStart()
	{
		if ( !Input.Pressed( InputButton.Jump ) ) return false;
		if ( !Controller.GroundEntity.IsValid() ) return false;
		return true;
	}

	protected override void OnStart()
	{
		float flGroundFactor = 1.0f;
		float flMul = CalculateJumpForce();

		Velocity -= Player.GravityDirection * flMul * flGroundFactor;
		Velocity += Player.Gravity * 0.5f * Time.Delta;

		Controller.GetMechanic<WalkMechanic>()
			.ClearGroundEntity();
	}

	/// <summary>
	/// The lower the gravity, the higher the player can jump.
	/// Unless in zero-gravity.
	/// </summary>
	/// <returns></returns>
	private float CalculateJumpForce()
	{
		if ( Player.Gravity.Length == 0 )
		{
			return 0;
		}
		var multiplier = 1f / (Player.Gravity.Length / 800f);
		return 250f * multiplier;
	}
	
}
