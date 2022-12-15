using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using System.Linq;

public class BlockSpawner : BaseSpawner<BasicBlock>
{
    [Space(5)]
    [Header("BLOCK SPAWNER")]
    [SerializeField] private float xArea;
    [SerializeField] private Vector2 randomOffset;
    [SerializeField] private Vector3 spawnOffset;

    [Header("Temp")]
    [SerializeField] private Data data;
    [SerializeField] private GameController gameController;

    private int column;
    private int columns;

    [System.Serializable]
    public class BlockType
    {
        public string name;
        public Material material;
        public double hpMultiplier;
        public double rewardMultiplier;

        public BlockType(string name, Material material, double hpMultiplier, double rewardMultiplier) {
            this.name = name;
            this.material = material;
            this.hpMultiplier = hpMultiplier;
            this.rewardMultiplier = rewardMultiplier;
        }
    }
    
  
    public BlockType[] blockTypes;

/*    public void Start()
    {
        blockTypes.Append(new BlockType("normal", materials[0], ))
    }*/

    public void SpawnBlockRow(out List <BasicBlock> spawnedBlocks)
    {
        //base.Spawn(out BasicBlock block);
        spawnedBlocks = new List <BasicBlock>();

        columns = Random.Range(7, 10);

        for (column = 0; column<columns; column++)
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
        xPos += Random.Range(-randomOffset.x, randomOffset.x);
        float yPos = Random.Range(-randomOffset.y, randomOffset.y);

        block.transform.position = spawnOffset + new Vector3(xPos, yPos, 0);
        spawnedBlock = block;
    }

    protected override void Get(BasicBlock block)
    {
        int typeId = Random.Range(0, 3);
        block.InitBlock(data.GetDepthBlocksHealth(), blockTypes[typeId].hpMultiplier, blockTypes[typeId].rewardMultiplier);
        block.gameObject.GetComponent<Renderer>().material = blockTypes[typeId].material;


        base.Get(block);
    }
}
