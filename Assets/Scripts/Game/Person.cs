using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
  // **************************** //
  // ***** Public variables ***** //
  // **************************** //

  [Tooltip("Units per second that a person moves.")]
  public float movementSpeed;

  [Tooltip("Max rotation of a person's direction in degrees per second.")]
  public float maxAngleDeltaPerSecond;

  [Tooltip("The radius around a person in which other people can become infected.")]
  public float maxInfectionRadius;

  [Tooltip("The time since becoming infected for the infection radius to reach max value.")]
  public float infectionRadiusGrowthDuration;

  [Tooltip("The amount of time a person will take to recover since becoming infected.")]
  public float recoveryDuration;

  [Tooltip("The color of infected people.")]
  public Color infectedColor;

  [Tooltip("The color of recovered people.")]
  public Color recoveredColor;

  [Tooltip("Prefab of the InfectionRadiusRenderer object to spawn.")]
  public SpriteRenderer infectionRadiusPrefab;

  // ***************************** //
  // ***** Private variables ***** //
  // ***************************** //

  private InfectionStatus infectionStatus;
  private Vector3 direction;
  private float infectionRadius;
  private float infectionRadiusGrowthTimer;
  private float perlinCoordinate;
  private float recoveryTimer;
  private Collider2D[] nearbyPeople;
  private SpriteRenderer infectionRadiusRenderer;

  // **************************** //
  // ***** Helper variables ***** //
  // **************************** //

  private SpriteRenderer sr;
  private CircleCollider2D cc2D;

  // *************************** //
  // ***** Unity functions ***** //
  // *************************** //

  void Awake()
  {
    // Initialize component helper variables
    sr = GetComponent<SpriteRenderer>();
    cc2D = GetComponent<CircleCollider2D>();

    // Initialize start values
    infectionStatus = InfectionStatus.HEALTHY;
    direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    infectionRadius = cc2D.radius;
    perlinCoordinate = Random.Range(0, 1000);
    // For efficiency, a person can infect up to 5 other nearby people at a time
    nearbyPeople = new Collider2D[5];

    infectionRadiusRenderer = Instantiate(infectionRadiusPrefab, transform.position, Quaternion.identity, transform);
  }

  void Update()
  {
    Move();
    CollideWithScreenEdges();

    if (infectionStatus == InfectionStatus.INFECTED)
    {
      UpdateInfectionRadius();
      InfectNearbyPeople();
      UpdateRecovery();
    }
  }

  void OnCollisionEnter2D(Collision2D collision)
  {
    Vector3 dirToOther = collision.transform.position - transform.position;

    // If we just ran into the other person
    if (Vector3.Dot(direction, dirToOther) > 0)
    {
      Vector3 normal = collision.GetContact(0).normal;
      direction = Vector3.Reflect(direction, normal);
    }
  }

  // **************************** //
  // ***** Public functions ***** //
  // **************************** //

  public void SetInfectionStatus(InfectionStatus newStatus)
  {
    InfectionStatus previousStatus = infectionStatus;
    infectionStatus = newStatus;
    UpdateColor();
    UpdateInfectionRadiusVisibility();

    Controller.instance.UpdatePopulationBreakdown(previousStatus, newStatus);
  }

  public void UpdateInfectionRadiusVisibility()
  {
    infectionRadiusRenderer.enabled = Controller.instance.showInfectionRadius && infectionStatus == InfectionStatus.INFECTED;
  }

  // ***************************** //
  // ***** Private functions ***** //
  // ***************************** //

  private void Move()
  {
    // The value used to move through perlin coordinates - bigger is more chaotic
    perlinCoordinate += Time.deltaTime * Controller.timeScale;
    // Value between -1 and 1
    float perlinScale = (Mathf.PerlinNoise(perlinCoordinate, 0f) * 2) - 1;
    // The amount by which we should rotate the direction this frame
    float angleDelta = maxAngleDeltaPerSecond * perlinScale * Time.deltaTime * Controller.timeScale;
    // Rotate the direction
    direction = Quaternion.AngleAxis(angleDelta, Vector3.forward) * direction;
    // Move the person in the given direction
    transform.position += direction * movementSpeed * Time.deltaTime * Controller.timeScale;
  }

  private void UpdateInfectionRadius()
  {
    if (infectionRadius < maxInfectionRadius)
    {
      infectionRadiusGrowthTimer += Time.deltaTime * Controller.timeScale;
      float growthProgress = infectionRadiusGrowthTimer / infectionRadiusGrowthDuration;
      infectionRadius = Mathf.Lerp(cc2D.radius, maxInfectionRadius, growthProgress);
      infectionRadiusRenderer.size = Vector2.one * infectionRadius * 2;
    }
  }

  private void InfectNearbyPeople()
  {
    int numTargets = Physics2D.OverlapCircleNonAlloc(transform.position, infectionRadius, nearbyPeople);
    for (int i = 0; i < numTargets; i++)
    {
      GameObject target = nearbyPeople[i].gameObject;

      // Don't target yourself, dummy!
      if (target == gameObject)
      {
        continue;
      }

      // Check if the target is actually a Person
      Person targetPerson;
      if (target.TryGetComponent<Person>(out targetPerson))
      {
        // Only infect healthy people
        if (targetPerson.infectionStatus == InfectionStatus.HEALTHY)
        {
          targetPerson.SetInfectionStatus(InfectionStatus.INFECTED);
        }
      }
    }
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

  private void UpdateRecovery()
  {
    recoveryTimer += Time.deltaTime * Controller.timeScale;
    if (recoveryTimer >= recoveryDuration)
    {
      SetInfectionStatus(InfectionStatus.RECOVERED);
    }
  }

  private void UpdateColor()
  {
    if (infectionStatus == InfectionStatus.INFECTED)
    {
      sr.color = infectedColor;
    }
    else if (infectionStatus == InfectionStatus.RECOVERED)
    {
      sr.color = recoveredColor;
    }
  }
}


