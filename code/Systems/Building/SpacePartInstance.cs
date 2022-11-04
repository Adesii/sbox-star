namespace Star.Systems.Building;

public partial class SpacePartInstance : BaseNetworkable
{
	[Net]
	public SpacePartDefinition PartDefinition { get; set; }

	[Net]
	public int Health { get; set; }


	/// <summary>
	/// Use Sparingly, this is a very expensive operation. because of the amount of parts per object.
	/// </summary>
	public virtual void TickPart()
	{
	}
}
