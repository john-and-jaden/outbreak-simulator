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
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Vector3 normal = collision.GetContact(0).normal;
        // direction = Vector3.Reflect(direction, normal);
        infected = true;
    }
}
