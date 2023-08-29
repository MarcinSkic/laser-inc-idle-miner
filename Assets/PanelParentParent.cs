using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelParentParent : MonoBehaviour
{
    [SerializeField] ResourcesModel resourcesModel;
    [SerializeField] GameObject panelParentPrefab;
    [SerializeField] GameController gameController;
    public float totalMoneyForNextPanel = 1e4f;
    public int currentlyBuiltRingI = -1;
    public int panelsInRow = 0;
    [SerializeField] float panelCostGrowth = 1.05f;
    public float degreesBetweenPanelsInRow = 3f;
    public Vector3 eulerBetweenRows = new Vector3(0f, 5f, 10f);
    public float panelParentScale = 1f;
    public float panelParentScaleGrowth = 1.05f;
    [SerializeField] float rotationPerSec;
    [SerializeField] PanelRing panelRingPrefab; 
    private PanelRing currentlyBuiltRing;
    public int panelsCount = 0;

    void Update()
    {
        while (resourcesModel.undecreasableEarnedMoney > totalMoneyForNextPanel && gameController.visitedDyson)
        {
            if (panelsInRow == 119 || currentlyBuiltRingI == -1)
            {
                panelsInRow = 0;
                currentlyBuiltRingI++;
                panelParentScale *= panelParentScaleGrowth;
                currentlyBuiltRing = Instantiate(panelRingPrefab, transform.position, Quaternion.identity, transform);
                currentlyBuiltRing.transform.localRotation = Quaternion.Euler(currentlyBuiltRingI * eulerBetweenRows);
                currentlyBuiltRing.rotationPerSec = rotationPerSec/(Mathf.Sqrt(panelParentScale*panelParentScale*panelParentScale));
            }
            totalMoneyForNextPanel *= panelCostGrowth;
            panelsInRow++;
            panelsCount++;
            var rotation = Quaternion.Euler(0, 0, 0);
            var newPanelParent = Instantiate(panelParentPrefab, currentlyBuiltRing.transform.position, rotation, currentlyBuiltRing.transform);
            newPanelParent.transform.localRotation = Quaternion.Euler(0, panelsInRow * degreesBetweenPanelsInRow, 0);
            newPanelParent.transform.localScale = new Vector3(panelParentScale, panelParentScale, panelParentScale);
        }
    }
}
