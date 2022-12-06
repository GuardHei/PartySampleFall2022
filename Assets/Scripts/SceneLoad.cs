using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour {
	public string nextLevelName;
		
	public void OnTriggerEnter2D(Collider2D other) => SceneManager.LoadScene(nextLevelName, LoadSceneMode.Single);

	public void LoadLevel(string levelName) => SceneManager.LoadScene(levelName, LoadSceneMode.Single);

	void Update() {
		if (Input.GetKeyUp(KeyCode.M)) SceneManager.LoadScene("Title", LoadSceneMode.Single);
		if (Input.GetKeyUp(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		if (Input.GetKeyUp(KeyCode.Q)) {
			#if UNITY_EDITOR
			EditorApplication.ExitPlaymode();
			#else
			Application.Quit();
			#endif
		}
	}
}
