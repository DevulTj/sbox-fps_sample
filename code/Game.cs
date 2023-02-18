﻿global using Sandbox;
global using Sandbox.UI;
global using System;
global using System.Linq;
global using System.Collections.Generic;
global using System.ComponentModel;

//
global using GameTemplate.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace GameTemplate;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class TemplateGameManager : GameManager
{
	public TemplateGameManager()
	{
		if ( Game.IsClient )
		{
			Game.RootPanel = new Hud();
		}
		else
		{
			Game.TickRate = 30;
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
			tx.Position = tx.Position + Vector3.Up * 10.0f; // raise it up
			pawn.Transform = tx;
		}

		Chat.AddChatEntry( To.Everyone, client.Name, "joined the game", client.SteamId, true );
	}

	public override void ClientDisconnect( IClient client, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( client, reason );
		Chat.AddChatEntry( To.Everyone, client.Name, "left the game", client.SteamId, true );
	}
}
