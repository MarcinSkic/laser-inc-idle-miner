using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingRotator : MonoBehaviour
{
    public BlocksManager blocksManager;

    [Header("Ring rotation")]
    public float MinDegsPerSecond;
    public float DegsPerSecondPerMovement;
    public float MaxDegsPerSecond;
    public float rotationChangeSofteningRatio;
    public float lastRotation;
    public GameObject[] rings;

    [Header("Light intensity")]
    public float MinLightIntensity;
    public float LightIntensityPerMovement;
    public float MaxLightIntensity;
    public float lightIntensityChangeSofteningRatio;
    public float lastLightIntensity;
    public Light[] lights;

    void Update()
    {
        float targetRotation = DegsPerSecondPerMovement * blocksManager.blockMovementsInARow;
        targetRotation = Mathf.Max(MinDegsPerSecond, Mathf.Min(targetRotation, MaxDegsPerSecond));
        float rotation = lastRotation * rotationChangeSofteningRatio + targetRotation * (1f- rotationChangeSofteningRatio);
        lastRotation = rotation;
        foreach (GameObject ring in rings)
        {
            ring.transform.Rotate(Vector3.up, rotation * Time.deltaTime);
        }

        float targetIntensity = LightIntensityPerMovement * blocksManager.blockMovementsInARow;
        targetIntensity = Mathf.Max(MinLightIntensity, Mathf.Min(targetIntensity, MaxLightIntensity));
        float intensity = lastLightIntensity * lightIntensityChangeSofteningRatio + targetIntensity * (1f - lightIntensityChangeSofteningRatio);
        lastLightIntensity = intensity;
        foreach (Light light in lights)
        {
            light.intensity=intensity;
        }
    }
}
