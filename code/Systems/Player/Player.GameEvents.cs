using Facepunch.Gunfight.WeaponSystem;
using Sandbox.Diagnostics;
using System.Linq;

namespace Facepunch.Gunfight;

public partial class Player
{
	private Logger eventLogger = new Logger( "player/GameEvent" );

	public void RunGameEvent( string eventName )
	{
		eventName = eventName.ToLowerInvariant();

		Inventory.ActiveWeapon?.Components.GetAll<WeaponComponent>()
			.ToList().ForEach( x => x.OnGameEvent( eventName ) );

		Controller.Mechanics.ForEach( x => x.OnGameEvent( eventName ) );

		OnGameEvent( eventName );
	}

	public void OnGameEvent( string eventName )
	{
		eventLogger.Trace( $"OnGameEvent ({eventName})" );
	}
}
