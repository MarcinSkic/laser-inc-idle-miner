using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;

public class ColorChanger : MonoBehaviour
{
    [ReadOnly] public float H;
    [ReadOnly] public float S;
    [ReadOnly] public float V;

    float HCounter;
    public float Hperiod;

    float SCounter;
    public float SPeriod;
    public float SMax;
    public float SMin;

    float VCounter;
    public float VPeriod;
    public float VMax;
    public float VMin;

    /*[ReadOnly]*/ public Color color;
    /*[ReadOnly]*/ public Color rockColor;
    public Data data;
    public List<Material> materials;
    public float HRockOffset;
    public float SRock;
    public float VRock;
    public List<Material> rockMaterials;


    void Update()
    {
        //float lightness = Math.Max(0f, (255f - 50*(float)data.depth)/255f);
        //color = new Color(lightness, 1f, lightness, 1f);

        H=((360f/Hperiod)*(float)data.depth)%360;

        SCounter = (float)data.depth % SPeriod;

        S = SMax - (SMax-SMin) * Math.Abs(SPeriod/2f - SCounter)/(SPeriod/2f);

        VCounter = (float)data.depth % VPeriod;

        V = VMax - (VMax - VMin) * Math.Abs(VPeriod/2f - VCounter)/(VPeriod/2f);

        color = Color.HSVToRGB(H/360, S, V);
        rockColor = Color.HSVToRGB((H+HRockOffset)%360 / 360, SRock, VRock);

        foreach (Material material in materials)
        {
            material.color = color;
        }
        foreach (Material material in rockMaterials)
        {
            material.color = rockColor;
        }
    }
}