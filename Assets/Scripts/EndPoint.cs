using UnityEngine;

public class EndPoint : MonoBehaviour {

	public Color reachedColor;
	
	[Header("Graphics Settings")]
	public SpriteRenderer flagRenderer;
	public SpriteRenderer rodRenderer;

	/**
	 * OnTriggerEnter2D() function will be called (automatically) by Unity engine when the current game object
	 * just starts colliding with other game objects. In order to trigger this function call, you need to make
	 * sure both game objects involved in the collision need to have a Collider2D component attached and the
	 * Collider2D component for the current game objects need to be a trigger, while the other game object
	 * needs to have a Rigidbody2D component attached of type "dynamic". Types "Kinematic" or "Static" will
	 * not trigger this function call.
	 */
	public void OnTriggerEnter2D(Collider2D other) {
		// If the player hits (or "triggers") the end point, we will change its color.
		flagRenderer.color = reachedColor;
		rodRenderer.color = reachedColor;
	}
}