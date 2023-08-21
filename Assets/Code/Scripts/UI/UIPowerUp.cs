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

    private string GetFormattedTime()
    {
        var leftTime = resourcesModel.powerUpTimeLeft;
        return string.Format(leftTime >= 3600 ? "{0:D2}:{1:D2}:{2:D2}" : "{1:D2}:{2:D2}",(int)leftTime/3600,((int)leftTime/60)%60,(int)leftTime%60);
    }

    private IEnumerator Timer()
    {
        yield return new WaitForEndOfFrame();

        while (resourcesModel.powerUpTimeLeft > 0f)
        {
            text.text = GetFormattedTime();

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
