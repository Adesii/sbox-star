using Star.Systems.Building;
using Star.Util;

namespace Star.Entities;

public partial class SpaceObject : FloatingEntity
{
	public const int GridSize = 32;

	/// <summary>
	/// The index of the object in the Parts list. so a part can occupy multiple cells. but still count as one part.
	/// </summary>
	[Net]
	public Dictionary<Vector2Int, int> ObjectPartsOccupations { get; set; } = new();

	[Net]
	public List<SpacePartInstance> Parts { get; set; } = new();

	public bool Selected = false;

	[Net]
	public CsgSolid Geometry { get; set; }


	public override void Spawn()
	{
		base.Spawn();
		Geometry = new( 1024 )
		{
			IsStatic = false,
			FloatingParent = this
		};

		//Create a TestObject
		CreateTestMesh();
	}

	public void CreateTestMesh()
	{
		var part = ResourceLibrary.Get<SpacePartDefinition>( "data/adesi/concrete.spart" );
		var partInstance = new SpacePartInstance
		{
			PartDefinition = part,
			Health = 100,
			Position = new( 0, 0 )
		};
		AddNewPart( partInstance );

		var partInstance2 = new SpacePartInstance
		{
			PartDefinition = part,
			Health = 100,
			Position = new( 1, 0 )
		};
		AddNewPart( partInstance2 );
	}

	public void AddNewPart( SpacePartInstance part )
	{
		Parts.Add( part );

		part.AddMeshToGeometry( Geometry );

		var ListOfOccupaiedCells = part.GetOccupation();
		foreach ( var cell in ListOfOccupaiedCells )
		{
			ObjectPartsOccupations[cell] = Parts.Count - 1;
		}

		//update surrounding parts
		var surrounding = new Vector2Int[]
			{
				new( part.Position.x - 1, part.Position.y ),
				new( part.Position.x + 1, part.Position.y ),
				new( part.Position.x, part.Position.y - 1 ),
				new( part.Position.x, part.Position.y + 1 ),
				new( part.Position.x - 1, part.Position.y - 1 ),
				new( part.Position.x + 1, part.Position.y + 1 ),
				new( part.Position.x - 1, part.Position.y + 1 ),
				new( part.Position.x + 1, part.Position.y - 1 ),
			};
		part.RemoveFromGeometry( Geometry );
		part.PaintGeometry( Geometry );

		foreach ( var cell in surrounding )
		{
			if ( ObjectPartsOccupations.ContainsKey( cell ) )
			{
				var parts = Parts[ObjectPartsOccupations[cell]];
				parts.RemoveFromGeometry( Geometry );
				parts.PaintGeometry( Geometry );
			}
		}

	}
	public override void ClientSpawn()
	{
		base.ClientSpawn();
		//CreateMesh();
	}

	public override void Tick()
	{
		base.Tick();
		if ( Selected && IsClient && Geometry.IsValid() && Geometry.PhysicsBody.IsValid() )
		{

			var radius = Geometry.PhysicsBody.GetBounds().Size.Length / 2;
			//create a circle with Lines
			for ( int i = 0; i < 360; i += 10 )
			{
				var angle = i * MathF.PI / 180;
				var x = MathF.Cos( angle ) * radius;
				var y = MathF.Sin( angle ) * radius;
				var x2 = MathF.Cos( angle + 10 * MathF.PI / 180 ) * radius;
				var y2 = MathF.Sin( angle + 10 * MathF.PI / 180 ) * radius;
				DebugOverlay.Line( LocalChunkPosition + new Vector3( x2, y2, 0 ), LocalChunkPosition + new Vector3( x, y, 0 ), Color.Green );
			}

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

	public Vector2Int ToGridIndex( Vector3 buildpos )
	{
		var localpos = Transform.PointToLocal( buildpos );
		var gridpos = new Vector2Int( (int)MathF.Round( localpos.x / GridSize ), (int)MathF.Round( localpos.y / GridSize ) );
		return gridpos;
	}

	public Vector3 ToWorldPosition( Vector2Int index )
	{
		var worldpos = Transform.PointToWorld( new( index.x * GridSize, index.y * GridSize, 0 ) );
		return worldpos;
	}

	[ConCmd.Server]
	public static void AddPartFromClient( int netid, Vector2 index, string v )
	{
		if ( FindByIndex( netid ) is not SpaceObject obj )
		{
			return;
		}
		obj.AddPartAt( index, v );
	}

	private void AddPartAt( Vector2 index, string v )
	{
		var part = ResourceLibrary.Get<SpacePartDefinition>( v );
		var partInstance = new SpacePartInstance
		{
			PartDefinition = part,
			Health = 100,
			Position = new Vector2Int( (int)index.x, (int)index.y )
		};
		AddNewPart( partInstance );
	}
}
