using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class KaifPulseGunController : MonoBehaviour
{
    public PlayerController player;
    public float ampDistance = 5; // max distance to determine amplifying the force
    public float force = 5;
    public float maxCooldown = 1;
    public float maxAngleSweep = 10;
    public PlayerForce forceType = PlayerForce.Burst;
    public Color readyColor = Color.cyan;

    private Camera _cam;
    private Vector2 _pointer;
    private bool _canUse = true;
    private SpriteRenderer _renderer;
    private Color _capsuleColor;

    private void Start() {
        _cam = Camera.main;
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _capsuleColor = readyColor;
        _renderer.color = _capsuleColor;
    }

    private void Update() {
        // get pointer vector from pulse gun to mouse (using world positions)
        var mousePos = Input.mousePosition;
        var mousePosInWorld = _cam.ScreenToWorldPoint(mousePos);
        _pointer = mousePosInWorld - transform.position;
        _pointer.Normalize();
        
        //Updates gun color every frame
        _renderer.color = _capsuleColor;

        // calculate degree and set rotation of pulse gun
        var degree = Mathf.Atan2(_pointer.y, _pointer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * degree);

        // handle pulse gun force on left mouse click
        if (Input.GetMouseButtonDown(0)) HandleForce();
    }

    private void HandleForce() {        
        //Cooldown Check
        if (!_canUse) return;

        // Change cone angle for wider sweep (ConeCast to check pulse)
        var hit = ConeCast(transform.position, _pointer, ampDistance, maxAngleSweep);

        // apply extra force if the force is applied close to an object
        // Note: the distance (ampDistance) to apply this force is adjustable
        if (hit.collider && hit.distance <= ampDistance) {
            player.ApplyVelocity(-force * _pointer, forceType);
        }

        StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine() {
        _canUse = false;
        _capsuleColor = Color.white;
        yield return new WaitForSeconds(maxCooldown);
        _canUse = true;
        _capsuleColor = readyColor;
    }


    //From a ray, will sweep out the given angle from both the left and the right to check for collisions
    public static RaycastHit2D ConeCast(Vector2 origin, Vector2 direction, float maxDistance, float coneAngle) {
    //Gets angle from direction vector
        float initAngle =  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Debug.Log(initAngle);

        //Makes a RayCast sweep from -coneAngle to +coneAngle with the direction vector in the center
        for (int angleShift = 0; angleShift < coneAngle; angleShift++){
            Vector2 leftDirection = new Vector2(
                Mathf.Cos((initAngle-angleShift)*Mathf.Deg2Rad),
                Mathf.Sin((initAngle-angleShift)*Mathf.Deg2Rad)
            ) * maxDistance;
            Vector2 rightDirection = new Vector2(
                Mathf.Cos((initAngle+angleShift)*Mathf.Deg2Rad),
                Mathf.Sin((initAngle+angleShift)*Mathf.Deg2Rad)
            ) * maxDistance;

            var hit1 = Physics2D.Raycast(origin, leftDirection, maxDistance);
            var hit2 = Physics2D.Raycast(origin, rightDirection, maxDistance);
            Debug.DrawRay(origin, rightDirection, Color.green,1);
            Debug.DrawRay(origin, leftDirection, Color.blue,1);
            if (hit1) return hit1;
            if (hit2) return hit2;
        }
        return new RaycastHit2D();
    }
}
