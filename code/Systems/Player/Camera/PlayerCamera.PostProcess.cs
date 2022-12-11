using Sandbox;

namespace Facepunch.Gunfight;

public partial class PlayerCamera
{
	protected void UpdatePostProcess()
	{
		var postProcess = Camera.Main.FindOrCreateHook<Sandbox.Effects.ScreenEffects>();
		postProcess.Sharpen = 0.05f;
		postProcess.Vignette.Intensity = 0.60f;
		postProcess.Vignette.Roundness = 1f;
		postProcess.Vignette.Smoothness = 0.3f;
		postProcess.Vignette.Color = Color.Black.WithAlpha( 1f );
		postProcess.MotionBlur.Scale = 0f;
		postProcess.Saturation = 1f;

		postProcess.FilmGrain.Response = 1f;
		postProcess.FilmGrain.Intensity = 0.01f;
	}
}
