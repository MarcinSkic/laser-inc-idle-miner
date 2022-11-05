using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Splashtext : MonoBehaviour
{
    [SerializeField] private TMP_Text splashdisplay;
    [SerializeField] private TMP_Text splashshadow;
    public double scale;
    public double change;

    string[] splashtexts = { "Minecraft's better", 
        "Also try Idle Breakout!",
        //2021 == bin 11111100101
        //02 == bin 10
        //12 == bin 1100
        //"Since 11111100101-10-1100!",
        //"Thanks, Marcin!",
        "This is main menu,\nif you haven't noticed"
    };



    void Start()
    {
        int chosen = Random.Range(0, splashtexts.Length);
        splashdisplay.text = splashtexts[chosen];
        splashshadow.text = splashtexts[chosen];
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
            scale *= System.Math.Pow(1.4, Time.deltaTime);
        }
        else if (change == -1)
        {
            scale /= System.Math.Pow(1.4, Time.deltaTime);
        }
        
        //scale = Random.Range(0.5f, 2f);
        gameObject.transform.localScale = new Vector3 ((float)scale, (float)scale, (float)scale);
    }
}
