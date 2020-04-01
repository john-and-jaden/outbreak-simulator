using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectionRadiusRenderer : MonoBehaviour
{
  // ***************************** //
  // ***** Private variables ***** //
  // ***************************** //
  private SpriteRenderer sr;

  private bool setShowInfectionRadius;

  // *************************** //
  // ***** Unity functions ***** //
  // *************************** //

  // Start is called before the first frame update
  void Start()
  {
    sr = GetComponent<SpriteRenderer>();
  }

  // Update is called once per frame
  void Update()
  {

  }

  // **************************** //
  // ***** Public functions ***** //
  // **************************** //
  public void setRadius(float radius)
  {
    sr.size = new Vector2(radius, radius);
  }
  public void SetShowInfectionRadius(bool showInfectionRadius)
  {
    sr.enabled = showInfectionRadius;
  }
}
