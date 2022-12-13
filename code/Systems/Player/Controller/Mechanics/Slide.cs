using Sandbox;

namespace Facepunch.Gunfight.Mechanics;

public partial class Slide : BaseMechanic
{
	public override float? EyeHeight => 40f;
	public override int SortOrder => 20;
	public override float? FrictionOverride => 0.5f;
	public override float? WishSpeed => 50f;
	public override Vector3? MoveInputScale => new( 0.5f, 0.5f );

	/// <summary>
	/// How fast we must be going before activating Slide.
	/// </summary>
	private float MinimumSpeed => 150f;

	/// <summary>
	/// We'll try to push the player forward to reach this speed.
	/// Make sure it's pretty high, as friction will make this much harder to reach.
	/// </summary>
	private float SlideSpeed => 1000f;

	private Sound SlideSound;

	/// <summary>
	/// If lock is true, we'll be stuck sliding until ShouldSlide says otherwise.
	/// </summary>
	private bool Lock = false;

	protected bool ShouldSlide()
	{
		if ( !Controller.GroundEntity.IsValid() ) return false;
		if ( Controller.Velocity.Length < MinimumSpeed ) return false;
		if ( TimeSinceActivated < 1 ) return true;

		return true;
	}

	protected override bool ShouldActivate()
	{
		if ( Lock ) return true;
		if ( !Input.Pressed( InputButton.View ) ) return false;
		if ( !Controller.GroundEntity.IsValid() ) return false;
		if ( Controller.Velocity.Length <= MinimumSpeed ) return false;

		return true;
	}

	protected override void OnActivate()
	{
		base.OnActivate();

		// Give a speed boost
		var forward = new Vector3( Controller.Velocity.x, Controller.Velocity.y, 0 ).Normal;
		if ( Controller.IsMechanicActive<Sprint>() || Controller.Velocity.Length < 300.0f )
			Controller.Velocity += forward * 120.0f;

		SlideSound = Controller.Player.PlaySound( "sounds/player/foley/slide/ski.loop.sound" );
		Lock = true;
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		Controller.Player.PlaySound( "sounds/player/foley/slide/ski.stop.sound" );
		SlideSound.Stop();
	}

	protected override void Simulate()
	{
		var hitNormal = Controller.GroundNormal;
		var slopeForward = new Vector3( hitNormal.x, hitNormal.y, 0 );

		if ( Controller.Velocity.Length < SlideSpeed )
			Controller.Velocity += slopeForward * SlideSpeed * Time.Delta;

		SlideSound.SetPitch( 0.5f + ( Controller.Velocity.Length / SlideSpeed ) );

		if ( !ShouldSlide() )
			Lock = false;
	}
}
