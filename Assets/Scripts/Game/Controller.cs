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

  [Header("Defaults")]

  [Tooltip("Default setting for the timescale of the simulation.")]
  public int defaultTimeScale;

  [Tooltip("Default setting for the total number of people.")]
  public float defaultNumPeople;

  [Tooltip("Default setting for the initial number of infected people.")]
  public float defaultPercentInfected;

  [Tooltip("Default setting for the visibility of people's infection radii.")]
  public bool defaultShowInfectionRadius;

  [Tooltip("Default setting for the max infection radius.")]
  public float defaultMaxInfectionRadius;

  [Tooltip("Default setting for the time it takes for a person to become fully contagious.")]
  public float defaultInfectionRadiusGrowthTime;

  [Tooltip("Default setting for the time it takes for an infected person to recover.")]
  public float defaultRecoveryTime;

  [Space]

  [Tooltip("Prefab of the person object to spawn.")]
  public Person personPrefab;

  [Tooltip("The status bar UI object.")]
  public Graph graph;

  [Tooltip("The infection curve UI object.")]
  public InfectionCurve infectionCurve;

  // ***************************** //
  // ***** Private variables ***** //
  // ***************************** //

  private List<Person> people;

  private int numPeople;

  private Dictionary<int, int> populationHealthBreakdown;

  public int infectionCurveTimeInterval;
  public float infectionCurveTimer;

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

  void Update()
  {
    infectionCurveTimer += Time.deltaTime;

    if (infectionCurveTimer >= infectionCurveTimeInterval)
    {
      infectionCurveTimer = 0;
      infectionCurve.AddPointToGraph(populationHealthBreakdown[(int)InfectionStatus.INFECTED] / numPeople * 100);
    }
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
    infectionCurveTimeInterval = 1;
  }

  public void SetTimescale(float timeScale)
  {
    Controller.timeScale = timeScale;
  }

  public void SetShowInfectionRadius(bool showInfectionRadius)
  {
    Controller.showInfectionRadius = showInfectionRadius;
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

    // graph.SetNumHealthyPeople(populationHealthBreakdown[(int)InfectionStatus.HEALTHY]);
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
    int infectedCount = Mathf.CeilToInt(defaultPercentInfected * numPeople);
    for (int i = 0; i < infectedCount; i++)
    {
      people[i].SetInfectionStatus(InfectionStatus.INFECTED);
    }
  }

  private void InitializePopulationBreakdown()
  {
    int infectedCount = Mathf.CeilToInt(defaultPercentInfected * numPeople);
    populationHealthBreakdown = new Dictionary<int, int>();
    populationHealthBreakdown.Add((int)InfectionStatus.HEALTHY, numPeople - infectedCount);
    populationHealthBreakdown.Add((int)InfectionStatus.INFECTED, infectedCount);
    populationHealthBreakdown.Add((int)InfectionStatus.RECOVERED, 0);
  }

  private void StartGraph()
  {
    int infectedCount = Mathf.CeilToInt(defaultPercentInfected * numPeople);
    graph.SetNumPeople(numPeople);
    graph.SetNumHealthyPeople(numPeople - infectedCount);
    graph.SetNumInfectedPeople(infectedCount);
    graph.SetNumRecoveredPeople(0);
  }
}
