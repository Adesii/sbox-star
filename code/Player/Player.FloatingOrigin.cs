using System;
using System.Linq;
using Sandbox;
using Star.World;


namespace Star.Player;

partial class Captain : IFloatingOrigin
{

	[Net, Predicted]
	public Vector3 LocalChunk { get; set; }
	[Net, Predicted]
	public Vector3 OriginPosition { get; set; }
	public Vector3 LocalChunkPosition { get; set; }

	public void UpdatePosition( Vector3 localchunk )
	{
		ResetInterpolation();
	}
	public void ViewUpdate( bool Inside )
	{

	}


	public Vector3 GetRealLocalPosition()
	{
		if ( Host.IsServer ) return LocalChunkPosition;
		return ((LocalChunk - Captain.Local.LocalChunk) * FloatingManager.ChunkSize) + LocalPosition;
	}
}
