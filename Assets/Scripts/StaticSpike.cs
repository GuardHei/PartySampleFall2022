using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticSpike : MonoBehaviour {

	public string guardDeathSfx = "GuardSfx-Death";
	public string rototoDeathSfx = "RototoSfx-Death";
    
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
