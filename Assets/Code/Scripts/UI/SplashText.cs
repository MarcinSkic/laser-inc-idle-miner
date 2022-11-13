using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SplashText : MonoBehaviour
{
    [SerializeField] private TMP_Text display;
    [SerializeField] private TMP_Text shadow;
    private float scale = 1;
    private float change = 1;

    [SerializeField]
    string[] texts = { 
        "Minecraft's better",
        "Also try Idle Breakout!",
        "This is main menu,\nif you haven't noticed"
    };



    void Start()
    {
        int chosenIndex = Random.Range(0, texts.Length);
        display.text = texts[chosenIndex];
        shadow.text = texts[chosenIndex];
    }

    private void Update()
    {
        if (scale > 1.04f)
        {
            change = -1;
        }
        else if (scale < 0.96f)
        {
            change = 1;
        }

        if (change == 1)
        {
            scale *= (float)System.Math.Pow(1.4f, Time.deltaTime);
            
        }
        else if (change == -1)
        {
            scale /= (float)System.Math.Pow(1.4f, Time.deltaTime);
        }

        gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }
}