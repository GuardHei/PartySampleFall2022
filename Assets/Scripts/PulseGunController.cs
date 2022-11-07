using System;
using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;
using UnityEngine.Rendering;

public class PulseGunController : MonoBehaviour
{
    public PlayerController player;
    public float ampDistance = 5; // max distance to determine amplifying the force
    public float force = 5;
    public float maxCooldown = 1;
    public float maxAngleSweep = 10;
    public PlayerForce forceType = PlayerForce.Burst;
    public Gradient minToMaxGradient;
    public GameObject hitVisual;
    public LayerMask detectionLayer;
<<<<<<< Updated upstream
=======
    public bool detectTriggers;
    public int smokeParticleNum = 50;
    public ParticleSystem smokeFx;
    public Sprite activeSprite;
    public Sprite inactiveSprite;
>>>>>>> Stashed changes

    private Camera _cam;
    private Vector2 _pointer;
    private bool _canUse = true;
    private SpriteRenderer _renderer;
    private RaycastHit2D _contactPoint;

    private void Start() {
        // note: this uses the main camera
        _cam = Camera.main;
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _renderer.sprite = activeSprite;
        
        // prepare visual
        hitVisual = Instantiate(hitVisual);
        hitVisual.SetActive(false);
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
        
        // find all valid colliders in the hit zone
        _contactPoint = ConeCast(_pointer, ampDistance, maxAngleSweep);

        // render hit visuals
        if (_contactPoint)
        {
            var point = _contactPoint.point;
            var dist = Vector2.Distance(point, transform.position);
            hitVisual.transform.position = point;
            hitVisual.GetComponent<SpriteRenderer>().color = minToMaxGradient.Evaluate(dist / ampDistance);
            hitVisual.SetActive(true);
        }
        else
        {
            hitVisual.SetActive(false);
        }

        // handle pulse gun force on left mouse click
        if (Input.GetMouseButtonDown(0)) HandleForce();
    }

    private void HandleForce() {        
        // cooldown Check
        if (!_canUse) return;

        // apply velocity if contact point exists
        if (_contactPoint)
            player.ApplyVelocity(-force * _pointer, forceType);

        StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine() {
        _canUse = false;
        _renderer.sprite = inactiveSprite;
        yield return new WaitForSeconds(maxCooldown);
        _canUse = true;
        _renderer.sprite = activeSprite;
    }


    // from a ray, will sweep out the given angle from both the left and the right to check for collisions
    private RaycastHit2D ConeCast(Vector2 direction, float maxDistance, float coneAngle) {
        // gets angle from direction vector
        float initAngle =  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // makes a RayCast sweep from -coneAngle to +coneAngle with the direction vector in the center
        for (int angleShift = 0; angleShift < coneAngle; angleShift++){
            Vector2 leftDirection = new Vector2(
                Mathf.Cos((initAngle-angleShift)*Mathf.Deg2Rad),
                Mathf.Sin((initAngle-angleShift)*Mathf.Deg2Rad)
            ) * maxDistance;
            Vector2 rightDirection = new Vector2(
                Mathf.Cos((initAngle+angleShift)*Mathf.Deg2Rad),
                Mathf.Sin((initAngle+angleShift)*Mathf.Deg2Rad)
            ) * maxDistance;

            var origin = transform.position;
            var hit1 = Physics2D.Raycast(origin, leftDirection, maxDistance, detectionLayer);
            var hit2 = Physics2D.Raycast(origin, rightDirection, maxDistance, detectionLayer);
            if (hit1) return hit1;
            if (hit2) return hit2;
        }
        return new RaycastHit2D();
    }
    
}
