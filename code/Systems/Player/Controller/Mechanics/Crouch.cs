namespace GameTemplate.Mechanics;

/// <summary>
/// The basic crouch mechanic for players.
/// </summary>
public partial class CrouchMechanic : PlayerControllerMechanic
{
	public override int SortOrder => 9;
	public override float? WishSpeed => 120f;
	public override float? EyeHeight => 40f;

	protected override bool ShouldStart()
	{
		if ( !Input.Down( "duck" ) ) return false;
		if ( !Controller.GroundEntity.IsValid() ) return false;
		if ( Controller.IsMechanicActive<SprintMechanic>() ) return false;

		return true;
	}
}
