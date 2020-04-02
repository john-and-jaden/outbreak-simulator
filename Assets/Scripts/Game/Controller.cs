using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
  // **************************** //
  // ***** Public variables ***** //
  // **************************** //

  [Tooltip("Number of people to be spawned.")]
  public int numPeople;

  [Tooltip("Distance between each person when spawned in a grid.")]
  public float spawnGapDistance;

  [Tooltip("Prefab of the person object to spawn.")]
  public Person personPrefab;

  [Tooltip("Initial number of infected people.")]
  public int initialNumberOfCases;

  [Tooltip("Whether or not to display infected people's infection radiuses.")]
  public bool showInfectionRadius;

  [Tooltip("The scale at which time passes.")]
  public float timeScale;
  // ***************************** //
  // ***** Private variables ***** //
  // ***************************** //

  private List<Person> people;

  // *************************** //
  // ***** Unity functions ***** //
  // *************************** //

  void Start()
  {
    people = new List<Person>();

    // Get the edges of the camera viewport in world coordinates
    float topEdge = Camera.main.ViewportToWorldPoint(Vector3.up).y;
    float bottomEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
    float leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
    float rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right).x;

    // Debug.Log(topEdge); // 20 
    // Debug.Log(bottomEdge); // -20 
    // Debug.Log(leftEdge); // 30
    // Debug.Log(rightEdge); // -30 

    float squareSideLength = 10;
    float circleRadius = 1;

    for (float i = leftEdge; i < rightEdge - (circleRadius * 2); i += squareSideLength)
    {
      for (float j = topEdge; j > bottomEdge + (circleRadius * 2); j -= squareSideLength)
      {
        float x = i + Random.Range(circleRadius, squareSideLength - circleRadius);
        float y = j - Random.Range(circleRadius, squareSideLength - circleRadius);

        Person person = Instantiate(personPrefab, new Vector3(x, y), Quaternion.identity);
        people.Add(person);
      }
    }

    InfectInitialPatients();
  }

  void Update()
  {
    // update whether or not to show people's infection radius
    foreach (Person person in people)
    {
      person.SetShowInfectionRadius(showInfectionRadius);
    }

    // update the time scale
    Time.timeScale = timeScale;
  }

  // ***************************** //
  // ***** Private functions ***** //
  // ***************************** //

  private void InfectInitialPatients()
  {
    for (int i = 0; i < initialNumberOfCases; i++)
    {
      people[i].SetInfectionStatus(InfectionStatus.INFECTED);
    }
  }
}
