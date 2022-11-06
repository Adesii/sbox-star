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

	public new Transform Transform
	{
		get
		{
			return new Transform( LocalChunkPosition, Rotation, Scale );
		}
		set
		{
			OriginPosition = value.Position; // TODO: Figure out if this should be LocalChunkPosition Instead for the Client
			Rotation = value.Rotation;
			Scale = value.Scale;
		}
	}


	[Net]
	public FloatingEntity FloatingParent { get; set; }
	public Vector3 LocalChunkPosition { get; set; }
	[Net]
	public Vector3 LocalParentPosition { get; set; }

	public override void ClientSpawn()
	{
		base.ClientSpawn();
	}

	public virtual void UpdatePosition( Vector3 localchunk )
	{
		if ( FloatingParent.IsValid() )
		{
			FloatingParent.UpdatePosition( localchunk );
			LocalChunkPosition = FloatingParent.LocalChunkPosition + LocalParentPosition;
		}
		else
		{

			LocalChunkPosition = ((LocalChunk - localchunk) * FloatingManager.ChunkSize) + OriginPosition;
		}
		if ( SceneObject.IsValid() )
			SceneObject.Position = LocalChunkPosition;
	}

	public void UpdatePosition()
	{
		UpdatePosition( (Local.Pawn as Captain).LocalChunk );
	}

	[Event.PreRender]
	private void UpdateRendering()
	{
		if ( Local.Pawn is not Captain player ) return;
		UpdatePosition();
		if ( SceneObject.IsValid() )
		{
			SceneObject.Transform = Transform;

		}
	}
	[Event.Tick]
	public virtual void Tick()
	{
		/* if ( Host.IsServer )
		{
			OriginPosition += Rotation * Vector3.Forward * 500 * Time.Delta;
			Rotation *= Rotation.FromYaw( 1 );

		}
		else */
		if ( Debug.Level > 4 )
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


	public Vector3 ConvertWorldToFloating( Vector3 position )
	{
		UpdatePosition( LocalChunk );
		return position - LocalChunkPosition;
	}


}
