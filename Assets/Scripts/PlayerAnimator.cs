using System;
using TarodevController;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    public PlayerController controller;
    public Animator animator;
    public SpriteRenderer renderer;

    private int _onGroundId = Animator.StringToHash("OnGround");
    private int _movingId = Animator.StringToHash("Moving");
    private bool _moving;

    private void Awake() {
        if (!animator) animator = GetComponent<Animator>();
        if (!renderer) renderer = GetComponent<SpriteRenderer>();
        if (controller) controller.GroundedChanged += SetOnGroundState;
        if (renderer) renderer.flipX = false;
    }

    private void Update() {
        if (!animator) return;
        var speed = controller.Speed;
        _moving = Mathf.Abs(speed.x) != .0f;
        animator.SetBool(_movingId, _moving);
        var input = controller.Input.x;
        if (Mathf.Abs(input) != .0f) renderer.flipX = input < .0f;
    }

    private void SetOnGroundState(bool onGround, float verticalSpeed) {
        animator.SetBool(_onGroundId, onGround);
    }
}
