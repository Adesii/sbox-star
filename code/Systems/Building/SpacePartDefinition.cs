using System.Collections.ObjectModel;
using Star.UI;

namespace Star.Systems.Building;

[GameResource( "Space Part Definition", "spart", "The Definition of a Space Part" )]
public partial class SpacePartDefinition : GameResource
{
	public string PartDescription { get; set; }
	public string Category { get; set; } = "Basic";

	[ResourceType( "png" )]
	public string PartIcon { get; set; }

	/* /// <summary>
	/// 0,0 is the center of the object and will always exist.
	/// </summary>
	public List<Vector2Int> Extends { get; set; } = new(){
		new( 0, 0 )
	}; //TODO: Create a custom editor for this. */

	/// <summary>
	/// 0,0 is the center of the object and will always exist.
	/// </summary>
	public Vector2 PartSize { get; set; } = new( 1, 1 );

	public static Dictionary<string, SpacePartDefinition> SpacePartDefinitions = new();

	protected override void PostLoad()
	{
		if ( SpacePartDefinitions == null )
		{
			SpacePartDefinitions = new();
		}
		SpacePartDefinitions[ResourcePath] = this;
		Event.Run( "BuildMode.Changed" );
	}

	protected override void PostReload()
	{
		if ( SpacePartDefinitions == null )
		{
			SpacePartDefinitions = new();
		}
		SpacePartDefinitions[ResourcePath] = this;
		Event.Run( "BuildMode.Changed" );
	}
}
