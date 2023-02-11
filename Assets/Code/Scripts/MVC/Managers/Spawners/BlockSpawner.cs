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

    public ResourcesManager resourceManager;

    private int column;
    private int columns;

    public float minExistingY = 0;

    private bool spawnOnPredefinedPosition = false;
    private Vector2 spawnPosition;
    private string spawnBlockTypeName = "";
    
    [InitializationField]
    public List<BlockTypeScriptable> blockTypeScriptables;
    [ReadOnly]
    public List<BlockType> blockTypes;

    [Header("DEBUG")]
    [SerializeField] private bool ifSpawningOnGrid;
    [SerializeField] private bool alwaysSpawnMinerals;

    protected override BasicBlock Create()
    {
        BasicBlock block = base.Create();
        block.blockSpawner = this;
        return block;
    }

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

    public void SpawnBlocksOnPositions(Vector2[] positions, string[] blockTypeNames, out List<BasicBlock> spawnedBlocks)
    {
        spawnedBlocks = new List<BasicBlock>();

        spawnOnPredefinedPosition = true;
        for (int i=0; i<positions.Length; i++)
        {
            spawnPosition = positions[i];
            spawnBlockTypeName = blockTypeNames[i] ?? "";
            Spawn(out var block);
            spawnedBlocks.Add(block);
        }
        spawnOnPredefinedPosition = false;
    }

    public override void Spawn(out BasicBlock spawnedBlock)
    {
        base.Spawn(out BasicBlock block);

        float xPos;
        float yPos;

        if (spawnOnPredefinedPosition)
        {
            xPos = spawnPosition.x;
            yPos = spawnPosition.y;

            block.transform.position = new Vector3(xPos, yPos, 0);
        } else
        {
            
            xPos = ((column / ((columns) - 1f)) * 2f - 1f) * xArea;
            yPos = 0;
            if (!ifSpawningOnGrid)
            {
                xPos += Random.Range(-randomOffset.x, randomOffset.x);
                yPos += Random.Range(-randomOffset.y, randomOffset.y);
            }
            else
            {
                yPos += minExistingY - 1.236094f - spawnOffset.y;   //Beautiful magic number <3
            }

            block.transform.position = spawnOffset + new Vector3(xPos, yPos, 0);
        }

        
        spawnedBlock = block;
    }

    protected override void Get(BasicBlock block)
    {
        int typeId = 0;

        if (spawnBlockTypeName != "")
        {
            for (int i=0; i<blockTypes.Count; i++)
            {
                if (blockTypes[i].name == spawnBlockTypeName)
                {
                    typeId = i;
                    break;
                }
            }
            spawnBlockTypeName = "";
        } else
        {
            #region generate random block type
            for (int i = blockTypes.Count - 1; i >= 0; i--)
            {
                double chance = 0;
                if (gameModel.Depth >= blockTypes[i].fullDepth || alwaysSpawnMinerals)
                {
                    chance = blockTypes[i].maxChance;
                }
                else if (gameModel.Depth >= blockTypes[i].minDepth)
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
            #endregion
        }

        block.InitBlock(manager.GetDepthBlocksHealth(), blockTypes[typeId]);

        int oreModelIndex = Random.Range(0, 6);
        Transform ModelsParent = block.gameObject.transform.GetChild(1).GetChild(0);         // TODO-UGLY: get rid of those GetChild

        // set ore model
        for (int i = 0; i < 6; i++)
        {
            ModelsParent.GetChild(i).gameObject.SetActive(false);
        }
        ModelsParent.GetChild(oreModelIndex).gameObject.SetActive(true);
        // set ore material
        ModelsParent.GetChild(oreModelIndex).GetComponentInChildren<MeshRenderer>().material = blockTypes[typeId].material;
        // set ore rotation
        int whetherToRotate = Random.Range(0, 2);
        ModelsParent.parent.rotation = Quaternion.Euler(new Vector3(0, 0, whetherToRotate*180));


        base.Get(block);
    }

    protected override void Release(BasicBlock element)
    {
        base.Release(element);
        element.onBlockDestroyed = null;
    }
}
