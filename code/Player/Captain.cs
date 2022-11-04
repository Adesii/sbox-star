using Star.Entities;
using Star.Util;
using Star.World;

namespace Star.Player;

public partial class Captain : Entity
{
	public static Captain Local => Sandbox.Local.Pawn as Captain;

	[Net, Predicted]
	public float ZoomLevel { get; set; }

	public float Speed => 100f;



	public bool IsBuildMode { get; set; }


	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;

		Components.GetOrCreate<SpaceCamera>();

		ZoomLevel = 5f;
	}

	public void ZoomBy( float amount )
	{
		ZoomLevel -= amount;
		ZoomLevel = ZoomLevel.Clamp( 1f, 1000f );
	}

	[Net, Predicted]
	public TimeSince LastChunkChange { get; set; }

	public override void BuildInput( InputBuilder inputBuilder )
	{
		base.BuildInput( inputBuilder );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( Input.Down( InputButton.Use ) )
		{
			Rotation = Rotation.RotateAroundAxis( Vector3.Up, 100f * Time.Delta );
		}
		if ( Input.Down( InputButton.Menu ) )
		{
			Rotation = Rotation.RotateAroundAxis( Vector3.Up, -100f * Time.Delta );
		}


		ZoomBy( Input.MouseWheel );


		var movement = new Vector3( Input.Forward, Input.Left, 0 );
		if ( Input.Down( InputButton.Run ) )
		{
			movement *= 2f;
		}

		movement *= Speed * (ZoomLevel * 0.25f);
		movement *= Rotation;

		LocalPosition += movement * Time.Delta;


		if ( LocalPosition.Length >= FloatingManager.ChunkSize )
		{
			FloatingManager.MoveLocalOffset( this );
		}


		Debugging();
	}

	private void Debugging()
	{
		if ( !FloatingManager.DebugFloatingPoint || !Debug.Enabled ) return;
		DebugOverlay.Box( -new Vector3( FloatingManager.ChunkSize ), new( FloatingManager.ChunkSize ), Color.Red, 0 );
		DebugOverlay.Sphere( GetRealLocalPosition(), 10f, Host.IsServer ? Color.Red : Color.Blue );

	}

	[ConCmd.Server]
	public static void CreateTestSpaceCube()
	{
		if ( ConsoleSystem.Caller.Pawn is not Captain captain ) return;

		var idk = new SpaceObject();
		idk.SetPosition( captain.LocalChunk, captain.LocalPosition, captain.LocalChunkPosition );
	}
}
