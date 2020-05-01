using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderDefault : MonoBehaviour
{
  void Start()
  {
    // Unity is stupid so force this thing to update on startup manually
    Slider slider = GetComponent<Slider>();
    float temp = slider.value;
    slider.value = slider.minValue;
    slider.value = slider.maxValue;
    slider.value = temp;
  }
}
