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
    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Vector3 normal = collision.GetContact(0).normal;
        // direction = Vector3.Reflect(direction, normal);
        infected = true;
    }
}
