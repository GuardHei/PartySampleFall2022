using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class CheckpointManager : MonoBehaviour
{
    private static Vector2 lastTouched;
    private static int currentSceneIndex = -1;
    private static List<Vector3> currentFlagsHit = new List<Vector3>();

    private bool reached = false;
    private SpriteRenderer[] _renderers;
    public Color reachedColor = new Color(15, 128, 255);
    public float fadeSpeed = .1f;

    void Awake()
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>();
        GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];

        //If we have switched to a new level since last death, teleport player to Startpoint
        if(currentSceneIndex != SceneManager.GetActiveScene().buildIndex){
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            currentFlagsHit.Clear();
            
            if(gameObject.tag == "Startpoint"){
                lastTouched = gameObject.transform.position;
                player.transform.position = lastTouched;
            }
        }

        //Else teleport player to last checkpoint
        else if((Vector2) gameObject.transform.position == lastTouched){
            player.transform.position = lastTouched;
        }

        foreach (var flagPos in currentFlagsHit){
            if(gameObject.transform.position == flagPos){
                Destroy(gameObject);
            }
        }
    }

    public IEnumerator Fade(float speed){
        while (_renderers[0].color.a > 0){
            foreach (var renderer in _renderers) {
                float fadeAmount = renderer.color.a - speed * Time.deltaTime;
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, fadeAmount);
                yield return null;
            }
        }
        Destroy(gameObject,.1f);
    }

	public void OnTriggerEnter2D(Collider2D other) {
		if (!other.CompareTag("Player") || reached) return;
		// If the player hits (or "triggers") the checkpoint, we will update the lastTouched coords and color the flag.
        foreach (var renderer in _renderers) renderer.color = reachedColor;
        		
        lastTouched = gameObject.transform.position;
        currentFlagsHit.Add(gameObject.transform.position);
        reached = true;

        //If hit, fade out the checkpoint so it can't be hit again
        StartCoroutine(Fade(fadeSpeed));
	}
}
