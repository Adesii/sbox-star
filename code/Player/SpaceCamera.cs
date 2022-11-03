using Star.World;

namespace Star.Player;

public class SpaceCamera : CameraMode
{
	public Vector3 CameraOffsetPosition;
	public Vector3 CameraOffset => new Vector3( -25, 0, 100 );

	public Vector3 Velocity;

	public override void Activated()
	{
		var cameraConfig = Systems.Config.Current.Camera;

		FieldOfView = cameraConfig.FOV;

		ZNear = cameraConfig.ZNear;
		ZFar = cameraConfig.ZFar;
		Ortho = cameraConfig.Ortho;

		base.Activated();
	}

	public override void Update()
	{
		if ( Local.Pawn is not Captain player ) return;


		Velocity = Vector3.Zero;

		CameraOffsetPosition = CameraOffset;
		CameraOffsetPosition *= 1.2f * player.ZoomLevel;
		CameraOffsetPosition.z = 50 * player.ZoomLevel;

		Position = player.LocalPosition + (player.LocalRotation * CameraOffsetPosition);

		Rotation = Rotation.LookAt( player.LocalPosition - Position, Vector3.Up );
	}


}
