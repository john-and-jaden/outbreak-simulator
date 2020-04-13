using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
  public Slider slider;

  public void SetHeight(int height)
  {
    slider.value = height;
  }

  public void SetMaxValue(int max)
  {
    slider.maxValue = max;
  }
}
