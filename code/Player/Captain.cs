using Sandbox.Internal;
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

	public List<SpaceObject> SelectedObjects { get; set; } = new();


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
		TraceMouse();
	}

	public void TraceMouse()
	{
		if ( Input.Pressed( InputButton.PrimaryAttack ) && !IsBuildMode && IsClient )
		{
			var ray = new Ray( CurrentView.Position, Screen.GetDirection( Mouse.Position ) );
			var idk = Trace.Ray( ray, 8192f ).IncludeClientside().Ignore( this ).Run();
			//DebugOverlay.TraceResult( idk, 2 );
			if ( idk is { Hit: true, HitPosition: var pos } hit )
			{
				if ( FindEntityForPhysicsBody( hit.Body ) is CsgSolid solid )
				{
					if ( solid.FloatingParent is SpaceObject spaceObject )
					{
						spaceObject.Selected = true;
						SelectedObjects.Add( spaceObject );
					}
					/* solid.UpdatePosition();
					AddOnServer( solid.NetworkIdent, pos - solid.LocalChunkPosition, scale, rotation ); */
				}
			}
			else
			{
				SelectedObjects.ForEach( x => x.Selected = false );
				SelectedObjects.Clear();
			}
		}
	}

	public Entity FindEntityForPhysicsBody( PhysicsBody body )
	{
		var topbody = body;
		while ( topbody.Parent != null )
		{
			topbody = topbody.Parent;
		}
		return Entity.All.OfType<FloatingModelEntity>().FirstOrDefault( x => x.PhysicsBody == topbody );
	}
	public Vector3 WorldToFloating( Vector3 pos )
	{
		return pos - (LocalChunk * FloatingManager.ChunkSize);
	}

	[ConCmd.Server]
	private static void AddOnServer( int solidid, Vector3 pos, float scale, Rotation rotation )
	{
		var solid = Entity.FindByIndex( solidid ) as CsgSolid;
		solid.Add( Game.Instance.CubeBrush, Game.Instance.RedMaterial, pos, scale, rotation );
	}

	[ConCmd.Server]
	private static void SubtractOnServer( int solidid, Vector3 pos, float scale, Rotation rotation )
	{
		var solid = Entity.FindByIndex( solidid ) as CsgSolid;
		solid.Subtract( Game.Instance.DodecahedronBrush, pos, scale, rotation );
		solid.Paint( Game.Instance.DodecahedronBrush, Game.Instance.ScorchedMaterial, pos, scale + 16f, rotation );
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
	[ConCmd.Server]
	public static void DeleteAll()
	{
		foreach ( var entity in Entity.All.OfType<FloatingEntity>() )
		{
			entity.Delete();
		}
	}
}
