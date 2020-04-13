using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
  public Bar healthyPeopleBar;
  public Bar infectedPeopleBar;
  public Bar recoveredPeopleBar;

  private int numPeople;

  public void SetNumPeople(int pop)
  {
    numPeople = pop;
    healthyPeopleBar.SetMaxValue(pop);
    infectedPeopleBar.SetMaxValue(pop);
    recoveredPeopleBar.SetMaxValue(pop);
  }

  public void SetNumHealthyPeople(int value)
  {
    healthyPeopleBar.SetHeight(value);
  }
  public void SetNumInfectedPeople(int value)
  {
    infectedPeopleBar.SetHeight(value);
  }
  public void SetNumRecoveredPeople(int value)
  {
    recoveredPeopleBar.SetHeight(value);
  }
}
