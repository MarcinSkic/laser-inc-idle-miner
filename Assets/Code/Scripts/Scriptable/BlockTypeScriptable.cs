using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockType
{
    public string name;
    public Material material;
    public double hpMultiplier;
    public double rewardMultiplier;
    public double minDepth;
    public double fullDepth;
    public double maxChance;
}

[CreateAssetMenu(fileName = "test block", menuName = "ScriptableObjects/BlockType", order = 1)]

public class BlockTypeScriptable : ScriptableObject
{
    [SerializeField]
    private BlockType blockType;


    public BlockType BlockType { 
        get
        {
            var block = Functions.GetObjectCopy(blockType);
            return block;
        }
        set => blockType = value;
    }
}
