using System;
using UnityEngine;

public class EndPoint : MonoBehaviour {

	public Color reachedColor;

	private SpriteRenderer[] _renderers;

	private void Awake() {
		_renderers = GetComponentsInChildren<SpriteRenderer>();
	}

	/**
	 * OnTriggerEnter2D() function will be called (automatically) by Unity engine when the current game object
	 * just starts colliding with other game objects. In order to trigger this function call, you need to make
	 * sure both game objects involved in the collision need to have a Collider2D component attached and the
	 * Collider2D component for the current game objects need to be a trigger, while the other game object
	 * needs to have a Rigidbody2D component attached of type "dynamic". Types "Kinematic" or "Static" will
	 * not trigger this function call.
	 */
	public void OnTriggerEnter2D(Collider2D other) {
		if (!other.CompareTag("Player")) return;
		// If the player hits (or "triggers") the end point, we will change its color.
		foreach (var renderer in _renderers) renderer.color = reachedColor;
		// Debug.Log("Hit the flag!");
	}
}