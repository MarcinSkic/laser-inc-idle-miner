using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPowerUp : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text text;
    [SerializeField] private ResourcesModel resourcesModel;

    public void StartTimer()
    {
        gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForEndOfFrame();

        while (resourcesModel.powerUpTimeLeft > 0f)
        {
            text.text = NumberFormatter.FormatSecondsToReadableShort(resourcesModel.powerUpTimeLeft);

            yield return new WaitForSeconds(1f);
        }

        gameObject.SetActive(false);
    }

    public void SetValueDirectly(string value)
    {
        gameObject.SetActive(true);

        text.text = value;
    }

    public void DisableDirectly()
    {
        gameObject.SetActive(false);
    }
}
