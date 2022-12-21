using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFovEnforcer : MonoBehaviour
{
    [SerializeField] Camera this_camera;
    [SerializeField] float target_fov;
    [SerializeField] GameObject cameraParent;
    public static float calculatedFov;

    [Header("a")]
    [SerializeField] float a;
    [SerializeField] float b;

    // Update is called once per frame
    void Update()
    {
        pSetFOV(target_fov);
        SetCameraOffset();
    }

    private void pSetFOV(float f)
    {
        calculatedFov = 2 * Mathf.Atan(Mathf.Tan(f * Mathf.Deg2Rad * 0.5f) / this_camera.aspect) * Mathf.Rad2Deg;
        this_camera.fieldOfView = calculatedFov;
       
    }

    private void SetCameraOffset()
    {
        float x = a*(0.5f / this_camera.aspect) * 16f - b;
        this_camera.transform.localPosition = new Vector3(0, -x, 0);
    }
}
