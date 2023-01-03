using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;

public class ColorChanger : MonoBehaviour
{
    [Serializable]
    public class RockMaterialListPosition
    {
        public Material material;
        public Color baseColor;
        public float baseWeight;
    }

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
    public GameModel gameModel;
    public List<Material> copiedMaterials;
    public List<Material> materials;
    public float HRockOffset;
    public bool customRocksSV;

    [ConditionalField(nameof(customRocksSV))]
    public float SRock;
    [ConditionalField(nameof(customRocksSV))]
    public float VRock;
    public List<RockMaterialListPosition> rockMaterials;

    private void Start()
    {
        foreach (Material material in materials)
        {
            Material newMat = new Material(material);
            copiedMaterials.Add(newMat);
        }
    }

    void OnApplicationQuit()
    {
        for (int i=0; i<materials.Count; i++)
        {
            materials[i].CopyPropertiesFromMaterial(copiedMaterials[i]);
        }

        foreach (RockMaterialListPosition rockMaterial in rockMaterials)
        {
            rockMaterial.material.color = new Color(1, 1, 1, 1);
        }
    }
    void Update()
    {
        //float lightness = Math.Max(0f, (255f - 50*(float)data.depth)/255f);
        //color = new Color(lightness, 1f, lightness, 1f);

        H=((360f/Hperiod)*(float)gameModel.Depth) %360;

        SCounter = (float)gameModel.Depth % SPeriod;

        S = SMax - (SMax-SMin) * Math.Abs(SPeriod/2f - SCounter)/(SPeriod/2f);

        VCounter = (float)gameModel.Depth % VPeriod;

        V = VMax - (VMax - VMin) * Math.Abs(VPeriod/2f - VCounter)/(VPeriod/2f);

        color = Color.HSVToRGB(H/360, S, V);
        Color rockColor;
        if (customRocksSV) {
            rockColor = Color.HSVToRGB((H+HRockOffset)%360 / 360, SRock, VRock);
        } else {
            rockColor = Color.HSVToRGB((H + HRockOffset) % 360 / 360, S, V);
        }

        foreach (Material material in materials)
        {
            material.color = color;
        }
        foreach (RockMaterialListPosition rockMaterial in rockMaterials)
        {
            float r = rockColor.r * (1 - rockMaterial.baseWeight) + rockMaterial.baseColor.r * rockMaterial.baseWeight;
            float g = rockColor.g * (1 - rockMaterial.baseWeight) + rockMaterial.baseColor.g * rockMaterial.baseWeight;
            float b = rockColor.b * (1 - rockMaterial.baseWeight) + rockMaterial.baseColor.b * rockMaterial.baseWeight;
            Color finalColor = new Color(r, g, b);
            rockMaterial.material.color = finalColor;
        }
    }
}