namespace GameTemplate.Mechanics;

/// <summary>
/// The basic sprinting mechanic for players.
/// It shouldn't, though.
/// </summary>
public partial class SprintMechanic : PlayerControllerMechanic
{
	/// <summary>
	/// Sprint has a higher priority than other mechanics.
	/// </summary>
	public override int SortOrder => 10;
	public override float? WishSpeed => 320f;

	protected override bool ShouldStart()
	{
		if ( !Input.Down( "run" ) ) return false;
		if ( Player.MoveInput.Length == 0 ) return false;

		return true;
	}
}
