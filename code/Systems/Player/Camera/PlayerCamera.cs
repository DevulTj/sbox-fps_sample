using Sandbox;

namespace Facepunch.Gunfight;

public partial class PlayerCamera
{
	public virtual void BuildInput( Player player )
	{
		//
	}

	public virtual void Update( Player player )
	{
		Camera.Position = player.EyePosition;
		Camera.Rotation = player.EyeRotation;
		Camera.FieldOfView = Game.Preferences.FieldOfView;
		Camera.FirstPersonViewer = player;
		Camera.ZNear = 0.5f;

		UpdatePostProcess();
	}
}
