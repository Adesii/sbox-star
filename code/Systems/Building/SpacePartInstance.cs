using Star.Entities;

namespace Star.Systems.Building;

public partial class SpacePartInstance : BaseNetworkable
{
	[Net]
	public SpacePartDefinition PartDefinition { get; set; }

	[Net]
	public int Health { get; set; }

	[Net]
	public Vector2Int Position { get; set; }

	private Vector3 LocalOffset => new( Position.x * SpaceObject.GridSize, Position.y * SpaceObject.GridSize, 0 );

	private Vector3 ObjectSize => new Vector3( SpaceObject.GridSize * PartDefinition.PartSize.x,
			 					SpaceObject.GridSize * PartDefinition.PartSize.y,
			 					SpaceObject.GridSize * 1.5f );


	/// <summary>
	/// Use Sparingly, this is a very expensive operation. because of the amount of parts per object.
	/// </summary>
	public virtual void TickPart()
	{
	}

	internal void AddMeshToGeometry( CsgSolid geometry )
	{
		var brush = Game.Instance.CubeBrush;
		geometry.Add( brush, Game.Instance.DefaultMaterial, position: LocalOffset,
			scale: ObjectSize * new Vector3( 1.5f, 1.5f, 1.14f ) );



	}



	internal List<Vector2Int> GetOccupation()
	{
		//is the size of the part in both directions
		var ListOfOccupaiedCells = new List<Vector2Int>();
		for ( int x = (int)-PartDefinition.PartSize.x / 2; x < PartDefinition.PartSize.x / 2; x++ )
		{
			for ( int y = (int)-PartDefinition.PartSize.y / 2; y < PartDefinition.PartSize.y / 2; y++ )
			{
				ListOfOccupaiedCells.Add( new( Position.x + x, Position.y + y ) );
			}
		}

		return ListOfOccupaiedCells;
	}

	internal void PaintGeometry( CsgSolid geometry )
	{
		var brush = Game.Instance.CubeBrush;
		var wallpaint = Game.Instance.RedMaterial;
		var floorpaint = Game.Instance.ScorchedMaterial;
		geometry.Paint( brush, floorpaint, position: LocalOffset, scale: ObjectSize * new Vector3( 1f, 1f, 1.14f ) );
		geometry.Paint( brush, wallpaint, position: LocalOffset + Vector3.Up * 5.2f, scale: ObjectSize * new Vector3( 1.1f, 1.1f, 1 ) );
	}

	internal void RemoveFromGeometry( CsgSolid geometry )
	{
		var brush = Game.Instance.CubeBrush;
		geometry.Subtract( brush, position: LocalOffset + Vector3.Up * 5f, scale: ObjectSize );
	}
}
