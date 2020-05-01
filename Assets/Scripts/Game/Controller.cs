using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
  // **************************** //
  // ***** Static variables ***** //
  // **************************** //

  public static Controller instance;
  public static float timeScale = 1f;
  public static bool showInfectionRadius = false;
  public static float maxInfectionRadius = 2f;
  public static float infectionRadiusGrowthTime = 1f;
  public static float recoveryTime = 5f;

  // **************************** //
  // ***** Public variables ***** //
  // **************************** //

  [Tooltip("Prefab of the person object to spawn.")]
  public Person personPrefab;

  [Tooltip("The statistics graph UI object.")]
  public Graph graph;

  // ***************************** //
  // ***** Private variables ***** //
  // ***************************** //

  private List<Person> people;

  private int numPeople;
  private float percentInitiallyInfected;

  private Dictionary<int, int> populationHealthBreakdown;

  // *************************** //
  // ***** Unity functions ***** //
  // *************************** //

  void Awake()
  {
    if (instance != null)
    {
      Destroy(instance.gameObject);
    }
    instance = this;
    instance.numPeople = 1;
  }

  void Start()
  {
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
    Controller.timeScale = timeScale;
  }

  public void SetNumPeople(float numPeople)
  {
    this.numPeople = (int)numPeople;
  }

  public void SetPercentInitiallyInfected(float percentInitiallyInfected)
  {
    this.percentInitiallyInfected = percentInitiallyInfected;
  }

  public void SetShowInfectionRadius(bool showInfectionRadius)
  {
    Controller.showInfectionRadius = showInfectionRadius;
    foreach (Person person in people)
    {
      person.UpdateInfectionRadiusVisibility();
    }
  }

  public void SetMaxInfectionRadius(float maxInfectionRadius)
  {
    Controller.maxInfectionRadius = maxInfectionRadius;
  }

  public void SetInfectionRadiusGrowthTime(float infectionRadiusGrowthTime)
  {
    Controller.infectionRadiusGrowthTime = infectionRadiusGrowthTime;
  }

  public void SetRecoveryTime(float recoveryTime)
  {
    Controller.recoveryTime = recoveryTime;
  }

  public void UpdatePopulationBreakdown(InfectionStatus previousStatus, InfectionStatus newStatus)
  {
    populationHealthBreakdown[(int)previousStatus] = populationHealthBreakdown[(int)previousStatus] + -1;
    populationHealthBreakdown[(int)newStatus] = populationHealthBreakdown[(int)newStatus] + 1;

    // graph.SetNumHealthyPeople(populationHealthBreakdown[(int)InfectionStatus.HEALTHY]);
    graph.SetNumInfectedPeople(populationHealthBreakdown[(int)InfectionStatus.Infected]);
    graph.SetNumRecoveredPeople(populationHealthBreakdown[(int)InfectionStatus.Recovered]);
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
    Debug.Log(percentInitiallyInfected);
    int infectedCount = Mathf.CeilToInt(percentInitiallyInfected * numPeople);
    for (int i = 0; i < infectedCount; i++)
    {
      people[i].SetInfectionStatus(InfectionStatus.Infected);
    }
  }

  private void InitializePopulationBreakdown()
  {
    int infectedCount = Mathf.CeilToInt(percentInitiallyInfected * numPeople);
    populationHealthBreakdown = new Dictionary<int, int>();
    populationHealthBreakdown.Add((int)InfectionStatus.Healthy, numPeople - infectedCount);
    populationHealthBreakdown.Add((int)InfectionStatus.Infected, infectedCount);
    populationHealthBreakdown.Add((int)InfectionStatus.Recovered, 0);
  }

  private void StartGraph()
  {
    int infectedCount = Mathf.CeilToInt(percentInitiallyInfected * numPeople);
    graph.SetNumPeople(numPeople);
    graph.SetNumHealthyPeople(numPeople - infectedCount);
    graph.SetNumInfectedPeople(infectedCount);
    graph.SetNumRecoveredPeople(0);
  }
}
