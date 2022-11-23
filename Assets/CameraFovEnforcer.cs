using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFovEnforcer : MonoBehaviour
{
    [SerializeField] Camera this_camera;
    [SerializeField] float target_fov;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pSetFOV(target_fov);
    }

    private void pSetFOV(float f)
    {
        f = 2 * Mathf.Atan(Mathf.Tan(f * Mathf.Deg2Rad * 0.5f) / this_camera.aspect) * Mathf.Rad2Deg;
        this_camera.fieldOfView = f;
    }
}
