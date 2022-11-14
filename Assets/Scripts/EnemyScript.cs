using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector2 right;
    public Vector2 left;
    public Vector2 down;
    public SpriteRenderer renderer;
    public float extradistance = 0.05f;
    public float castdistance = 2.0f;
    public float speed = 3;
    private bool _rightDir = true;

    // Start is called before the first frame update
    void Start()
    {
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
        
        rb.rotation = 0;
        float currSpeed = speed;

        if (!hitr.collider) {
            _rightDir = false;
        } else if (!hitl.collider) {
            _rightDir = true;
        }

        currSpeed *= _rightDir ? 1.0f : -1.0f;

        rb.velocity = new Vector2(currSpeed, 0.0f);
        
        Debug.DrawLine(r, r - new Vector2(0.0f, castdistance), hitr.collider ? Color.green : Color.red);
        Debug.DrawLine(l, l - new Vector2(0.0f, castdistance), hitl.collider ? Color.green : Color.red);
        
    }

    void Update()
    {
        
    }
}

/*Vector2 r = transform.position;
        Vector2 l = transform.position;

        r.x = r.x + right.x + extradistance;
        r.y = r.y + right.y;
        l.x = l.x + left.x - extradistance;
        l.y = l.y + left.y;

        RaycastHit2D hitr = Physics2D.Raycast(r, -Vector2.up, 1f);
        RaycastHit2D hitl = Physics2D.Raycast(l, -Vector2.up, 1f);

        rb.rotation = 0;
        
        if (hitr.collider == null || hitl.collider == null) {
            speed = speed * -1;
        }

        rb.velocity = new Vector2(speed, 0.0f);*/