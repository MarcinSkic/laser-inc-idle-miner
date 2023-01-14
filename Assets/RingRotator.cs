using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingRotator : MonoBehaviour
{
    public float DegsPerSecond;

    void Update()
    {
        gameObject.transform.Rotate(Vector3.up, DegsPerSecond * Time.deltaTime);
    }
}
