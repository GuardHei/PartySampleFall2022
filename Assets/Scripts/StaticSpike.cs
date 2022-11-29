using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticSpike : MonoBehaviour {
    
	public void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.GetComponent<NEnemyAI3>())
		{
			Destroy(other.gameObject);
			return;
		}
		if (!other.CompareTag("Player")) return;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
