using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEnemyAIScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 right;
    public Vector2 left;
    public Vector2 down;
    public SpriteRenderer renderer;
    public float extradistance = 1.0f;
    public float castdistance = 2.0f;
    public float speed = 3;
    public float chasespeed = 0.03f;
    private bool _rightDir = true;
    private Vector2 lastposition;
    public float radius = 4.0f;
    private Vector2 chase;
    private GameObject playerObj = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!playerObj) {
            playerObj = GameObject.FindGameObjectWithTag("Player");
            chase.y = 0;
        }
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate() 
    {   
        Vector2 r = transform.position;
        Vector2 l = transform.position;

        r.x = r.x + extradistance;
        l.x = l.x - extradistance;

        RaycastHit2D hitr = Physics2D.Raycast(r, -Vector2.up, castdistance);
        RaycastHit2D hitl = Physics2D.Raycast(l, -Vector2.up, castdistance);
        var hitc = Physics2D.OverlapCircle(transform.position, radius, 4);

        rb.rotation = 0;
        float currSpeed = speed;
        if (hitc != null) {
            chase.x = playerObj.transform.position.x;
            if ((!hitl.collider && chase.x <= transform.position.x) || (!hitr.collider && chase.x >= transform.position.x)) {
                rb.velocity = new Vector2(0,0);
            } else {
                transform.position = Vector2.MoveTowards(transform.position, chase, chasespeed);
            }
        }
        else {
            if (!hitr.collider) {
                _rightDir = false;
            } else if (!hitl.collider) {
                _rightDir = true;
            }
            currSpeed *= _rightDir ? 1.0f : -1.0f;
            rb.velocity = new Vector2(currSpeed, 0.0f);
        }
        
        Debug.DrawLine(r, r - new Vector2(0.0f, castdistance), hitr.collider ? Color.green : Color.red);
        Debug.DrawLine(l, l - new Vector2(0.0f, castdistance), hitl.collider ? Color.green : Color.red);
        Debug.Log(hitc);
        
    }

    void Update()
    {
        
    }
}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEnemyAIScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 right;
    public Vector2 left;
    public Vector2 down;
    public SpriteRenderer renderer;
    public float extradistance = 1.0f;
    public float castdistance = 2.0f;
    public float speed = 3;
    public float chasespeed = 0.03f;
    private bool _rightDir = true;
    private Vector2 prevelocity;
    public float radius = 4.0f;
    private Vector2 chase;
    private GameObject playerObj = null;

    // Start is called before the first frame update
    void Start()
    {
        if (!playerObj) {
            playerObj = GameObject.FindGameObjectWithTag("Player");
            chase.y = 0;
        }
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate() 
    {   
        Vector2 r = transform.position;
        Vector2 l = transform.position;

        r.x = r.x + extradistance;
        l.x = l.x - extradistance;

        RaycastHit2D hitr = Physics2D.Raycast(r, -Vector2.up, castdistance);
        RaycastHit2D hitl = Physics2D.Raycast(l, -Vector2.up, castdistance);
        var hitc = Physics2D.OverlapCircle(transform.position, radius, 4);

        rb.rotation = 0;
        float currSpeed = speed;
        if (hitc != null) {
            if (!hitl.collider || !hitr.collider) {
                rb.velocity = new Vector2(0,0);
            } else {
                chase.x = playerObj.transform.position.x;
                transform.position = Vector2.MoveTowards(transform.position, chase, chasespeed);
            }
        }
        else {
            if (!hitr.collider) {
                _rightDir = false;
            } else if (!hitl.collider) {
                _rightDir = true;
            }
            currSpeed *= _rightDir ? 1.0f : -1.0f;
            rb.velocity = new Vector2(currSpeed, 0.0f);
        }
        
        Debug.DrawLine(r, r - new Vector2(0.0f, castdistance), hitr.collider ? Color.green : Color.red);
        Debug.DrawLine(l, l - new Vector2(0.0f, castdistance), hitl.collider ? Color.green : Color.red);
        Debug.Log(hitc);
        
    }

    void Update()
    {
        
    }
}*/