using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour {
  public string nextLevelName;

  public void OnTriggerEnter2D(Collider2D other) {
			 SceneManager.LoadScene(nextLevelName, LoadSceneMode.Single);
		}
}
