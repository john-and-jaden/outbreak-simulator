﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    // **************************** //
    // ***** Public variables ***** //
    // **************************** //

    [Tooltip("Current infection status of a person.")]
    public InfectionStatus infectionStatus;
    [Tooltip("Units per second that a person moves.")]
    public float movementSpeed;
    [Tooltip("The radius around a person in which other people can become infected.")]
    public float spreadRadius;
    [Tooltip("Max rotation of a person's direction in degrees per second.")]
    public float maxAngleDeltaPerSecond;

    // ***************************** //
    // ***** Private variables ***** //
    // ***************************** //

    private Vector3 direction;
    private float perlinCoordinate;
    public float recoveryDuration;
    public float recoveryTimer;

    // **************************** //
    // ***** Helper variables ***** //
    // **************************** //

    private SpriteRenderer sr;
    private CircleCollider2D cc2D;

    // *************************** //
    // ***** Unity functions ***** //
    // *************************** //

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        cc2D = GetComponent<CircleCollider2D>();
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        perlinCoordinate = Random.Range(0, 1000);
    }

    void Update()
    {
        Move();
        CollideWithScreenEdges();
        UpdateInfectionStatus();
        UpdateColor();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 dirToOther = collision.transform.position - transform.position;

        // If we are just ran into the other person
        if (Vector3.Dot(direction, dirToOther) > 0)
        {
            Vector3 normal = collision.GetContact(0).normal;
            direction = Vector3.Reflect(direction, normal);
        }

        if (collision.gameObject.GetComponent<Person>().infectionStatus == InfectionStatus.INFECTED && infectionStatus == InfectionStatus.HEALTHY)
        {
            infectionStatus = InfectionStatus.INFECTED;
        }
    }

    // **************************** //
    // ***** Public functions ***** //
    // **************************** //

    public void SetInfectionStatus(InfectionStatus newStatus)
    {
        infectionStatus = newStatus;
    }

    // ***************************** //
    // ***** Private functions ***** //
    // ***************************** //

    private void Move()
    {
        // The value used to move through perlin coordinates - bigger is more chaotic
        perlinCoordinate += Time.deltaTime;
        // Value between -1 and 1
        float perlinScale = (Mathf.PerlinNoise(perlinCoordinate, 0f) * 2) - 1;
        // The amount by which we should rotate the direction this frame
        float angleDelta = maxAngleDeltaPerSecond * perlinScale * Time.deltaTime;
        // Rotate the direction
        direction = Quaternion.AngleAxis(angleDelta, Vector3.forward) * direction;
        // Move the person in the given direction
        transform.position += direction * movementSpeed * Time.deltaTime;
    }

    private void CollideWithScreenEdges()
    {
        // Store these values in local variables just for shorter names
        float radius = cc2D.radius;
        Vector3 pos = transform.position;

        // Get the edges of the camera viewport in world coordinates
        float topEdge = Camera.main.ViewportToWorldPoint(Vector3.up).y;
        float bottomEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
        float leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
        float rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right).x;

        // Bounce the person off of the edges
        if (pos.x < leftEdge + radius)
        {
            direction = Vector3.Reflect(direction, Vector2.right);
        }
        if (pos.x > rightEdge - radius)
        {
            direction = Vector3.Reflect(direction, Vector2.left);
        }
        if (pos.y < bottomEdge + radius)
        {
            direction = Vector3.Reflect(direction, Vector2.up);
        }
        if (pos.y > topEdge - radius)
        {
            direction = Vector3.Reflect(direction, Vector2.down);
        }

        // Clamp the position of the person to the screen space
        float x = Mathf.Clamp(transform.position.x, leftEdge + radius, rightEdge - radius);
        float y = Mathf.Clamp(transform.position.y, bottomEdge + radius, topEdge - radius);
        transform.position = new Vector3(x, y);
    }

    private void UpdateInfectionStatus()
    {
        if (infectionStatus == InfectionStatus.INFECTED)
        {
            recoveryTimer += Time.deltaTime;
            if (recoveryTimer >= recoveryDuration)
            {
                infectionStatus = InfectionStatus.RECOVERED;
            }
        }
    }

    private void UpdateColor()
    {
        if (infectionStatus == InfectionStatus.INFECTED)
        {
            sr.color = Color.red;
        }
        else if (infectionStatus == InfectionStatus.RECOVERED)
        {
            sr.color = Color.green;
        }
    }
}