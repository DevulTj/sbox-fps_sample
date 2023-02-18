using GameTemplate.Weapons;
using Sandbox.Diagnostics;

namespace GameTemplate;

public partial class Player
{
	static string realm = Game.IsServer ? "server" : "client";
	static Logger eventLogger = new Logger( $"player/GameEvent/{realm}" );

	public void RunGameEvent( string eventName )
	{
		eventName = eventName.ToLowerInvariant();

		Inventory.ActiveWeapon?.Components.GetAll<WeaponComponent>()
			.ToList()
			.ForEach( x => x.OnGameEvent( eventName ) );

		Controller.Mechanics.ToList()
			.ForEach( x => x.OnGameEvent( eventName ) );

		eventLogger.Trace( $"OnGameEvent ({eventName})" );
	}
}
