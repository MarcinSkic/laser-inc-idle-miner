using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDetector : MonoBehaviour
{
    public Camera thisCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = thisCamera.ScreenPointToRay(Input.mousePosition);
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.TryGetComponent<BasicBlock>(out var block))
                    {
                        block.TakeDamage(SettingsModel.Instance.clickDamage);
                    }
                    else if (hit.collider.gameObject.TryGetComponent<RewardBat>(out var bat))
                    {
                        bat.getClicked();
                    }
                }
            }
            
        }
    }
}
