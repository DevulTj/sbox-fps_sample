using Sandbox;
using System;
using System.Linq;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace Facepunch.Gunfight;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class GunfightGameManager : GameManager
{
	public GunfightGameManager()
	{
		if ( Game.IsServer )
		{
			_ = new Hud();
		}
	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( IClient client )
	{
		base.ClientJoined( client );

		// Create a pawn for this client to play with
		var pawn = new Player();
		client.Pawn = pawn;
		pawn.Respawn();

		// Get all of the spawnpoints
		var spawnpoints = Entity.All.OfType<SpawnPoint>();

		// chose a random one
		var randomSpawnPoint = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		// if it exists, place the pawn there
		if ( randomSpawnPoint != null )
		{
			var tx = randomSpawnPoint.Transform;
			tx.Position += tx.Rotation.Up * 10.0f;
			pawn.Transform = tx;
		}

		Chat.AddChatEntry( To.Everyone, client.Name, "joined the game", client.SteamId, true );
	}

	public override void ClientDisconnect( IClient client, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( client, reason );
		Chat.AddChatEntry( To.Everyone, client.Name, "left the game", client.SteamId, true );
	}

	public override void DoPlayerDevCam( IClient client )
	{
		// do nothing
	}
}
