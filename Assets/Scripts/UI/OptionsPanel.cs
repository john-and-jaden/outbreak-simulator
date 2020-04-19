using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  public Animator buttonAnimator;
  public Animator panelAnimator;

  private bool isOpen = false;

  public void OnPointerEnter(PointerEventData eventData)
  {
    if (!isOpen)
    {
      buttonAnimator.SetBool("IsExtended", true);
    }
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (!isOpen)
    {
      buttonAnimator.SetBool("IsExtended", false);
    }
  }

  public void TogglePanel()
  {
    isOpen = !isOpen;
    panelAnimator.SetBool("IsOpen", isOpen);
    buttonAnimator.SetBool("IsOpen", isOpen);
  }
}
