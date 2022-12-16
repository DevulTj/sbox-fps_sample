using Facepunch.Gunfight;
using Sandbox;
using Sandbox.UI;

public partial class Hud : HudEntity<RootPanel>
{
	public Hud()
	{
		if ( !Game.IsClient )
			return;

		RootPanel.StyleSheet.Load( "/UI/Hud.scss" );
		RootPanel.AddChild<Chat>();
		RootPanel.AddChild<Info>();
		RootPanel.AddChild<Players>();
		RootPanel.AddChild<Crosshair>();
	}
}
