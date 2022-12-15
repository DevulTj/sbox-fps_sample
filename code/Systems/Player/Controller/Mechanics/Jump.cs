using Sandbox;

namespace Facepunch.Gunfight.Mechanics;

/// <summary>
/// The jump mechanic for players.
/// </summary>
public partial class Jump : PlayerControllerMechanic
{
	public override int SortOrder => 25;

	private float Cooldown => 0.5f;
	private float Gravity => 700f;

	private bool Lock = false;
	
	public TimeUntil WindupComplete { get; protected set; }

	protected override bool ShouldStart()
	{
		if ( Lock ) return true;

		if ( !Input.Pressed( InputButton.Jump ) ) return false;
		if ( !Controller.GroundEntity.IsValid() ) return false;

		return true;
	}

	protected override void OnStart()
	{
		WindupComplete = 0.1f;
		Lock = true;
		Simulate();
	}

	protected override void OnStop()
	{
		TimeUntilCanStart = Cooldown;
	}

	protected override void Simulate()
	{
		if ( !Controller.GroundEntity.IsValid() )
		{
			Lock = false;
			return;
		}

		if ( WindupComplete && Controller.GroundEntity.IsValid() )
		{
			float flGroundFactor = 1.0f;
			float flMul = 250f;
			float startz = Velocity.z;

			Velocity = Velocity.WithZ( startz + flMul * flGroundFactor );
			Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;

			Controller.GetMechanic<Walk>()
				.ClearGroundEntity();

			Controller.Player.PlaySound( "sounds/player/foley/gear/player.jump.gear.sound" );

			Lock = false;
		}
	}
}
