using Sandbox;
using Star.Player;
using Star.Util;
using Star.World;

namespace Star.Entities;

public partial class FloatingEntity : Entity, IFloatingOrigin
{
	public FloatingEntity()
	{
		Transmit = TransmitType.Always;
	}
	protected SceneObject SceneObject;
	[Net]
	public Vector3 LocalChunk { get; set; }

	[Net]
	private Vector3 _OriginPosition { get; set; }
	public Vector3 OriginPosition
	{
		get
		{
			return _OriginPosition;
		}
		set
		{
			if ( value.Length >= FloatingManager.ChunkSize )
			{
				var newchunk = new Vector3( value.x / FloatingManager.ChunkSize, value.y / FloatingManager.ChunkSize, value.z / FloatingManager.ChunkSize );
				LocalChunk += newchunk;
				_OriginPosition = value - newchunk * FloatingManager.ChunkSize;
			}
			else
			{
				_OriginPosition = value;
			}
		}
	}
	public Vector3 LocalChunkPosition { get; set; }

	public override void ClientSpawn()
	{
		base.ClientSpawn();
	}

	public virtual void UpdatePosition( Vector3 localchunk )
	{
		var offsetPosition = ((LocalChunk - localchunk) * FloatingManager.ChunkSize) + OriginPosition;
		if ( SceneObject.IsValid() )
			SceneObject.Position = offsetPosition;
	}

	public void UpdatePosition()
	{
		UpdatePosition( (Local.Pawn as Captain).LocalChunk );
	}

	[Event.PreRender]
	private void UpdateRendering()
	{
		if ( Local.Pawn is not Captain player ) return;
		var offsetPosition = ((LocalChunk - player.LocalChunk) * FloatingManager.ChunkSize) + OriginPosition;
		LocalChunkPosition = offsetPosition;
		if ( SceneObject.IsValid() )
			SceneObject.Position = offsetPosition;
	}
	[Event.Tick]
	public virtual void Tick()
	{
		if ( Host.IsServer )
		{
			OriginPosition += Rotation * Vector3.Forward * 500 * Time.Delta;
			Rotation *= Rotation.FromYaw( 1 );

		}
		else if ( Debug.Level > 4 )
		{
			DebugOverlay.Text( $"Origin: {OriginPosition} - LocalChunk: {LocalChunk} - LocalChunkPosition: {LocalChunkPosition}", LocalChunkPosition, 0 );

		}

	}


	public void ViewUpdate( bool insideView )
	{
	}

	public void SetPosition( Vector3 chunk, Vector3 Origin, Vector3 localChunkPosition )
	{
		LocalChunk = chunk;
		OriginPosition = Origin;
		LocalChunkPosition = localChunkPosition;
		UpdateLocalPositions();
	}



	[ClientRpc]
	public void UpdateLocalPositions()
	{
		UpdatePosition( LocalChunk );
	}
}
