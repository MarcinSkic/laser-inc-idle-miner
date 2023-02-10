using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBallCollisionDetector : MonoBehaviour
{
    [SerializeField] private ShadowBall ball;
    public void OnTriggerEnter(Collider collider)
    {
        ball.HandleDetectionFromTrigger(collider);
    }
}
