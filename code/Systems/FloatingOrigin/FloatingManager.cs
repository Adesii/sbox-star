using Star.Player;

namespace Star.World
{

	public static partial class FloatingManager
	{

		public const int ChunkSize = 512;//8192;

		public const int ViewDistance = 90;
		[ConVar.Replicated]
		public static bool DebugFloatingPoint { get; set; } = false;
		[Event.Tick.Client]
		public static void DebugPlayers()
		{
			int i = 0;
			foreach ( var player in Client.All.Select( x => x.Pawn as Captain ) )
			{
				DebugOverlay.ScreenText( $"Player: {player.Client.Name} - LocalChunk: {player.LocalChunk} - Origin: {player.OriginPosition} - LocalChunkPosition: {player.LocalChunkPosition}", new Vector2( 100, 200 + 10 * i ) );
				if ( player != Local.Pawn )
					DebugOverlay.Sphere( player.GetRealLocalPosition(), 10, Color.Green );
				i++;
			}
		}

		[Event.Tick.Server]
		public static void ServerTick()
		{
			Update();
		}

		public static void MoveLocalOffset( Captain local )
		{
			local.OriginPosition = local.LocalPosition;
			local.LocalChunk = new( local.LocalChunk + local.LocalPosition.Normal );
			local.LocalPosition = Vector3.Zero;
			local.ResetInterpolation();
			Update();

		}
		public static void Update()
		{
			if ( Host.IsServer )
			{
				UpdateLocalPositions();
				return;
			}
			var localChunk = (Local.Pawn as Captain).LocalChunk;
			foreach ( var item in Entity.All.OfType<IFloatingOrigin>() )
			{
				if ( localChunk.Distance( item.LocalChunk ) <= ViewDistance || item == Local.Pawn )
				{
					item.ViewUpdate( true );
					item.UpdatePosition( localChunk );
				}
				else
				{
					item.ViewUpdate( false );
				}
			}
		}

		[ClientRpc]
		public static void UpdateLocalPositions()
		{
			Update();
		}
	}
}
