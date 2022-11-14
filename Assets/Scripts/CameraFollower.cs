using TarodevController;
using UnityEngine;

public class CameraFollower : MonoBehaviour {

	[Header("Target")]
	public PlayerController player;

	[Header("Settings")]
	[Range(0f, 100f)]
	public float maxHorizontalSpeed;
	[Range(0f, 100f)]
	public float maxVerticalSpeed;
	[Range(0f, 10f)]
	public float horizontalSmoothTime;
	[Range(0f, 10f)]
	public float verticalSmoothTime;
	public Vector3 offset;

	[Header("Status (Do not modify these fields through Editor)")]
	public float currentHorizontalSpeed;
	public float currentVerticalSpeed;
	public Vector3 actualOffset;
	public bool xFlip;
	public bool lastXFlip;

	/**
	 * LateUpdate() function will be called (automatically) by Unity engine every frame after Update(). Normally, you should
	 * add functions that require other components to complete here (eg. camera that tracks current frame player's position). Here we are
	 * dealing with basic camera controls.
	 * Note that the access token (public/protected/private) does not matter if you want it to be
	 * automatically called by the engine.
	 */
	private void LateUpdate() {
		var playerPosition = player.transform.position;
		actualOffset = offset;
		lastXFlip = xFlip;
		xFlip = player.Speed.x < 0;
		if (player.Speed.x == 0f) xFlip = lastXFlip;
		if (xFlip) actualOffset.x = -actualOffset.x;
		var targetPosition = playerPosition + actualOffset;
		var position = transform.position;
		var positionX = Mathf.SmoothDamp(position.x, targetPosition.x, ref currentHorizontalSpeed, horizontalSmoothTime, maxHorizontalSpeed);
		var positionY = Mathf.SmoothDamp(position.y, targetPosition.y, ref currentVerticalSpeed, verticalSmoothTime, maxVerticalSpeed);
		position = new Vector3(positionX, positionY, offset.z); // We should not modify the z axis of the camera.
		transform.position = position;
	}
	
	
}