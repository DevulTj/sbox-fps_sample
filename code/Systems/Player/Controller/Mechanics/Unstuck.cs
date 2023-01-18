using Sandbox;

namespace Facepunch.Gunfight.Mechanics;

public partial class UnstuckMechanic : PlayerControllerMechanic
{
	public int StuckTries { get; set; } = 0;

	TraceResult ActiveTrace;

	protected override bool ShouldStart()
	{
		ActiveTrace = Controller.TraceBBox( Controller.Position, Controller.Position );
		// Not stuck, we cool
		if ( !ActiveTrace.StartedSolid )
		{
			StuckTries = 0;
			return false;
		}

		return true;
	}

	public bool TestAndFix()
	{
		if ( ActiveTrace.StartedSolid )
		{
			if ( PlayerController.Debug )
			{
				DebugOverlay.Text( $"[stuck in {ActiveTrace.Entity}]", Controller.Position, Color.Red );
				DebugOverlay.Box( ActiveTrace.Entity, Color.Red );
			}
		}

		//
		// Client can't jiggle its way out, needs to wait for
		// server correction to come
		//
		if ( Game.IsClient )
			return true;

		int AttemptsPerTick = 20;

		for ( int i = 0; i < AttemptsPerTick; i++ )
		{
			var pos = Controller.Position + Vector3.Random.Normal * (((float)StuckTries) / 2.0f);

			// First try the up direction for moving platforms
			if ( i == 0 )
			{
				pos = Controller.Position + Vector3.Up * 5;
			}

			ActiveTrace = Controller.TraceBBox( pos, pos );

			if ( !ActiveTrace.StartedSolid )
			{
				if ( PlayerController.Debug )
				{
					DebugOverlay.Text( $"unstuck after {StuckTries} tries ({StuckTries * AttemptsPerTick} tests)", Controller.Position, Color.Green, 5.0f );
					DebugOverlay.Line( pos, Controller.Position, Color.Green, 5.0f, false );
				}

				Controller.Position = pos;
				return false;
			}
			else
			{
				if ( PlayerController.Debug )
				{
					DebugOverlay.Line( pos, Controller.Position, Color.Yellow, 0.5f, false );
				}
			}
		}

		StuckTries++;

		return true;
	}

	protected override void Simulate()
	{
		TestAndFix();
	}
}
