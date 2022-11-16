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

    [Header("Temp")]
    [SerializeField] private Data data;
    [SerializeField] private GameController gameController;

    public override BasicBlock Spawn()
    {
        var block = base.Spawn();
        block.transform.position = new Vector3(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y), 0);
        return block;
    }

    protected override void Get(BasicBlock block)
    {
        block.InitBlock(data.GetWaveEnemiesHealth());

        base.Get(block);
    }
}
