using System;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerController : MonoBehaviour {
	
	/* Normally, if you declare a variable as public, it should appear on the inspector as long as it can be 
	correctly serialized. Most built-in Unity classes can be shown on the inspector. It is more convenient to 
	modify frequently changed fields through the inspector than codes. */
	public bool enableEditorDebug;

	[Header("Input Settings")]
	public KeyCode leftKey = KeyCode.A;
	public KeyCode rightKey = KeyCode.D;
	public KeyCode jumpKey = KeyCode.W;

	[Header("Movement Settings")]
	[Range(0f, 20f)]
	public float speed = 5;
	[Range(0f, 20f)]
	public float acceleration = 2;
	[Range(0f, 20f)]
	public float jumpPower = 5;
	[Range(0f, 20f)]
	public float highJumpPower = 2;
	[Range(0f, 20f)]
	public float stopPower = 3;
	[Range(0f, 30f)]
	public float gravity = 5;
	public bool canMoveInAir;

	[Header("Graphics Settings")]
	public SpriteRenderer renderer;

	[Header("Physics Settings")]
	public Rigidbody2D rigidbody;
	[Range(0f, 5f)]
	public float bottomOffset = 0.5f;

	[Header("Player Conditions (Do not modify these fields through Editor)")]
	public float currentSpeedX;
	public float currentSpeedY;
	public bool touchesGround;
	public bool isJumping;
	public bool hasCheckedCollision;
	
	/** Start() function will (only) be called (automatically) by Unity engine when the game object attached
	 * by this MonoBehaviour is instantiated and active in the scene. It will be called before all other
	 * message functions. Thus, you can use Start() to initialize your customized MonoBehaviour scripts.
	 * Note that the access token (public/protected/private) does not matter if you want it to be
	 * automatically called by the engine.
	 */
	private void Start() {
		if (enableEditorDebug) Debug.Log("PlayerController.Start() is called.");
	}

	/**
	 * Update() function will be called (automatically) by Unity engine every frame. Normally, you should
	 * add any gameplay-related codes here (eg. input detection, NPC's ai logic and etc). Here we are
	 * dealing with basic player controls.
	 * Note that the access token (public/protected/private) does not matter if you want it to be
	 * automatically called by the engine.
	 */
	private void Update() {
		if (enableEditorDebug) Debug.Log("PlayerController.Update() is called.");

		// Get the time elapsed since last frame.
		float timeElapsed = Time.deltaTime;
		
		// Get the current position of the player.
		Vector2 currentPosition = transform.position;

		if (touchesGround) { // If the player stands on the ground, we need to reset certain fields.
			currentSpeedY = 0;
			isJumping = false;
		} else { // If the player is in the air, we need to apply gravity on the player.
			currentSpeedY -= gravity * timeElapsed;
		}

		// Check the input.
		bool leftPressed = Input.GetKey(leftKey); // Is the player holding down the left key?
		bool rightPressed = Input.GetKey(rightKey); // Is the player holding down the right key?
		bool jumpPressed = Input.GetKey(jumpKey); // Is the player holding down the jump key?

		// Process horizontal speed.
		if (touchesGround || canMoveInAir) { // Only when the player stands on the ground or if we allow it to move in the air.
			if (leftPressed && !rightPressed) { // Iff the resultant input is to the left, we need to accelerate the player's speed to the expected level (-speed).
				currentSpeedX = Mathf.MoveTowards(currentSpeedX, -speed, acceleration * timeElapsed);
			} else if (!leftPressed && rightPressed) { // Iff the resultant input is to the right, we need to accelerate the player's speed to the expected level (speed).
				currentSpeedX = Mathf.MoveTowards(currentSpeedX, speed, acceleration * timeElapsed);
			} else { // If the resultant input is zero, we need to decelerate the player's speed to zero.
				currentSpeedX = Mathf.MoveTowards(currentSpeedX, 0, stopPower * timeElapsed);
			}
		}

		// Process vertical speed.
		if (jumpPressed) { // If the player is holding the jump key, we need to accelerate the player's vertical speed to fight against gravity.
			if (!isJumping) { // Iff it is the first frame the player jumps, we need to give the player a strong upper force.
				isJumping = true;
				currentSpeedY += jumpPower;
			} else { // Iff the player is holding the jump buttons, we still need to give the player a weaker upper force to implement "high jump"; 
				currentSpeedY += highJumpPower * timeElapsed;
			}
		}
		
		// Finally, apply all these changes to the physics system of the player and reset certain fields;
		rigidbody.velocity = new Vector2(currentSpeedX, currentSpeedY);
		touchesGround = false;
		hasCheckedCollision = false;
	}

	/**
	 * OnCollisionStay2D() function will be called (automatically) by Unity engine when the current game object
	 * keeps colliding with other game objects. In order to trigger this function call, you need to make sure both
	 * game objects involved in the collision need to have a Collider2D component attached and is not a trigger.
	 * Also, the current game object need to have a Rigidbody2D component attached of the type "dynamic". Types 
	 * "Kinematic" or "Static" will not trigger this function call.
	 */
	private void OnCollisionStay2D(Collision2D collision) {
		foreach (ContactPoint2D contact in collision.contacts) {
			if (contact.point.y <= transform.position.y - bottomOffset) { // Player touches the ground if any contact point is lower or at the same height as the player's bottom.
				touchesGround = true;
				hasCheckedCollision = true;
				return;
			}
		}
		
		if (!hasCheckedCollision) {
			touchesGround = false; // Otherwise, player is in the air.
			hasCheckedCollision = true;
		} else {
			currentSpeedX = 0; // This is for better character control, try to figure out the function of this line of code by yourself!
		}
	}
}