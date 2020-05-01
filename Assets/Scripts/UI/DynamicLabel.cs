using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DynamicLabel : MonoBehaviour
{
  [Tooltip("If true, this label will display the current value multiplied by 100 and with a percentage sign.")]
  public bool showFloatAsPercentage = false;

  [Tooltip("Number of digits after the decimal point.")]
  public int numDecimalPlaces = 1;

  private Text textComponent;

  void Awake()
  {
    textComponent = GetComponent<Text>();
  }

  public void SetValue(float value)
  {
    // Set the display value based on whether percentage mode is set
    float displayValue = value * (showFloatAsPercentage ? 100 : 1);

    // "F2" for example is a decimal format with 2 decimal places
    string decimalFormat = "F" + numDecimalPlaces.ToString();

    // Set the Text component to the new value
    textComponent.text = displayValue.ToString(decimalFormat) + (showFloatAsPercentage ? "%" : "");
  }
}
