using UnityEngine;

public class Deadzone : MonoBehaviour {

	public Transform startPoint;

	/**
	 * OnTriggerEnter2D() function will be called (automatically) by Unity engine when the current game object
	 * just starts colliding with other game objects. In order to trigger this function call, you need to make
	 * sure both game objects involved in the collision need to have a Collider2D component attached and is
	 * a trigger. Rigidbody2D is not required.
	 */
	public void OnTriggerEnter2D(Collider2D other) {
		// If the player hits (or "triggers") the deadzone, we will teleport the player back to the start point.
		other.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, other.transform.position.z);
	}
}