using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopulationDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  public Image healthyBar;
  public Image infectedBar;
  public Image recoveredBar;
  [Tooltip("The interval at which the population display will refresh.")]
  public float refreshRate = 1f;

  private bool isDisplayActive;
  private Animator animator;

  void Awake()
  {
    animator = GetComponent<Animator>();
  }

  void Start()
  {
    isDisplayActive = true;
    StartCoroutine(RefreshDisplay());
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    Debug.Log("yeet");
    animator.SetBool("IsActive", true);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    animator.SetBool("IsActive", false);
  }

  private IEnumerator RefreshDisplay()
  {
    while (isDisplayActive)
    {
      // Get the status rates [0, 1.0] for each infection status
      Dictionary<InfectionStatus, float> statusRates = Controller.instance.GetPopulationStatusRates();

      // Get the status rates, or 0 if no people with the given status were found
      float infectedRate = 0;
      statusRates.TryGetValue(InfectionStatus.Infected, out infectedRate);
      float healthyRate = 0;
      statusRates.TryGetValue(InfectionStatus.Healthy, out healthyRate);
      float recoveredRate = 0;
      statusRates.TryGetValue(InfectionStatus.Recovered, out recoveredRate);

      // Update the bars to fill up their respective areas corresponding to their rates
      UpdateStatusBar(infectedBar, 0, infectedRate);
      UpdateStatusBar(healthyBar, infectedRate, healthyRate);
      UpdateStatusBar(recoveredBar, infectedRate + healthyRate, recoveredRate);

      // Wait for refreshRate seconds before updating again
      yield return new WaitForSeconds(refreshRate);
    }
  }

  private void UpdateStatusBar(Image bar, float startPoint, float proportion)
  {
    bar.rectTransform.anchorMin = new Vector2(0, startPoint);
    bar.rectTransform.anchorMax = new Vector2(1, startPoint + proportion);
  }
}
