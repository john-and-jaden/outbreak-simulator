using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator panelAnimator;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("enter");
        panelAnimator.SetBool("IsOpen", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("exit");
        panelAnimator.SetBool("IsOpen", false);
    }
}
