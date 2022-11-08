using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEnemyAI2 : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        if (!playerObj) {
            playerObj = GameObject.FindGameObjectWithTag("Player");
        }
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate() 
    {   
        seen = false;

        Vector2 r = transform.position;
        Vector2 l = transform.position;
        difference = transform.position - playerObj.transform.position;
        distance = difference.magnitude;

        r.x = r.x + extraDistance;
        l.x = l.x - extraDistance;

        RaycastHit2D hitr = Physics2D.Raycast(r, -Vector2.up, castDistance, wallPlatformColliderMask);
        RaycastHit2D hitl = Physics2D.Raycast(l, -Vector2.up, castDistance, wallPlatformColliderMask);
        RaycastHit2D wallRight = Physics2D.Raycast(transform.position, Vector2.right, wallDistance, wallPlatformColliderMask);
        RaycastHit2D wallLeft = Physics2D.Raycast(transform.position, Vector2.left, wallDistance, wallPlatformColliderMask);
        
        rb.rotation = 0;
        float currSpeed = speed;
        
        if (distance <= sight) {
            RaycastHit2D look = Physics2D.Linecast (transform.position, playerObj.transform.position, wallPlatformColliderMask);
            if (!look.collider || look.collider == playerObj.GetComponent<Collider2D>()) {
                seen = true;
            }
        }

        if (!hitr.collider || wallRight.collider != null) {
            _rightDir = false;
        } else if (!hitl.collider || wallLeft.collider != null) {
            _rightDir = true;
        }

        if (seen) {
            if (transform.position.x > playerObj.transform.position.x) {
                if (!hitl.collider) {
                    rb.velocity = new Vector2 (0.0f, 0.0f);
                } else {
                    rb.velocity = new Vector2(-currSpeed, 0.0f);
                }
            } else {
                if (!hitr.collider) {
                    rb.velocity = new Vector2 (0.0f, 0.0f);
                } else {
                    rb.velocity = new Vector2(currSpeed, 0.0f);
                }
            }
        } else {
            currSpeed *= _rightDir ? 1.0f : -1.0f;
            rb.velocity = new Vector2(currSpeed, 0.0f);
        }

        Debug.DrawLine(r, r - new Vector2(0.0f, castDistance), hitr.collider ? Color.green : Color.red);
        Debug.DrawLine(l, l - new Vector2(0.0f, castDistance), hitl.collider ? Color.green : Color.red);
        
    }

    void Update()
    {
        
    }
}