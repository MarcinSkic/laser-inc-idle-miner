using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private BasicBlock blockPrefab;
    [SerializeField] private Transform blocksParent;
    [SerializeField] private Vector2 spawnArea;

    [Header("DEBUG")]
    [Header("Read Only")]
    [SerializeField] private int active;
    [SerializeField] private int inactive;

    [Header("TEMP")]
    [SerializeField] private Data data;
    [SerializeField] private GameController gameController;

    private ObjectPool<BasicBlock> pool;
    private void Awake()
    {
        pool = new ObjectPool<BasicBlock>(CreateBlock,BlockGet,BlockRelease);
    }

    public void SpawnBlock()
    {
        var block = pool.Get();
        block.transform.position = new Vector3(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y), 0);
    }

    public void Update()
    {
        active = pool.CountActive;
        inactive = pool.CountInactive;
    }

    private BasicBlock CreateBlock()
    {
        var block = Instantiate(blockPrefab, blocksParent);
        block.gameController = gameController;
        block.pool = pool;
        block.data = data;

        return block;
    }

    private void BlockGet(BasicBlock block)
    {
        block.hp = data.GetWaveEnemiesHealth();
        block.maxHp = block.hp;

        block.gameObject.SetActive(true);
    }

    private void BlockRelease(BasicBlock block)
    {
        block.gameObject.SetActive(false);
    }
}
