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

  // ***************************** //
  // ***** Private variables ***** //
  // ***************************** //

  private List<Person> people;

  int numPeople;

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
    ClearPeople();
    SpawnPeople();
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

  // ***************************** //
  // ***** Private functions ***** //
  // ***************************** //

  private void SpawnPeople()
  {
    Debug.Log(numPeople);


    // Get the edges of the camera viewport in world coordinates
    float topEdge = Camera.main.ViewportToWorldPoint(Vector3.up).y;
    float bottomEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
    float leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
    float rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right).x;

    // our algorithm relies on these values, but the first one will eventually become a user-input one.
    float squareSideLength = 2;
    float circleRadius = 1;

    // get the width and height of the screen
    float screenWidth = rightEdge - leftEdge;
    float screenHeight = topEdge - bottomEdge;

    // get the number of squares we can fit on the screen
    int gridWidth = (int)(screenWidth / squareSideLength);
    int gridHeight = (int)(screenHeight / squareSideLength);

    // get the extra space we have outside of our grid
    float horizontalExtraSpace = (screenWidth % squareSideLength);
    float verticalExtraSpace = (screenHeight % squareSideLength);

    // get the amount of extra space to add between each square
    float horizontalGap = horizontalExtraSpace / (gridWidth - 1);
    float verticalGap = verticalExtraSpace / (gridHeight - 1);

    for (float i = leftEdge; i < rightEdge; i += (squareSideLength + horizontalGap))
    {
      for (float j = topEdge; j > bottomEdge; j -= (squareSideLength + verticalGap))
      {
        float x = i + Random.Range(circleRadius, squareSideLength - circleRadius);
        float y = j - Random.Range(circleRadius, squareSideLength - circleRadius);

        Person person = Instantiate(personPrefab, new Vector3(x, y), Quaternion.identity);
        people.Add(person);
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
}
