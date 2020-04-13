using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
  public Bar infectedPeopleBar;

  void start()
  {
    SetNumInfectedPeople(0);
  }

  public void SetNumInfectedPeople(int value)
  {
    infectedPeopleBar.SetHeight(value);
  }

  public void SetWorldPopulation(int pop)
  {
    infectedPeopleBar.SetMaxValue(pop);
  }
}
