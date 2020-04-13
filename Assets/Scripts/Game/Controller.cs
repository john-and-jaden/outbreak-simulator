using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
  // **************************** //
  // ***** Static variables ***** //
  // **************************** //

  public static Controller instance;

  // **************************** //
  // ***** Public variables ***** //
  // **************************** //

  [Tooltip("Distance between each person when spawned in a grid.")]
  public float spawnGapDistance;

  [Tooltip("Prefab of the person object to spawn.")]
  public Person personPrefab;

  [Tooltip("Initial number of infected people.")]
  public int initialNumberOfCases;

  [Tooltip("Whether or not to display infected people's infection radiuses.")]
  public bool showInfectionRadius;

  [Tooltip("The statistics graph")]
  public Graph graph;

  // ***************************** //
  // ***** Private variables ***** //
  // ***************************** //

  private List<Person> people;

  int numPeople;

  Dictionary<int, int> populationHealthBreakdown;

  // *************************** //
  // ***** Unity functions ***** //
  // *************************** //

  void Start()
  {
    if (instance != null)
    {
      Destroy(instance.gameObject);
    }
    instance = this;
    instance.numPeople = 1;

    ClearPeople();
  }

  // **************************** //
  // ***** Public functions ***** //
  // **************************** //

  public void StartSimulation()
  {
    InitializePopulationBreakdown();
    ClearPeople();
    SpawnPeople();
    StartGraph();
    InfectInitialPatients();
  }

  public void SetTimescale(float timeScale)
  {
    Time.timeScale = timeScale;
  }

  public void SetShowInfectionRadius(bool showInfectionRadius)
  {
    this.showInfectionRadius = showInfectionRadius;
    foreach (Person person in people)
    {
      person.UpdateInfectionRadiusVisibility();
    }
  }

  public void SetNumPeople(float numPeople)
  {
    this.numPeople = (int)numPeople;
  }

  public void UpdatePopulationBreakdown(InfectionStatus previousStatus, InfectionStatus newStatus)
  {
    populationHealthBreakdown[(int)previousStatus] = populationHealthBreakdown[(int)previousStatus] + -1;
    populationHealthBreakdown[(int)newStatus] = populationHealthBreakdown[(int)newStatus] + 1;

    graph.SetNumHealthyPeople(populationHealthBreakdown[(int)InfectionStatus.HEALTHY]);
    graph.SetNumInfectedPeople(populationHealthBreakdown[(int)InfectionStatus.INFECTED]);
    graph.SetNumRecoveredPeople(populationHealthBreakdown[(int)InfectionStatus.RECOVERED]);
  }

  // ***************************** //
  // ***** Private functions ***** //
  // ***************************** //

  private void SpawnPeople()
  {
    // Get the edges of the camera viewport in world coordinates
    float topEdge = Camera.main.ViewportToWorldPoint(Vector3.up).y;
    float bottomEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
    float leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
    float rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right).x;

    // define the radius of a person
    float circleRadius = 1;

    // spray and pray
    while (people.Count < numPeople)
    {
      float x = Random.Range(leftEdge, rightEdge);
      float y = Random.Range(topEdge, bottomEdge);

      bool positionValid = true;

      for (int i = 0; i < people.Count; i++)
      {
        if (Vector2.Distance(people[i].transform.position, new Vector2(x, y)) < 2 * circleRadius)
        {
          positionValid = false;
          break;
        }
      }

      if (positionValid)
      {
        people.Add(Instantiate(personPrefab, new Vector3(x, y), Quaternion.identity));
      }
    }
  }

  private void ClearPeople()
  {
    if (people != null)
    {
      foreach (Person person in people)
      {
        Destroy(person.gameObject);
      }
    }

    people = new List<Person>();
  }

  private void InfectInitialPatients()
  {
    for (int i = 0; i < initialNumberOfCases; i++)
    {
      people[i].SetInfectionStatus(InfectionStatus.INFECTED);
    }
  }

  private void InitializePopulationBreakdown()
  {
    populationHealthBreakdown = new Dictionary<int, int>();
    populationHealthBreakdown.Add((int)InfectionStatus.HEALTHY, numPeople - initialNumberOfCases);
    populationHealthBreakdown.Add((int)InfectionStatus.INFECTED, initialNumberOfCases);
    populationHealthBreakdown.Add((int)InfectionStatus.RECOVERED, 0);
  }

  private void StartGraph()
  {
    graph.SetNumPeople(numPeople);
    graph.SetNumHealthyPeople(numPeople - initialNumberOfCases);
    graph.SetNumInfectedPeople(initialNumberOfCases);
    graph.SetNumRecoveredPeople(0);
  }
}
