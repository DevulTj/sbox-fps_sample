using Sandbox;

namespace Facepunch.Gunfight.Mechanics;

/// <summary>
/// The jump mechanic for players.
/// </summary>
public partial class JumpMechanic : PlayerControllerMechanic
{
	public override int SortOrder => 25;

	private float Cooldown => 0.5f;

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
			float flMul = CalculateJumpForce();

			Velocity -= Player.GravityDirection * flMul * flGroundFactor;
			Velocity += Player.Gravity * 0.5f * Time.Delta;

			Controller.GetMechanic<WalkMechanic>()
				.ClearGroundEntity();

			Controller.Player.PlaySound( "sounds/player/foley/gear/player.jump.gear.sound" );

			Lock = false;
		}
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
