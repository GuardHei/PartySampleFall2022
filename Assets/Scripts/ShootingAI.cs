using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingAI : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 right;
    public Vector2 left;
    public Vector2 down;
    public SpriteRenderer renderer;
    public float extraDistance = 0.05f;
    public float castDistance = 2.0f;
    public float speed = 3;
    private bool _rightDir = true;
    private GameObject playerObj = null;
    private Vector2 difference;
    public float distance;
    public float sight = 3.0f;
    public bool seen = false;
    public LayerMask wallPlatformColliderMask;
    public float wallDistance = 0.8f;
    public bool flip = true;
    public bool disabled = false;
    public float disableTime = 0;
    public float testtime = 0;
    public bool shoot = false;
    public GameObject projectile;
    public float recoilTime = 2.0f;
    public bool recoil = false;
    public float actualRecoilTime;
    public float proportion;
    public Vector2 launchVelocity;

    //set not_set to false if using total_velocity or else it will be default velocity
    //there is no need to change proportions if using this
    public bool not_set = true;
    public float total_velocity;
    
    public string alertSfx = "GuardSfx-Alert";

    public float maxShootPercent = 100.0f;
    public float minShootPercent = 0.0f;
    private float maxShootRange;
    private float minShootRange;

    
    // Start is called before the first frame update
    void Start()
    {
        if (!playerObj) {
            playerObj = GameObject.FindGameObjectWithTag("Player");
        }
        renderer = GetComponentInChildren(typeof(SpriteRenderer)) as SpriteRenderer;
        rb = gameObject.GetComponent<Rigidbody2D>();
        maxShootRange = sight * maxShootPercent * 0.01f;
        minShootRange = sight * minShootPercent * 0.01f;
    }

    // Update is called once per frame
    void FixedUpdate() 
    {   
        projectile.transform.position = transform.position;
        if (!disabled) {
            if (!shoot) {
                AI ();
            } else {
                if (!recoil) {
                    Shooting ();
                }
                shoot = false;
            }
        }
    }

    void Shooting () {
        GameObject ball = Instantiate(projectile, transform.position, transform.rotation);
        ball.AddComponent<ProjectileScript>();

        if (not_set) {
            float s =  Mathf.Abs(difference.x) * Mathf.Sqrt (2.0f) / (Mathf.Sqrt(Mathf.Abs(difference.x) / 4.9f));
            launchVelocity = new Vector2 (s * proportion, s * proportion);
        } else {
            float b = playerObj.transform.position.x - transform.position.x;
            float a = 4.9f * (b/total_velocity) * (b/total_velocity);
            float sqrtPart = Mathf.Sqrt (b * b - 4.0f * a * (transform.position.y - playerObj.transform.position.y - a));
            float ans1 = Mathf.Atan((-b + sqrtPart) / (-2.0f * a));
            float ans2 = Mathf.Atan((-b - sqrtPart) / (-2.0f * a));
            Debug.Log("s: " + sqrtPart);
            Debug.Log("1: " + ans1);
            Debug.Log("2: " + ans2);
            if (playerObj.transform.position.x > transform.position.x) {
                launchVelocity = new Vector2 (Mathf.Cos(ans2) * proportion * total_velocity, Mathf.Sin(ans2) * proportion * total_velocity);
            } else {
                launchVelocity = new Vector2 (Mathf.Abs(Mathf.Cos(ans1) * proportion * total_velocity) * -1, Mathf.Abs(Mathf.Sin(ans1) * proportion * total_velocity));
            }
            
        }
        
        if (difference.x < 0) {
            launchVelocity.x = Mathf.Abs(launchVelocity.x);
        } else if (difference.x > 0) {
            launchVelocity.x = Mathf.Abs(launchVelocity.x) * -1;
        }

        if (ball.GetComponent<Rigidbody2D>() == null) {
            ball.AddComponent<Rigidbody2D>().AddRelativeForce(launchVelocity);
        } else {
            ball.GetComponent<Rigidbody2D>().AddRelativeForce(launchVelocity);
        }
        recoil = true;
        actualRecoilTime = recoilTime;
    }

    void AI () {
        var seenBefore = seen;
        seen = false;

        Vector2 r = transform.position;
        Vector2 l = transform.position;
        
        if (playerObj != null) {
            difference = transform.position - playerObj.transform.position;
            distance = difference.magnitude;
        }
        
        r.x = r.x + extraDistance;
        l.x = l.x - extraDistance;

        RaycastHit2D hitr = Physics2D.Raycast(r, -Vector2.up, castDistance, wallPlatformColliderMask);
        RaycastHit2D hitl = Physics2D.Raycast(l, -Vector2.up, castDistance, wallPlatformColliderMask);
        RaycastHit2D wallRight = Physics2D.Raycast(transform.position, Vector2.right, wallDistance, wallPlatformColliderMask);
        RaycastHit2D wallLeft = Physics2D.Raycast(transform.position, Vector2.left, wallDistance, wallPlatformColliderMask);
        
        rb.rotation = 0;
        float currSpeed = speed;
        if (playerObj != null) {
            if (distance <= sight) {
                RaycastHit2D look = Physics2D.Linecast (transform.position, playerObj.transform.position, wallPlatformColliderMask);
                if (!look.collider || look.collider == playerObj.GetComponent<Collider2D>()) {
                    seen = true;
                    if (!seenBefore) {
                        if (!string.IsNullOrWhiteSpace(alertSfx) && SfxManager.Instance) SfxManager.Instance.PlaySfx(alertSfx, transform.position);
                    }
                }
            }
        }

        if (!hitr.collider || wallRight.collider != null) {
            _rightDir = false;
        } else if (!hitl.collider || wallLeft.collider != null) {
            _rightDir = true;
        }

        if (distance > minShootRange && distance < maxShootRange && seen) {
            shoot = true;
        } else {
            shoot = false;
        }
        
        var speedY = rb.velocity.y;

        if (!shoot) {
            if (seen) {
                if (transform.position.x > playerObj.transform.position.x) {
                    if (!hitl.collider) {
                        renderer.flipX = !flip;
                        rb.velocity = new Vector2 (0.0f, speedY);
                    } else if (distance < minShootRange) {
                        renderer.flipX = flip;
                        rb.velocity = new Vector2(currSpeed, speedY);
                    } else {
                        renderer.flipX = !flip;
                        rb.velocity = new Vector2(-currSpeed, speedY);
                    }
                } else {
                    if (!hitr.collider) {
                        renderer.flipX = flip;
                        rb.velocity = new Vector2 (0.0f, speedY);
                    } else if (distance < minShootRange) {
                        renderer.flipX = !flip;
                        rb.velocity = new Vector2(-currSpeed, speedY);
                    } else {
                        renderer.flipX = flip;
                        rb.velocity = new Vector2(currSpeed, speedY);
                    }
                }
            } else {
                currSpeed *= _rightDir ? 1.0f : -1.0f;
                if (currSpeed < 0) {
                    renderer.flipX = !flip;
                } else {
                    renderer.flipX = flip;
                }
                rb.velocity = new Vector2(currSpeed, speedY);
            }
        } else {
            rb.velocity = new Vector2 (0.0f, speedY);
        }

        Debug.DrawLine(r, r - new Vector2(0.0f, castDistance), hitr.collider ? Color.green : Color.red);
        Debug.DrawLine(l, l - new Vector2(0.0f, castDistance), hitl.collider ? Color.green : Color.red);
    }

    void Update()
    {
        if (disabled) {

            disableTime = disableTime - Time.deltaTime;

            if (disableTime > 0) {
                disabled = true;
            } else {
                disabled = false;
            }
        }

        if (recoil) {
            actualRecoilTime = actualRecoilTime - Time.deltaTime;
        }

        if (actualRecoilTime <= 0) {
            recoil = false;
        }
        
    }

    public void DisableAI(float increaseTime) 
    {
        if (increaseTime > disableTime) {
            disableTime = increaseTime;
        }
        disabled = true;
    }
}

/*GameObject ball = Instantiate(projectile, transform.position, transform.rotation);
        float s =  difference.x * Mathf.Sqrt (2.0f) / (Mathf.Sqrt(difference.x / 4.9f));
        float ang = Vector2.Angle (transform.position, playerObj.transform.position);
        float x = Mathf.Cos (ang) * s;
        float y = Mathf.Sin (ang) * s;
        launchVelocity = new Vector2 (s * ins, s * ins);
*/