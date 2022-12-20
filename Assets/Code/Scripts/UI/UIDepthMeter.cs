using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDepthMeter : MonoBehaviour
{
    [SerializeField]
    private RawImage slidingImage;
    [SerializeField]
    private float depthToUVDivider;

    private Rect slidingImageUvRect;



    private void Awake()
    {
        slidingImageUvRect = slidingImage.uvRect;
    }

    public void SetDepth(double depth)
    {
        slidingImageUvRect.y = -(float)depth/depthToUVDivider;
        slidingImageUvRect.height = CameraFovEnforcer.calculatedFov/11f;
        slidingImage.uvRect = slidingImageUvRect;
    }
}
