using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticSpike : MonoBehaviour {
    
	public Transform startPoint;

	public void OnTriggerEnter2D(Collider2D other) {
		if (!other.CompareTag("Player")) return;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
