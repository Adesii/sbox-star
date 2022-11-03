using Architect;
using Star.Player;
using Star.World;

namespace Star.Entities;

public partial class SpaceObject : FloatingEntity
{
	public int MaxObjectWidth => 32;
	public int MaxObjectHeight => 32;

	public int CellScale => 32;
	/// <summary>
	/// The parts of the Space object using the IDEnt format as the string.
	/// i.e. "Star.Block.Concrete"
	/// </summary>
	[Net]
	public Dictionary<Vector3Int, string> ObjectParts { get; set; } = new();

	public bool Selected = false;

	private WallObject VisualObject;



	public override void ClientSpawn()
	{
		base.ClientSpawn();
		CreateMesh();
	}
	public override void UpdatePosition( Vector3 localchunk )
	{
		var offsetPosition = ((LocalChunk - localchunk) * FloatingManager.ChunkSize) + OriginPosition;
		LocalChunkPosition = offsetPosition;
		VisualObject.so.Position = offsetPosition;
		VisualObject.so.Bounds = new BBox( -1000, 1000 ) + offsetPosition;
	}
	[Event.Hotload]
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
	}


}
