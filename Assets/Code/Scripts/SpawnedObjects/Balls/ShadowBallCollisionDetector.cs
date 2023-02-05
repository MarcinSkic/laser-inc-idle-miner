using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBallCollisionDetector : MonoBehaviour
{
    public void OnTriggerEnter(Collider collider)
    {
        gameObject.GetComponentInParent<ShadowBall>().handleDetectionFromTrigger(collider);
    }
}
