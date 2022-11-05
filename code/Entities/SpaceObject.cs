using Architect;
using Sandbox.Csg;
using Star.Player;
using Star.Systems.Building;
using Star.Util;
using Star.World;

namespace Star.Entities;

public partial class SpaceObject : FloatingEntity
{
	public int MaxObjectWidth => 32;
	public int MaxObjectHeight => 32;

	public int CellScale => 32;
	/// <summary>
	/// The index of the object in the Parts list. so a part can occupy multiple cells. but still count as one part.
	/// </summary>
	[Net]
	public Dictionary<Vector2Int, int> ObjectPartsOccupations { get; set; } = new();

	[Net]
	public List<SpacePartInstance> Parts { get; set; } = new();

	public bool Selected = false;

	private WallObject VisualObject;

	[Net]
	public CsgSolid Geometry { get; set; }




	public override void Spawn()
	{
		base.Spawn();
		Geometry = new( 1024 );
		Geometry.Add( Game.Instance.CubeBrush,
			Game.Instance.DefaultMaterial,
			scale: new Vector3( 8000, 8000, 100f ) );
		Geometry.IsStatic = false;
		Geometry.FloatingParent = this;

	}
	public override void ClientSpawn()
	{
		base.ClientSpawn();
		//CreateMesh();
	}
	public override void UpdatePosition( Vector3 localchunk )
	{
		base.UpdatePosition( localchunk );
		if ( VisualObject == null || !VisualObject.so.IsValid() ) return;
		VisualObject.so.Position = LocalChunkPosition - new Vector3( (VisualObject.GridSize * MaxObjectWidth) / 2 );
		VisualObject.so.Bounds = new BBox( -1000, 1000 ) + LocalChunkPosition;
	}
	//[Event.Hotload]
	private void CreateMesh()
	{
		if ( IsServer ) return;
		var mult = CellScale / 32;
		var gridSize = new Vector2( MaxObjectWidth * mult, MaxObjectHeight * mult );
		var mapBounds = new Vector3( MaxObjectWidth * CellScale, MaxObjectHeight * CellScale, 64 );

		VisualObject?.Destroy();
		VisualObject = new( Map.Scene, Map.Physics, gridSize, mapBounds );

		VisualObject.HEMesh.CreateGrid( MaxObjectWidth * mult, MaxObjectHeight * mult );

		var my = (int)(VisualObject.GridSize.x / MaxObjectWidth);
		var mx = (int)(VisualObject.GridSize.y / MaxObjectHeight);

		var rect = new Rect( MaxObjectHeight / 2, MaxObjectHeight / 2, 1, 1 );

		var left = (int)(rect.Left * mx);
		var right = (int)(rect.Right * mx);
		var bottom = (int)(rect.Bottom * my);
		var top = (int)(rect.Top * my);

		VisualObject.AddEdge( left, bottom, left, top, 1 );
		VisualObject.AddEdge( right, bottom, right, top, 1 );
		VisualObject.AddEdge( left, bottom, right, bottom, 1 );
		VisualObject.AddEdge( left, top, right, top, 1 );


		var recty = new Rect( 10, 0, 5, 5 );

		var lefty = (int)(recty.Left * mx);
		var righty = (int)(recty.Right * mx);
		var bottomy = (int)(recty.Bottom * my);
		var topy = (int)(recty.Top * my);

		VisualObject.AddEdge( lefty, bottomy, lefty, topy, 1 );
		VisualObject.AddEdge( righty, bottomy, righty, topy, 1 );
		VisualObject.AddEdge( lefty, bottomy, righty, bottomy, 1 );
		VisualObject.AddEdge( lefty, topy, righty, topy, 1 );



		VisualObject.RebuildMesh();

		UpdatePosition();
	}

	public override void Tick()
	{
		base.Tick();
		if ( VisualObject != null && Captain.Local.IsBuildMode && Selected )
		{
			VisualObject.DebugDraw( LocalChunkPosition );
		}

		if ( Debug.Level >= 1 && IsClient )
		{
			DebugOverlay.Sphere( LocalChunkPosition, 50, Color.Red );
			if ( Geometry.IsValid() )
			{
				DebugOverlay.Sphere( Geometry.LocalChunkPosition, 70, Color.Green );
			}
		}
	}


}
