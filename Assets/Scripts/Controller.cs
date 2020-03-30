using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private List<Person> people;
    public int numPeople;
    public float distance;
    public Person personPrefab;
    public int initialNumberOfCases;

    // Start is called before the first frame update
    void Start()
    {
        people = new List<Person>();
        int sideLength = (int)Mathf.Sqrt(numPeople);
        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                float x = i * distance - sideLength * distance / 2;
                float y = j * distance - sideLength * distance / 2;
                Person person = Instantiate(personPrefab, new Vector3(x, y), Quaternion.identity);
                people.Add(person);
            }
        }
        
        infectInitialPatients();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void infectInitialPatients()
    {
        for (int i = 0; i < initialNumberOfCases; i++)
        {
            people[i].SetInfectionStatus(1);
        }
    }
}
