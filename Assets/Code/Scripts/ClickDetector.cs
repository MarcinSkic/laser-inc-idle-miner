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

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.TryGetComponent<BasicBlock>(out var block))
                {
                    block.TakeDamage(SettingsModel.Instance.clickDamage);
                    AudioManager.Instance.Play("block_hit");
                }
                else if (hit.collider.gameObject.TryGetComponent<RewardBat>(out var bat))
                {
                    bat.OnClicked();
                }
            }

        }
    }
}
