using Sandbox;
using System;

namespace Facepunch.Gunfight.Mechanics;

/// <summary>
/// The heavy landing mechanic for players.
/// </summary>
public partial class HeavyLand : BaseMechanic
{
	public override int SortOrder => 30;
	public override string Name => "Heavy Land";
	public override float? FrictionOverride => 2.5f;
	public override float? WishSpeed => 150f;

	private bool Lock = false;
	private TimeUntil TimeUntilFinished = 0f;

	protected override bool ShouldActivate()
	{
		if ( Lock ) return true;
		if ( MathF.Abs( LastVelocity.z ) < 200f ) return false;		

		// We just landed, makes sense, right?
		return !LastGroundEntity.IsValid() && GroundEntity.IsValid(); 
	}

	protected override void OnActivate()
	{
		TimeUntilFinished = 0.7f;
		Lock = true;

		// Play the heavy land sound, on top of the light one
		Player.PlaySound( "sounds/player/foley/gear/player.heavy_land.gear.sound" );

		var strength = MathF.Abs( Controller.LastVelocity.z ).LerpInverse( 0, 100f, true );
		_ = new CameraModifiers.Pitch( 1f, 2f * strength );
	}

	protected override void Simulate()
	{
		if ( TimeUntilFinished )
		{
			Lock = false;
		}
	}
}
