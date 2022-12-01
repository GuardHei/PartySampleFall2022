using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Deadzone : MonoBehaviour {
	
	public string guardDeathSfx = "GuardSfx-Death";
	public string rototoDeathSfx = "RototoSfx-Death";

	/**
	 * OnTriggerEnter2D() function will be called (automatically) by Unity engine when the current game object
	 * just starts colliding with other game objects. In order to trigger this function call, you need to make
	 * sure both game objects involved in the collision need to have a Collider2D component attached and is
	 * a trigger. Rigidbody2D is not required.
	 */	
	public void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.GetComponent<NEnemyAI3>() || other.gameObject.GetComponent<ShootingAI>())
		{
			if (!string.IsNullOrWhiteSpace(guardDeathSfx) && SfxManager.Instance) SfxManager.Instance.PlaySfx(guardDeathSfx, other.transform.position);
			Destroy(other.gameObject);
			return;
		}
		if (!other.CompareTag("Player")) return;
		if (!string.IsNullOrWhiteSpace(rototoDeathSfx) && SfxManager.Instance) SfxManager.Instance.PlaySfx(rototoDeathSfx, other.transform.position);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
	}
}