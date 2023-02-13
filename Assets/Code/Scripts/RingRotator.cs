using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingRotator : MonoBehaviour
{
    public BlocksModel blocksModel;

    public GameObject[] rings;
    public Light[] lights;

    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private AnimationCurve intensityCurve;

    [SerializeField] [ReadOnly] private float targetSpeed = 0f;
    [SerializeField] private float deccelerationPerSecond = 2f;

    void Update()
    {
        targetSpeed = Mathf.Max(blocksModel.currentSpeed, targetSpeed - deccelerationPerSecond * Time.deltaTime);

        var rotation = rotationCurve.Evaluate(targetSpeed);
        var intensity = intensityCurve.Evaluate(targetSpeed);

        foreach (GameObject ring in rings)
        {
            ring.transform.Rotate(Vector3.up, rotation * Time.deltaTime);
        }
        foreach (Light light in lights)
        {
            light.intensity = intensity;
        }
    }
}
