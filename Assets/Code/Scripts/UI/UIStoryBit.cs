using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIStoryBit : MonoBehaviour
{
    [Header("CONFIGURATION")]
    public TextMeshProUGUI text;
    public UIButtonController[] closePopup;
    public GameObject resizedPanel;
    public Image robotImage;

    [TextArea]
    public string story;

    [Header("ANIMATION")]
    [SerializeField] private float currentTime = 0f;
    [SerializeField] private bool finished = false;
    [SerializeField] private float timeToStartTextWriting = 0.9f;
    [SerializeField] float timeToEndTextWriting = 3f;
    [SerializeField] float textProgress = 0f;
    float startXScale = 0.2f;
    float startYScale = 0.0f;
    float timeToStartXScaling = 0.3f;
    float timeToEndXScaling = 0.8f;
    float timeToStartYScaling = 0.0f;
    float timeToEndYScaling = 0.3f;
    float timeToStartRobotAppearing = 0.8f;
    float timeToEndRobotAppearing = 1.2f;
    
    

    private bool soundToPlay = true;
    private string currentText = "";

    public void Show()
    {
        if (finished) return;

        gameObject.SetActive(true);
        resizedPanel.transform.localScale = new Vector3(0, 0, 1);

        foreach (var p in closePopup)
        {
            p.Init();
            p.onClick += StartHiding;
        }

        if (soundToPlay)
        {
            AudioManager.Instance.Play("robot");
            soundToPlay = false;
        }

        
    }

    private void Update()
    {
        if (finished)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                Hide();
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

            if(currentTime > timeToEndTextWriting || finished)
            {
                currentText = story;
                
            } 
            else
            {
                textProgress = Mathf.Clamp((currentTime - timeToStartTextWriting) / (timeToEndTextWriting - timeToStartTextWriting) , 0f, 1f);
                currentText = story.Substring(0, (int)(story.Length * textProgress));
            }
            text.text = currentText;
        }

    }

    private float Sigmoid(float value)
    {
        value = (value - 0.5f) * 12f;
        return 1.0f / (1.0f + Mathf.Exp(-value));
    }

    public void StartHiding()
    {
        finished = true;
        timeToStartRobotAppearing = 0.8f;
        timeToEndRobotAppearing = 1.0f;
        currentTime = Mathf.Min(1.0f, currentTime);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
