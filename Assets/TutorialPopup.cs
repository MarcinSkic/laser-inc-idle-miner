using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    public Image panel;
    public Image robotImage;
    public TMP_Text text;
    public Image arrow;
    public GameObject resizedPanel;

    public float currentTime = 0f;
    float startXScale = 0.2f;
    float startYScale = 0.0f;
    float timeToStartXScaling= 0.3f;
    float timeToEndXScaling = 0.8f;
    float timeToStartYScaling = 0.0f;
    float timeToEndYScaling = 0.3f;
    float timeToStartArrowAppearing = 1.4f;
    float timeToEndArrowAppearing = 1.8f;
    float timeToStartRobotAppearing = 0.8f;
    float timeToEndRobotAppearing = 1.2f;
    float arrowMovementPeriod = 2.6f;
    float arrowMovementAmplitude = 80;
    bool turningOff = false;
    float arrowCycleOffset = 0f;

    float Sigmoid(float value)
    {
        value = (value - 0.5f)*12f;
        return 1.0f / (1.0f + Mathf.Exp(-value));
    }

    private void OnDisable()
    {
        if (turningOff)
        {
            currentTime = -1;
            resizedPanel.transform.localScale = new Vector3(0, 0, 1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        resizedPanel.transform.localScale = new Vector3(0, 0, 1);
        arrow.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (turningOff)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                resizedPanel.SetActive(false);
            }
        }
        else
        {
            currentTime += Time.deltaTime;
        }

        if (currentTime >= 0)
        {
            float xScale;
            if (currentTime > timeToEndXScaling)
            {
                xScale = 1;
            }
            else
            {
                xScale = startXScale + Mathf.Max(0, Sigmoid((currentTime - timeToStartXScaling) / (timeToEndXScaling - timeToStartXScaling)) * (1f - startXScale));
            }
            float yScale;
            if (currentTime > timeToEndYScaling)
            {
                yScale = 1;
            }
            else
            {
                yScale = startYScale + Mathf.Max(0, Sigmoid((currentTime - timeToStartYScaling) / (timeToEndYScaling - timeToStartYScaling)) * (1f - startYScale));
            }
            resizedPanel.transform.localScale = new Vector3(xScale, yScale, 1);

            if (currentTime > timeToEndRobotAppearing)
            {
                robotImage.color = new Color(1, 1, 1, 1);
            }
            else
            {
                robotImage.color = new Color(1, 1, 1, Mathf.Max(0, (currentTime - timeToStartRobotAppearing) / (timeToEndRobotAppearing - timeToStartRobotAppearing)));
            }

            if (arrow.IsActive())
            {
                if (currentTime > timeToEndArrowAppearing)
                {
                    arrow.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    arrow.color = new Color(1, 1, 1, Mathf.Max(0, (currentTime - timeToStartArrowAppearing) / (timeToEndArrowAppearing - timeToStartArrowAppearing)));
                }
                arrow.transform.localPosition = new Vector3(Mathf.Sin((currentTime+arrowCycleOffset) * 6.28f / arrowMovementPeriod) * arrowMovementAmplitude, arrow.transform.localPosition.y, arrow.transform.localPosition.z);

            }
        }
    }

    public void StartFinishingSequence()
    {
        turningOff = true;
        timeToStartRobotAppearing = 0.8f;
        timeToEndRobotAppearing = 1.0f;
        timeToStartArrowAppearing = 0.8f;
        timeToEndArrowAppearing = 1.0f;
        float timeToSet = Mathf.Min(1.0f, currentTime);
        arrowCycleOffset = currentTime - timeToSet;
        currentTime = timeToSet;
    }
}
