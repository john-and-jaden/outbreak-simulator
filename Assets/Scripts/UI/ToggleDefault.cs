using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleDefault : MonoBehaviour
{
    void Start()
    {
        // Unity is stupid so force this thing to update on startup manually
        Toggle toggle = GetComponent<Toggle>();
        bool temp = toggle.isOn;
        toggle.isOn = false;
        toggle.isOn = true;
        toggle.isOn = temp;
    }
}
