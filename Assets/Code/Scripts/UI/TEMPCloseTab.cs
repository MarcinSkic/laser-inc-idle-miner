using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMPCloseTab : MonoBehaviour
{
    public void Clicked()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
