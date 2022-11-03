﻿using Sandbox;
using Star.Player;
using Star.UI;

namespace Star;

public partial class Game : Sandbox.Game
{
	public static Game Instance => Sandbox.Game.Current as Game;

	public Game()
	{

		if ( IsClient )
		{
			new SceneSkyBox( Map.Scene, Material.Load( "materials/skybox/light_test_interior_basic.vmat" ) );

			Local.Hud?.Delete( true );
			Local.Hud = new Hud();
		}
	}
	public override void ClientSpawn()
	{
		waitabit();
	}
	private async void waitabit()
	{
		await Task.DelaySeconds( 1 );
		new SceneSunLight( Map.Scene, Rotation.LookAt( Vector3.Down - new Vector3( 100, 100, 0 ), Vector3.Up ), Color.White );
		Map.Scene.AmbientLightColor = Color.White * 0.1f;
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var pawn = new Captain();
		client.Pawn = pawn;
	}
}