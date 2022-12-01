using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticSpike : MonoBehaviour {

	public string guardDeathSfx = "GuardSfx-Death";
	public string rototoDeathSfx = "RototoSfx-Death";
    
	public void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.TryGetComponent(out NEnemyAI3 ai3)) {
			if (!ai3.dead && !string.IsNullOrWhiteSpace(guardDeathSfx) && SfxManager.Instance) SfxManager.Instance.PlaySfx(guardDeathSfx, other.transform.position);
			ai3.dead = true;
			Destroy(other.gameObject);
			return;
		}
		
		if (other.gameObject.TryGetComponent(out ShootingAI sai)) {
			if (!sai.dead && !string.IsNullOrWhiteSpace(guardDeathSfx) && SfxManager.Instance) SfxManager.Instance.PlaySfx(guardDeathSfx, other.transform.position);
			sai.dead = true;
			Destroy(other.gameObject);
			return;
		}
		
		if (!other.CompareTag("Player")) return;
		if (!string.IsNullOrWhiteSpace(rototoDeathSfx) && SfxManager.Instance) SfxManager.Instance.PlaySfx(rototoDeathSfx, other.transform.position);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
