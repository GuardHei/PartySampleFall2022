using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectileScript : MonoBehaviour
{

    public CollisionDetectionMode collisionDetectionMode;
    public Rigidbody2D rb;
    public string name;
    private bool found = false;
    public float time = 10;
    public float actualTime;
    
    public string rototoDeathSfx = "RototoSfx-Death";

    public int characterLayer;

    // Start is called before the first frame update
    void Start()
    {
        characterLayer = LayerMask.NameToLayer("Character");
        rb = GetComponent<Rigidbody2D>();
        if (rb.velocity.magnitude == 0) {
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
        }
        actualTime = time;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.layer == characterLayer) return;
        if (!found) {
            name = collider.gameObject.name;
            found = true;
        }
        if (collider.CompareTag("Player")) {
            if (!string.IsNullOrWhiteSpace(rototoDeathSfx) && SfxManager.Instance) SfxManager.Instance.PlaySfx(rototoDeathSfx, collider.transform.position);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (name != collider.gameObject.name) {
            Debug.Log("Projectile destroyed");
            Destroy (this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude != 0) {
            rb.constraints = RigidbodyConstraints2D.None;
        }
        actualTime = actualTime - Time.deltaTime;
        if (actualTime <= 0) {
            Destroy (this.gameObject);
        }
    }
}
