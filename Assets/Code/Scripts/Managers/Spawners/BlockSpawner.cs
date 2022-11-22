using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class BlockSpawner : BaseSpawner<BasicBlock>
{
    [Space(5)]
    [Header("BLOCK SPAWNER")]
    [SerializeField] private Vector2 spawnArea;
    [SerializeField] private Vector3 spawnOffset;

    [Header("Temp")]
    [SerializeField] private Data data;
    [SerializeField] private GameController gameController;

    public override void Spawn(out BasicBlock spawnedBlock)
    {
        base.Spawn(out BasicBlock block);
        block.transform.position = spawnOffset + new Vector3(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y), 0);
        spawnedBlock = block;
    }

    protected override void Get(BasicBlock block)
    {
        block.InitBlock(data.GetDepthBlocksHealth());

        base.Get(block);
    }
}
