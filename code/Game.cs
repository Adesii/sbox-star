using Sandbox;

using Star.Player;
using Star.UI;

namespace Star;

public partial class Game : Sandbox.Game
{
	public static Game Instance => Sandbox.Game.Current as Game;


	public CsgBrush CubeBrush { get; } = ResourceLibrary.Get<CsgBrush>( "brushes/cube.csg" );
	public CsgBrush DodecahedronBrush { get; } = ResourceLibrary.Get<CsgBrush>( "brushes/dodecahedron.csg" );

	public CsgMaterial DefaultMaterial { get; } = ResourceLibrary.Get<CsgMaterial>( "materials/csgdemo/default.csgmat" );
	public CsgMaterial RedMaterial { get; } = ResourceLibrary.Get<CsgMaterial>( "materials/csgdemo/red.csgmat" );
	public CsgMaterial ScorchedMaterial { get; } = ResourceLibrary.Get<CsgMaterial>( "materials/csgdemo/scorched.csgmat" );


	public Game()
	{

		if ( IsClient )
		{
			new SceneSkyBox( Map.Scene, Material.Load( "materials/skybox/light_test_interior_basic.vmat" ) );

			Local.Hud?.Delete( true );
			Local.Hud = new Hud();
			//new SceneSunLight( Map.Scene, Rotation.LookAt( Vector3.Down - new Vector3( 100, 100, 0 ), Vector3.Up ), Color.White );
			Map.Scene.AmbientLightColor = Color.White * 0.5f;
		}
	}
	public override void ClientSpawn()
	{
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var pawn = new Captain();
		client.Pawn = pawn;
	}
}
