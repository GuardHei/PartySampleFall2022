using UnityEngine;

public class StaticSpike : MonoBehaviour {
    
	public Transform startPoint;

	public void OnTriggerEnter2D(Collider2D other) {
		other.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, other.transform.position.z);
	}
}
