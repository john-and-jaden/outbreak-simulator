using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public float speed;
    private Vector3 direction;
    private bool infected;
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D cc2D;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cc2D = GetComponent<CircleCollider2D>();
        direction = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        rigidbody.velocity = direction * speed;
    }

    // Update is called once per frame
    void Update()
    {
        // transform.position += direction * speed * Time.deltaTime;
        // rigidbody.AddForce(direction * speed);
        if (infected)
        {
            spriteRenderer.color = Color.red;
        }

        CollideWithScreenEdges();
    }

    private void CollideWithScreenEdges()
    {
        Vector3 leftEdgeTempVec = transform.position - new Vector3(cc2D.radius, 0);
        Vector3 pos = Camera.main.WorldToViewportPoint(leftEdgeTempVec);
        if (pos.x < 0.0)
        {
            rigidbody.velocity = Vector3.Reflect(rigidbody.velocity, Vector3.right);
        }
        Vector3 rightEdgeTempVec = transform.position + new Vector3(cc2D.radius, 0);
        pos = Camera.main.WorldToViewportPoint(rightEdgeTempVec);
        if (pos.x > 1.0)
        {
            rigidbody.velocity = Vector3.Reflect(rigidbody.velocity, Vector3.right);
        }
        Vector3 bottomEdgeTempVec = transform.position - new Vector3(0, cc2D.radius);
        pos = Camera.main.WorldToViewportPoint(bottomEdgeTempVec);
        if (pos.y < 0.0)
        {
            rigidbody.velocity = Vector3.Reflect(rigidbody.velocity, Vector3.down);
        }
        Vector3 topEdgeTempVec = transform.position + new Vector3(0, cc2D.radius);
        pos = Camera.main.WorldToViewportPoint(topEdgeTempVec);
        if (pos.y > 1.0)
        {
            rigidbody.velocity = Vector3.Reflect(rigidbody.velocity, Vector3.up);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Vector3 normal = collision.GetContact(0).normal;
        // direction = Vector3.Reflect(direction, normal);
        infected = true;
    }
}
