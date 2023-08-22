using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelRing : MonoBehaviour
{
    public float rotationPerSec;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationPerSec * Time.deltaTime);
    }
}
