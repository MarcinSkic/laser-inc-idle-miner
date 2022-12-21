using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDepthMeter : MonoBehaviour
{
    [SerializeField]
    private RawImage slidingImage;
    [SerializeField]
    List<TMP_Text> labels;
    [SerializeField]
    private float depthToUVDivider;
    [SerializeField]
    private int gapsBeetwenLabels = 5;
    [SerializeField]
    private float gapWidth;
    [SerializeField]
    private float labelsBottomPadding = 30;
    [SerializeField]
    private int labelsWithHigherThanCurrentDepth = 2;

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

        int labeledDepth = (int)depth-((int)depth)% gapsBeetwenLabels;
        for(int i = -labelsWithHigherThanCurrentDepth; i < labels.Count - labelsWithHigherThanCurrentDepth; i++)
        {
            labels[i+ labelsWithHigherThanCurrentDepth].rectTransform.anchoredPosition = new Vector2(0, labelsBottomPadding + (gapsBeetwenLabels * i * gapWidth) + ((float)depth - labeledDepth) * gapWidth);

            labels[i+ labelsWithHigherThanCurrentDepth].text = string.Format("{0:D}m", labeledDepth - gapsBeetwenLabels * i);
        }
    }
}
