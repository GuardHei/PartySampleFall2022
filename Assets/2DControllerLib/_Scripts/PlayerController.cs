// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace TarodevController {
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController {
        [SerializeField] private ScriptableStats _stats;

        #region Internal

        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private CapsuleCollider2D _standingCollider;
        private CapsuleCollider2D _col; // current active collider
        private PlayerInput _input;
        private bool _cachedTriggerSetting;

        private FrameInput _frameInput;
        private Vector2 _speed;
        private Vector2 _currentExternalVelocity;
        private int _fixedFrame;
        private bool _hasControl = true;

        #endregion

        #region External

        public event Action<bool, float> GroundedChanged;
        public event Action<bool, Vector2> DashingChanged;
        public event Action<bool> WallGrabChanged;
        public event Action<bool> LedgeClimbChanged;
        public event Action<bool> Jumped;
        public event Action AirJumped;
        public ScriptableStats PlayerStats => _stats;
        public Vector2 Input => _frameInput.Move;
        public Vector2 Speed => _speed;
        public Vector2 GroundNormal => _groundNormal;
        public int WallDirection => _wallDir;
        public bool GrabbingLedge => _grabbingLedge;
        public bool ClimbingLedge => _climbingLedge;

        public virtual void ApplyVelocity(Vector2 vel, PlayerForce forceType) {
            if (forceType == PlayerForce.Burst) _speed += vel;
            else _currentExternalVelocity += vel;
        }

        public virtual void TakeAwayControl(bool resetVelocity = true) {
            if (resetVelocity) _rb.velocity = Vector2.zero;
            _hasControl = false;
        }

        public virtual void ReturnControl() {
            _speed = Vector2.zero;
            _hasControl = true;
        }

        #endregion

        protected virtual void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            _col = _standingCollider;
            _input = GetComponent<PlayerInput>();
            _cachedTriggerSetting = Physics2D.queriesHitTriggers;
            Physics2D.queriesStartInColliders = false;
            Application.targetFrameRate = 60;
        }

        protected virtual void Update() {
            GatherInput();
        }

        protected virtual void GatherInput() {
            _frameInput = _input.FrameInput;

            if (_frameInput.JumpDown) {
                _jumpToConsume = true;
                _frameJumpWasPressed = _fixedFrame;
            }

            if (_frameInput.DashDown && _stats.AllowDash) _dashToConsume = true;

            if (UnityEngine.Input.GetKey(KeyCode.Q)) {
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#else
                Application.Quit();
#endif
            }
        }

        protected virtual void FixedUpdate() {
            _fixedFrame++;

            CheckCollisions();
            HandleCollisions();
            HandleWalls();
            HandleLedges();
            
            HandleJump();
            HandleDash();
            
            HandleHorizontal();
            HandleVertical();
            ApplyVelocity();
        }

        #region Collisions

        private readonly RaycastHit2D[] _groundHits = new RaycastHit2D[2];
        private readonly RaycastHit2D[] _ceilingHits = new RaycastHit2D[2];
        private readonly Collider2D[] _wallHits = new Collider2D[5];
        private Vector2 _groundNormal;
        private int _groundHitCount;
        private int _ceilingHitCount;
        private int _wallHitCount;
        private int _ladderHitCount;
        private int _frameLeftGrounded = int.MinValue;
        private bool _grounded;

        protected virtual void CheckCollisions() {
            Physics2D.queriesHitTriggers = false;
            
            // Ground and Ceiling
            _groundHitCount = Physics2D.CapsuleCastNonAlloc(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _groundHits, _stats.GrounderDistance, ~_stats.PlayerLayer);
            _ceilingHitCount = Physics2D.CapsuleCastNonAlloc(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _ceilingHits, _stats.GrounderDistance, ~_stats.PlayerLayer);

            // Walls and Ladders
            var bounds = GetWallDetectionBounds(); // won't be able to detect a wall if we're crouching mid-air
            _wallHitCount = Physics2D.OverlapBoxNonAlloc(bounds.center, bounds.size, 0, _wallHits, _stats.ClimbableLayer);

            Physics2D.queriesHitTriggers = _cachedTriggerSetting;
        }

        protected virtual bool TryGetGroundNormal(out Vector2 groundNormal) {
            Physics2D.queriesHitTriggers = false;
            var hit = Physics2D.Raycast(_rb.position, Vector2.down, _stats.GrounderDistance * 2, ~_stats.PlayerLayer);
            Physics2D.queriesHitTriggers = _cachedTriggerSetting;
            groundNormal = hit.normal; // defaults to Vector2.zero if nothing was hit
            return hit.collider != null;
        }

        private Bounds GetWallDetectionBounds() {
            var colliderOrigin = _rb.position + _standingCollider.offset;
            return new Bounds(colliderOrigin, _stats.WallDetectorSize);
        }

        protected virtual void HandleCollisions() {
            // Hit a Ceiling
            if (_ceilingHitCount > 0) _speed.y = Mathf.Min(0, _speed.y);

            // Landed on the Ground
            if (!_grounded && _groundHitCount > 0) {
                _grounded = true;
                ResetDash();
                ResetJump();
                GroundedChanged?.Invoke(true, Mathf.Abs(_speed.y));
            }
            // Left the Ground
            else if (_grounded && _groundHitCount == 0) {
                _grounded = false;
                _frameLeftGrounded = _fixedFrame;
                GroundedChanged?.Invoke(false, 0);
            }
        }

        #endregion

        #region Walls

        private float _currentWallJumpMoveMultiplier = 1f; // aka "Horizontal input influence"
        private int _wallDir;
        private bool _isOnWall;
        private bool _isLeavingWall; // prevents immediate re-sticking to wall

        protected virtual void HandleWalls() {
            if (!_stats.AllowWalls) return;

            _currentWallJumpMoveMultiplier = Mathf.MoveTowards(_currentWallJumpMoveMultiplier, 1f, 1f / _stats.WallJumpInputLossFrames);

            // May need to prioritize the nearest wall here... But who is going to make a climbable wall that tight?
            // TODO: Fix here for TileMaps. Won't be able to use the wall's transform.position. Maybe use collider's center if that will be a thing
            _wallDir = _wallHitCount > 0 ? (int)Mathf.Sign(_wallHits[0].transform.position.x - transform.position.x) : 0;

            if (!_isOnWall && ShouldStickToWall()) SetOnWall(true);
            else if (_isOnWall && !ShouldStickToWall()) SetOnWall(false);

            bool ShouldStickToWall() {
                if (_wallDir == 0 || _grounded) return false;
                return _stats.RequireInputPush ? Mathf.Sign(_frameInput.Move.x) == _wallDir : true;
            }
        }

        private void SetOnWall(bool on) {
            _isOnWall = on;
            if (on) _speed = Vector2.zero;
            else {
                _isLeavingWall = false; // after we've left the wall
                ResetAirJumps(); // so that we can air jump even if we didn't leave via a wall jump
            }
            WallGrabChanged?.Invoke(on);
        }

        #endregion

        #region Ledges

        private Vector2 _ledgeCornerPos;
        private bool _grabbingLedge;
        private bool _climbingLedge;

        protected virtual void HandleLedges() {
            if (_climbingLedge || !_isOnWall) return;

            _grabbingLedge = TryGetLedgeCorner(out _ledgeCornerPos);

            if (_grabbingLedge) HandleLedgeGrabbing();
        }
        
        protected virtual bool TryGetLedgeCorner(out Vector2 cornerPos) {
            cornerPos = Vector2.zero;
            Vector2 grabHeight = _rb.position + _stats.LedgeGrabPoint.y * Vector2.up;

            var hit1 = Physics2D.Raycast(grabHeight + _stats.LedgeRaycastSpacing * Vector2.down, _wallDir * Vector2.right, 0.5f, _stats.ClimbableLayer);
            if (!hit1.collider) return false; // Should hit below the ledge. Mainly used to determine xPos accurately

            var hit2 = Physics2D.Raycast(grabHeight + _stats.LedgeRaycastSpacing * Vector2.up, _wallDir * Vector2.right, 0.5f, _stats.ClimbableLayer);
            if (hit2.collider) return false; // we only are within ledge-grab range when the first hits and second doesn't

            var hit3 = Physics2D.Raycast(grabHeight + new Vector2(_wallDir * 0.5f, _stats.LedgeRaycastSpacing), Vector2.down, 0.5f, _stats.ClimbableLayer);
            if (!hit3.collider) return false; // gets our yPos of the corner
            
            cornerPos = new(hit1.point.x, hit3.point.y);
            return true;
        }

        protected virtual void HandleLedgeGrabbing() {
            // Snap to ledge position
            var xInput = _frameInput.Move.x;
            var yInput = _frameInput.Move.y;
            if (yInput != 0 && (xInput == 0 || Mathf.Sign(xInput) == _wallDir) && _hasControl) {
                var pos = _rb.position;
                var targetPos = _ledgeCornerPos - Vector2.Scale(_stats.LedgeGrabPoint, new(_wallDir, 1f));
                _rb.position = Vector2.MoveTowards(pos, targetPos, _stats.LedgeGrabDeceleration * Time.fixedDeltaTime);
            }

            if (yInput > _stats.VerticalDeadzoneThreshold) StartCoroutine(ClimbLedge());
        }

        protected virtual IEnumerator ClimbLedge() {
            LedgeClimbChanged?.Invoke(true);
            _climbingLedge = true;

            TakeAwayControl();
            var targetPos = _ledgeCornerPos - Vector2.Scale(_stats.LedgeGrabPoint, new(_wallDir, 1f));
            transform.position = targetPos;

            float lockedUntil = Time.time + _stats.LedgeClimbDuration;
            while (Time.time < lockedUntil)
                yield return new WaitForFixedUpdate();

            _climbingLedge = false;
            _grabbingLedge = false;
            SetOnWall(false);

            targetPos = _ledgeCornerPos +  Vector2.Scale(_stats.StandUpOffset, new(_wallDir, 1f));
            transform.position = targetPos;
            
            ReturnControl();
            LedgeClimbChanged?.Invoke(false);
        }

        #endregion

        #region Jumping

        private bool _jumpToConsume;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private bool _bufferedJumpUsable;
        private int _frameJumpWasPressed = int.MinValue;
        private int _airJumpsRemaining;

        private bool CanUseCoyote => _coyoteUsable && !_grounded && _fixedFrame < _frameLeftGrounded + _stats.CoyoteFrames;
        private bool HasBufferedJump => _bufferedJumpUsable && _fixedFrame < _frameJumpWasPressed + _stats.JumpBufferFrames;
        private bool CanAirJump => _airJumpsRemaining > 0;

        protected virtual void HandleJump() {
            if (!PlayerStats.AllowJump) return;
            
            if (_jumpToConsume || HasBufferedJump) {
                if (_isOnWall && !_isLeavingWall) WallJump();
                else if (_grounded || CanUseCoyote) NormalJump();
                else if (_jumpToConsume && CanAirJump) AirJump();
            }
            
            _jumpToConsume = false; // Always consume the flag

            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true; // Early end detection
        }

        protected virtual void NormalJump() { // includes ladder jumps
            _endedJumpEarly = false;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _speed.y = _stats.JumpPower;
            Jumped?.Invoke(false);
        }

        protected virtual void WallJump() {
            _endedJumpEarly = false;
            _bufferedJumpUsable = false;
            _isLeavingWall = true;
            _currentWallJumpMoveMultiplier = 0;
            _speed = Vector2.Scale(_stats.WallJumpPower, new(-_wallDir, 1));
            Jumped?.Invoke(true);
        }

        protected virtual void AirJump() {
            _endedJumpEarly = false;
            _airJumpsRemaining--;
            _speed.y = _stats.JumpPower;
            AirJumped?.Invoke();
        }

        protected virtual void ResetJump() {
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            ResetAirJumps();
        }

        protected virtual void ResetAirJumps() => _airJumpsRemaining = _stats.MaxAirJumps;

        #endregion

        #region Dashing

        private bool _dashToConsume;
        private bool _canDash;
        private Vector2 _dashVel;
        private bool _dashing;
        private int _startedDashing;

        protected virtual void HandleDash() {
            if (_dashToConsume && _canDash) {
                var dir = new Vector2(_frameInput.Move.x, Mathf.Max(_frameInput.Move.y, 0f)).normalized;
                if (dir == Vector2.zero) {
                    _dashToConsume = false;
                    return;
                }
                
                _dashVel = dir * _stats.DashVelocity;
                _dashing = true;
                _canDash = false;
                _startedDashing = _fixedFrame;
                DashingChanged?.Invoke(true, dir);

                _currentExternalVelocity = Vector2.zero; // Strip external buildup
            }

            if (_dashing) {
                _speed = _dashVel;
                // Cancel when the time is out or we've reached our max safety distance
                if (_fixedFrame > _startedDashing + _stats.DashDurationFrames) {
                    _dashing = false;
                    DashingChanged?.Invoke(false, Vector2.zero);
                    _speed.y = Mathf.Min(0, _speed.y);
                    _speed.x *= _stats.DashEndHorizontalMultiplier;
                    if (_grounded) ResetDash();
                }
            }

            _dashToConsume = false;
        }

        protected virtual void ResetDash() {
            _canDash = true;
        }

        #endregion

        #region Horizontal

        protected virtual void HandleHorizontal() {
            if (_dashing) return;

            // Deceleration
            if (Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadzoneThreshold)
                _speed.x = Mathf.MoveTowards(_speed.x, 0, (_grounded ? _stats.GroundDeceleration : _stats.AirDeceleration) * Time.fixedDeltaTime);
            // Regular Horizontal Movement
            else {
                // Prevent useless horizontal speed buildup when against a wall
                if (_wallHitCount > 0 && Mathf.Approximately(_rb.velocity.x, 0) && Mathf.Sign(_frameInput.Move.x) == Mathf.Sign(_speed.x))
                    _speed.x = 0;

                var inputX = _frameInput.Move.x;
                _speed.x = Mathf.MoveTowards(_speed.x, inputX * _stats.MaxSpeed, _currentWallJumpMoveMultiplier * _stats.Acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Vertical

        protected virtual void HandleVertical() {
            if (_dashing) return;
            // Grounded & Slopes
            if (_grounded && _speed.y <= 0f) { // TODO: double check this velocity condition. If we're going up a slope, y-speed will be >0
                _speed.y = _stats.GroundingForce;

                if (TryGetGroundNormal(out _groundNormal)) {
                    if (!Mathf.Approximately(_groundNormal.y, 1f)) { // on a slope
                        _speed.y = _speed.x * -_groundNormal.x / _groundNormal.y;
                        if (_speed.x != 0) _speed.y += _stats.GroundingForce;
                    }
                }
            }
            // Wall Climbing & Sliding
            else if (_isOnWall && !_isLeavingWall) {
                if (_frameInput.Move.y > 0) _speed.y = _stats.WallClimbSpeed;
                else if (_frameInput.Move.y < 0) _speed.y = -_stats.MaxWallFallSpeed;
                else if (_grabbingLedge) _speed.y = Mathf.MoveTowards(_speed.y, 0, _stats.LedgeGrabDeceleration * Time.fixedDeltaTime);
                else _speed.y = Mathf.MoveTowards(Mathf.Min(_speed.y, 0), -_stats.MaxWallFallSpeed, _stats.WallFallAcceleration * Time.fixedDeltaTime);
            }
            // In Air
            else {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _speed.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                _speed.y = Mathf.MoveTowards(_speed.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        protected virtual void ApplyVelocity() {
            if (!_hasControl) return;
            _rb.velocity = _speed + _currentExternalVelocity;

            _currentExternalVelocity = Vector2.MoveTowards(_currentExternalVelocity, Vector2.zero, _stats.ExternalVelocityDecay * Time.fixedDeltaTime);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            if (_stats == null) return;

            if (_stats.ShowWallDetection && _standingCollider != null) {
                Gizmos.color = Color.white;
                var bounds = GetWallDetectionBounds();
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
            if (_stats.ShowLedgeDetection) {
                Gizmos.color = Color.red;
                var facingDir = Mathf.Sign(_wallDir);
                var grabHeight = transform.position + _stats.LedgeGrabPoint.y * Vector3.up;
                var grabPoint = grabHeight + facingDir * _stats.LedgeGrabPoint.x * Vector3.right;
                Gizmos.DrawWireSphere(grabPoint, 0.05f);
                Gizmos.DrawWireSphere(grabPoint + Vector3.Scale(_stats.StandUpOffset, new(facingDir, 1)), 0.05f);
                Gizmos.DrawRay(grabHeight + _stats.LedgeRaycastSpacing * Vector3.down, 0.5f * facingDir * Vector3.right);
                Gizmos.DrawRay(grabHeight + _stats.LedgeRaycastSpacing * Vector3.up, 0.5f * facingDir * Vector3.right);
            }
        }

        private void OnValidate() {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
            if (_standingCollider == null) Debug.LogWarning("Please assign a Capsule Collider to the Standing Collider slot", this);
            if (_rb == null) _rb = GetComponent<Rigidbody2D>(); // serialized but hidden in the inspector
        }
#endif
    }

    public interface IPlayerController {
        /// <summary>
        /// true = Landed. false = Left the Ground. float is Impact Speed
        /// </summary>
        public event Action<bool, float> GroundedChanged;
        public event Action<bool, Vector2> DashingChanged; // Dashing - Dir
        public event Action<bool> WallGrabChanged;
        public event Action<bool> LedgeClimbChanged;
        public event Action<bool> Jumped; // Is wall jump
        public event Action AirJumped;

        public ScriptableStats PlayerStats { get; }
        public Vector2 Input { get; }
        public Vector2 Speed { get; }
        public Vector2 GroundNormal { get; }
        public int WallDirection { get; }
        public bool GrabbingLedge { get; }
        public bool ClimbingLedge { get; }
        public void ApplyVelocity(Vector2 vel, PlayerForce forceType);
    }

    public enum PlayerForce {
        /// <summary>
        /// Added directly to the players movement speed, to be controlled by the standard deceleration
        /// </summary>
        Burst,

        /// <summary>
        /// An additive force handled by the decay system
        /// </summary>
        Decay
    }
}