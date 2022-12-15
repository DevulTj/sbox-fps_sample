using Facepunch.Gunfight.WeaponSystem;
using Sandbox;

namespace Facepunch.Gunfight.Mechanics;

/// <summary>
/// The basic crouch mechanic for players.
/// </summary>
public partial class Crouch : PlayerControllerMechanic
{
	public override int SortOrder => 9;
	public override float? WishSpeed => 120f;
	public override float? EyeHeight => 40f;

	protected override bool ShouldStart()
	{
		if ( !Input.Down( InputButton.Duck ) ) return false;
		if ( !Controller.GroundEntity.IsValid() ) return false;
		if ( Controller.IsMechanicActive<Sprint>() ) return false;

		return true;
	}
}
