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

    public void SpawnBlockRow(out BasicBlock[] spawnedBlocks)
    {
        //base.Spawn(out BasicBlock block);
        spawnedBlocks = new BasicBlock[] { };

        columns = Random.Range(7, 10);

        for (column = 0; column<columns; column++)
        {
            Spawn(out BasicBlock spawnedBlock);
            spawnedBlocks.Append(spawnedBlock);
            
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
        block.InitBlock(data.GetDepthBlocksHealth());

        base.Get(block);
    }
}
