using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFovEnforcer : MonoBehaviour
{
    [SerializeField] Camera this_camera;
    [SerializeField] float target_fov;
    public static float calculatedFov;

    [Header("a")]
    [SerializeField] float a;
    [SerializeField] float b;

    // Update is called once per frame
    void Update()
    {
        SetFOV(target_fov);
        SetCameraOffset();
    }

    private void SetFOV(float f)
    {
        calculatedFov = 2 * Mathf.Atan(Mathf.Tan(f * Mathf.Deg2Rad * 0.5f) / this_camera.aspect) * Mathf.Rad2Deg;
        this_camera.fieldOfView = calculatedFov;
       
    }

    private void SetCameraOffset()
    {
        float y = a*(0.5f / this_camera.aspect) * 16f - b;
        this_camera.transform.localPosition = new Vector3(this_camera.transform.localPosition.x, -y, this_camera.transform.localPosition.z);
    }
}