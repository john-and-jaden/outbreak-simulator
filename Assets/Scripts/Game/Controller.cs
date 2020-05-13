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

  // ***************************** //
  // ***** Private variables ***** //
  // ***************************** //

  private List<Person> people;

  private int numPeople;
  private float percentInitiallyInfected;
  private float percentSocialDistancing;

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

    people = new List<Person>();
  }

  // **************************** //
  // ***** Public functions ***** //
  // **************************** //

  public void StartSimulation()
  {
    ClearPeople();
    SpawnPeople();
    InfectInitialPatients();
    SociallyDistancePeople();
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

  public void SetPercentSocialDistancing(float percentSocialDistancing)
  {
    this.percentSocialDistancing = percentSocialDistancing;
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

  public Dictionary<InfectionStatus, float> GetPopulationStatusRates()
  {
    Dictionary<InfectionStatus, float> statusRates = new Dictionary<InfectionStatus, float>();
    foreach (Person person in people)
    {
      InfectionStatus status = person.GetInfectionStatus();
      if (!statusRates.ContainsKey(status))
      {
        statusRates.Add(status, 0);
      }
      statusRates[status] += 1.0f / people.Count;
    }
    return statusRates;
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
    int infectedCount = Mathf.CeilToInt(percentInitiallyInfected * numPeople);
    
    bool[] isInfected = new bool[numPeople];
    while (infectedCount > 0)
    {
      int i = Random.Range(0, numPeople);
      if (!isInfected[i])
      {
        isInfected[i] = true;
        people[i].SetInfectionStatus(InfectionStatus.Infected);
        infectedCount--;
      }
    }
  }

  private void SociallyDistancePeople()
  {
    int distancingCount = Mathf.CeilToInt(percentSocialDistancing * numPeople);

    bool[] isDistancing = new bool[numPeople];
    while (distancingCount > 0)
    {
      int i = Random.Range(0, numPeople);
      if (!isDistancing[i])
      {
        isDistancing[i] = true;
        people[i].SetSociallyDistancing();
        distancingCount--;
      }
    }
  }
}
