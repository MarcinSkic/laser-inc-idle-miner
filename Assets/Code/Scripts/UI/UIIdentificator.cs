using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIdentificator : MonoBehaviour
{
    public string parameter;

    private void Start()
    {
        Debug.Log(parameter+ " " + this.name);
    }
}
