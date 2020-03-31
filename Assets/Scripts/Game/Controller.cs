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
    int sideLength = (int)Mathf.Sqrt(numPeople);
    for (int i = 0; i < sideLength; i++)
    {
      for (int j = 0; j < sideLength; j++)
      {
        float x = i * spawnGapDistance - sideLength * spawnGapDistance / 2;
        float y = j * spawnGapDistance - sideLength * spawnGapDistance / 2;
        Person person = Instantiate(personPrefab, new Vector3(x, y), Quaternion.identity);
        people.Add(person);
      }
    }

    InfectInitialPatients();
  }

  void Update()
  {

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
