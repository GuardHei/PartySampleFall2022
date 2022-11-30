using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    public CollisionDetectionMode collisionDetectionMode;
    public Rigidbody2D rb;
    public string name;
    private bool found = false;
    public float time = 10;
    public float actualTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb.velocity.magnitude == 0) {
            rb.constraints = RigidbodyConstraints2D.FreezePosition;
        }
        actualTime = time;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (!found) {
            name = collider.gameObject.name;
            found = true;
        }
        if (name != collider.gameObject.name) {
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
