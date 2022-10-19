using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class PulseGunController : MonoBehaviour {
    public PlayerController player;
    public float ampDistance = 5; // max distance to determine amplifying the force
    public float force = 5;
    public PlayerForce forceType = PlayerForce.Burst;

    private Camera _cam;
    private Vector2 _pointer;

    private void Start() {
        _cam = Camera.main;
    }

    private void Update() {
        // get pointer vector from pulse gun to mouse (using world positions)
        var mousePos = Input.mousePosition;
        var mousePosInWorld = _cam.ScreenToWorldPoint(mousePos);
        _pointer = mousePosInWorld - transform.position;
        _pointer.Normalize();
        
        // calculate degree and set rotation of pulse gun
        var degree = Mathf.Atan2(_pointer.y, _pointer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * degree);

        // handle pulse gun force on left mouse click
        if (Input.GetMouseButtonDown(0)) HandleForce();
    }

    private void HandleForce() {
        // raycast to check for objects in front of the gun
        // Note: should refine to allow hits in a larger area -- simulate wave-like force of pulse gun
        var hit = Physics2D.CircleCast((Vector2) transform.position + _pointer * .1f, .1f, _pointer);
        
        // apply extra force if the force is applied close to an object
        // Note: the distance (ampDistance) to apply this force is adjustable
        if (hit.collider && hit.distance <= ampDistance) {
            // print("Close Hit: Extra Launch!");
            player.ApplyVelocity(-force * _pointer, forceType); // won't work on current player controller
        }
    }
}
