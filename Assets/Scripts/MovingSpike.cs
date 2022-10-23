using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpike : MonoBehaviour {
    public Transform pointA;
    public Transform pointB;
    public bool isRight = true;
    public float speed = 0.3f;
    public Rigidbody2D rb;

    void Start() {
        transform.position = pointA.position;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (isRight) {
            Vector2 endPoint = Vector2.MoveTowards(rb.position, pointB.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(endPoint);
            if (rb.position == (Vector2) pointB.position) {
                isRight = false;
            } 
        } else {
            Vector2 startPoint = Vector2.MoveTowards(rb.position, pointA.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(startPoint);
            if (rb.position == (Vector2) pointA.position) {
                isRight = true;
            }
        }
    }
}
