namespace GameTemplate.Mechanics;

/// <summary>
/// The jump mechanic for players.
/// </summary>
public partial class JumpMechanic : PlayerControllerMechanic
{
	public override int SortOrder => 25;
	private float Gravity => 700f;

	protected override bool ShouldStart()
	{
		if ( !Input.Pressed( "jump" ) ) return false;
		if ( !Controller.GroundEntity.IsValid() ) return false;
		return true;
	}

	protected override void OnStart()
	{
		float flGroundFactor = 1.0f;
		float flMul = 250f;
		float startz = Velocity.z;

		Velocity = Velocity.WithZ( startz + flMul * flGroundFactor );
		Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;

		Controller.GetMechanic<WalkMechanic>()
			.ClearGroundEntity();
	}
}
