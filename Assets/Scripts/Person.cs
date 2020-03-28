using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public float speed;
    private Vector3 direction;
    private bool infected;
    private Rigidbody2D rb2D;
    private SpriteRenderer sr;
    private CircleCollider2D cc2D;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cc2D = GetComponent<CircleCollider2D>();
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        direction.Normalize();
        rb2D.velocity = direction * speed;
        Debug.Log(Vector3.Reflect(new Vector3(-1, 1), Vector3.down));
    }

    // Update is called once per frame
    void Update()
    {
        // transform.position += direction * speed * Time.deltaTime;
        // rigidbody.AddForce(direction * speed);
        if (infected)
        {
            sr.color = Color.red;
        }

        CollideWithScreenEdges();
    }

    private void CollideWithScreenEdges()
    {
        Vector3 leftEdgeTempVec = transform.position - new Vector3(cc2D.radius, 0);
        Vector3 pos = Camera.main.WorldToViewportPoint(leftEdgeTempVec);
        if (pos.x < 0.0)
        {
            rb2D.velocity = Vector3.Reflect(rb2D.velocity, Vector3.right);
        }
        Vector3 rightEdgeTempVec = transform.position + new Vector3(cc2D.radius, 0);
        pos = Camera.main.WorldToViewportPoint(rightEdgeTempVec);
        if (pos.x > 1.0)
        {
            rb2D.velocity = Vector3.Reflect(rb2D.velocity, Vector3.right);
        }
        Vector3 bottomEdgeTempVec = transform.position - new Vector3(0, cc2D.radius);
        pos = Camera.main.WorldToViewportPoint(bottomEdgeTempVec);
        if (pos.y < 0.0)
        {
            rb2D.velocity = Vector3.Reflect(rb2D.velocity, Vector3.down);
        }
        Vector3 topEdgeTempVec = transform.position + new Vector3(0, cc2D.radius);
        pos = Camera.main.WorldToViewportPoint(topEdgeTempVec);
        if (pos.y > 1.0)
        {
            rb2D.velocity = Vector3.Reflect(rb2D.velocity, Vector3.up);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 normal = collision.GetContact(0).normal;
        direction = Vector3.Reflect(direction, normal);
        rb2D.velocity = direction * speed;
        infected = true;
    }
}
