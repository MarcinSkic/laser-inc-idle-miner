using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using MyBox;

public class BlockSpawner : BaseSpawner<BasicBlock>
{
    [Space(5)]
    [Header("BLOCK SPAWNER")]
    [SerializeField] private float xArea;
    [SerializeField] private Vector2 randomOffset;
    [SerializeField] private Vector3 spawnOffset;

    [Header("Temp")]
    [SerializeField] private BlocksModel model;
    [SerializeField] private GameModel gameModel;
    [SerializeField] private BlocksManager manager;

    private int column;
    private int columns;

    public float minExistingY = 0;

    [InitializationField]
    public List<BlockTypeScriptable> blockTypeScriptables;
    [ReadOnly]
    public List<BlockType> blockTypes;

    [Header("DEBUG")]
    [SerializeField] private bool ifSpawningOnGrid;
    [SerializeField] private bool alwaysSpawnMinerals;
    

    protected override void Awake()
    {
        base.Awake();
        TransformScriptablesIntoBlockTypes();
    }

    private void TransformScriptablesIntoBlockTypes()
    {
        blockTypes = new List<BlockType>();

        foreach (var scriptable in blockTypeScriptables)
        {
            blockTypes.Add(scriptable.BlockType);
        }
    }


    /*    public void Start()
        {
            blockTypes.Append(new BlockType("normal", materials[0], ))
        }*/

    public void SpawnBlockRow(out List <BasicBlock> spawnedBlocks)
    {
        //base.Spawn(out BasicBlock block);
        spawnedBlocks = new List<BasicBlock>();

        if (ifSpawningOnGrid) {
            columns = 8;
        } else {
            columns = Random.Range(7, 10);
        }

        for (column = 0; column < columns; column++)
        {
            Spawn(out BasicBlock spawnedBlock);
            spawnedBlocks.Add(spawnedBlock);

        }

    }

    public override void Spawn(out BasicBlock spawnedBlock)
    {
        base.Spawn(out BasicBlock block);

        float xPos;
        // xPos = Random.Range(-spawnArea.x, spawnArea.x);
        xPos = ((column/((columns)-1f))*2f-1f)*xArea;
        float yPos = 0;
        if (!ifSpawningOnGrid)
        {
            xPos += Random.Range(-randomOffset.x, randomOffset.x);
            yPos += Random.Range(-randomOffset.y, randomOffset.y);
        } else
        {
            yPos += minExistingY - 1.236094f - spawnOffset.y;
        }

        block.transform.position = spawnOffset + new Vector3(xPos, yPos, 0);
        spawnedBlock = block;
    }

    protected override void Get(BasicBlock block)
    {
        int typeId = 0;
        for (int i=blockTypes.Count-1; i>=0; i--)
        {
            double chance = 0;
            if (gameModel.Depth >= blockTypes[i].fullDepth || alwaysSpawnMinerals) {
                chance = blockTypes[i].maxChance;
            } else if (gameModel.Depth >= blockTypes[i].minDepth)
            {
                double part = (gameModel.Depth - blockTypes[i].minDepth) / (blockTypes[i].fullDepth - blockTypes[i].minDepth);
                chance = part * blockTypes[i].maxChance;
            }
            if (chance > Random.Range(0f, 1f))
            {
                typeId = i;
                break;
            }
        }

        block.InitBlock(manager.GetDepthBlocksHealth(), blockTypes[typeId].hpMultiplier, blockTypes[typeId].rewardMultiplier);
        Material[] mats = block.gameObject.GetComponentInChildren<MeshRenderer>().materials;
        mats[1] = blockTypes[typeId].material;
        block.gameObject.GetComponentInChildren<MeshRenderer>().materials = mats;


        base.Get(block);
    }
}
