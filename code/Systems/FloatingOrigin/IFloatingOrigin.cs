using System;
using Star.Player;

namespace Star.World
{
	public interface IFloatingOrigin
	{
		Vector3 LocalChunk { get; set; }
		Vector3 OriginPosition { get; set; }
		Vector3 LocalChunkPosition { get; set; }

		void UpdateLocalPositions()
		{
			LocalChunkPosition = ((LocalChunk - (Local.Pawn as Captain).LocalChunk) * FloatingManager.ChunkSize) + OriginPosition;
		}

		void UpdatePosition( Vector3 localchunk );
		void ViewUpdate( bool insideView );
	}
}
