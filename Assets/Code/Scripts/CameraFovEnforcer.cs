using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFovEnforcer : MonoBehaviour
{
    [SerializeField] Camera this_camera;
    [SerializeField] Camera[] other_cameras;
    [SerializeField] float target_fov;
    public static float calculatedFov;
    public float previous_aspect;

    [Header("a")]
    [SerializeField] float a;
    [SerializeField] float b;

    public GameController gameController;

    private void Start()
    {
        previous_aspect = this_camera.aspect;
        SetFOV(target_fov);
        SetCameraOffset();
        gameController.RedrawDepthMeter();
    }

    void Update()
    {
        // check if recalculation needed
        if (this_camera.aspect != previous_aspect)
        {
            SetFOV(target_fov);
            SetCameraOffset();
            previous_aspect = this_camera.aspect;
            gameController.RedrawDepthMeter();
        }
    }

    private void SetFOV(float f)
    {
        calculatedFov = 2 * Mathf.Atan(Mathf.Tan(f * Mathf.Deg2Rad * 0.5f) / this_camera.aspect) * Mathf.Rad2Deg;
        this_camera.fieldOfView = calculatedFov;
       for (int i=0; i<other_cameras.Length; i++)
        {
            other_cameras[i].fieldOfView = calculatedFov;
        }
    }

    private void SetCameraOffset()
    {
        float y = a*(0.5f / this_camera.aspect) * 16f - b;
        this_camera.transform.localPosition = new Vector3(this_camera.transform.localPosition.x, -y, this_camera.transform.localPosition.z);
        for (int i = 0; i < other_cameras.Length; i++)
        {
            other_cameras[i].transform.localPosition = new Vector3(other_cameras[i].transform.localPosition.x, -y, other_cameras[i].transform.localPosition.z);
        }
    }
}
