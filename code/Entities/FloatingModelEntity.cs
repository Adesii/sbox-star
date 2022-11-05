namespace Star.Entities;

public class FloatingModelEntity : FloatingEntity
{
	public PhysicsBody PhysicsBody { get; set; }
	public PhysicsWorld PhysicsWorld { get; set; }



	public bool PhysicsEnabled
	{
		get
		{
			if ( PhysicsBody == null ) return false;
			return PhysicsBody.Enabled;
		}
		set
		{
			if ( PhysicsBody == null ) return;
			PhysicsBody.Enabled = value;
		}
	}

	public bool EnableSolidCollisions
	{
		get
		{
			if ( PhysicsBody == null ) return false;
			return PhysicsBody.EnableSolidCollisions;
		}
		set
		{
			if ( PhysicsBody == null ) return;
			PhysicsBody.EnableSolidCollisions = value;
		}
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();
		SceneObject = new SceneObject( Map.Scene, Model.Load( "models/arrow.vmdl" ) );
	}

	public PhysicsGroup SetupPhysicsFromSphere( PhysicsMotionType motionType, Vector3 center, float radius )
	{
		if ( PhysicsWorld == null && Host.IsServer )
		{
			PhysicsWorld = new();
		}
		else if ( Host.IsClient )
		{
			PhysicsWorld = Map.Physics;
		}
		PhysicsBody = new PhysicsBody( PhysicsWorld );
		PhysicsBody.AddSphereShape( center, radius );
		return PhysicsWorld.Group;
	}

	public override void UpdatePosition( Vector3 localchunk )
	{
		base.UpdatePosition( localchunk );
		if ( PhysicsBody.IsValid() )
		{
			PhysicsBody.Transform = Transform;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PhysicsBody?.Remove();
	}
}
