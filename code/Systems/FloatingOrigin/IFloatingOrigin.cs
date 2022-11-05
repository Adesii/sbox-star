using System;
using Star.Entities;
using Star.Player;

namespace Star.World
{
	public interface IFloatingOrigin
	{
		Vector3 LocalChunk { get; set; }
		Vector3 OriginPosition { get; set; }
		Vector3 LocalChunkPosition { get; set; }
		//This is Relative to the Floating Parent
		Vector3 LocalParentPosition { get; set; }

		FloatingEntity FloatingParent { get; set; }

		void UpdateLocalPositions()
		{
			if ( FloatingParent.IsValid() )
			{
				FloatingParent.UpdateLocalPositions();
				LocalChunkPosition = FloatingParent.LocalChunkPosition + LocalParentPosition;
				return;
			}
			LocalChunkPosition = ((LocalChunk - (Local.Pawn as Captain).LocalChunk) * FloatingManager.ChunkSize) + OriginPosition;
		}

		void UpdatePosition( Vector3 localchunk );
		void ViewUpdate( bool insideView );
	}
}
