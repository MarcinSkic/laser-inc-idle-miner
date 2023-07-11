using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.Rotate(0, 0, -180*Time.deltaTime);
    }
}
