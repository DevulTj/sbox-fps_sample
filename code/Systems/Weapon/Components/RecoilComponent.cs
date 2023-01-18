using Sandbox;
using System.Collections.Generic;

namespace Facepunch.Gunfight.WeaponSystem;

public partial class Recoil : WeaponComponent, ISingletonComponent
{
	[Net, Predicted] public Vector2 CurrentRecoil { get; set; }
	[Net, Predicted] public TimeUntil TimeUntilRemove { get; set; }

	public ComponentData Data => Weapon.WeaponData.Recoil;
	protected override bool EnableActivateEvents => false;

	public override void OnGameEvent( string eventName )
	{
		if ( eventName == "primaryfire.start" )
		{
			AddRecoil();
		}
	}

	public void AddRecoil()
	{
		var entry = Game.Random.FromList( Data.Presets );
		CurrentRecoil += new Vector2( entry.x, entry.y ) * Time.Delta;
		TimeUntilRemove = Data.RecoveryTime;
	}

	public override void Simulate( IClient cl, Player player )
	{
		base.Simulate( cl, player );

		var pitchOffset = Input.AnalogLook.pitch;

		if ( TimeUntilRemove )
			CurrentRecoil -= Data.DecayFactor * Time.Delta;

		if ( pitchOffset > 0f )
		{
			// Figure this magic number out later, it's shit
			pitchOffset *= 8f;
			var newPitch = (CurrentRecoil.y - pitchOffset).Clamp( 0f, Data.MaxRecoil );
			CurrentRecoil = CurrentRecoil.WithY( newPitch );
		}

		CurrentRecoil = CurrentRecoil.Clamp( 0, Data.MaxRecoil );

		if ( PlayerController.Debug )
		{
			DebugOverlay.ScreenText( $"Recoil Amount: {CurrentRecoil}", 25, 0 );
		}
	}

	public struct ComponentData
	{
		public List<Vector2> Presets { get; set; }
		public float RecoveryTime { get; set; }
		public float MaxRecoil { get; set; }
		public float DecayFactor { get; set; }
	}
}
