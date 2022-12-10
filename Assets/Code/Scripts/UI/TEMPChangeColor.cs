using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEMPChangeColor : MonoBehaviour
{
    public Image img;
    public Color TurnOnImage;
    public Color TurnOffImage;

    private bool turnedOn = true;
    public void ChangeColor()
    {
        if(turnedOn)
        {
            img.color = TurnOffImage;
            
        } 
        else
        {
            img.color = TurnOnImage;
        }
        turnedOn = !turnedOn;
    }
}
