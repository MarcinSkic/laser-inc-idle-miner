using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using System.Linq;

public class BlocksManager : MonoBehaviour
{
    [SerializeField] private BlocksModel model;
    [SerializeField] private GameModel gameModel;
    [SerializeField] private BlockSpawner blockSpawner;

    public UnityAction<double> onBlockDestroyed;

    public int blockMovementsInARow = 0;

    [SerializeField] private float backgroundYChange;

    private void Update()
    {
        //This condition is needed if we ever comeback to blocks having random y in the same wave
        //if((isMoving && !IsAnyBlockAboveY(model.maximum_block_movement_y)) || (!isMoving && !IsAnyBlockAboveY(model.maximum_block_movement_y - model.block_movement_trigger_difference_y)))
        if(!IsAnyBlockAboveY(model.maximum_block_movement_y))
        {
            IncreaseDepth();   
            blockMovementsInARow++;

            if (!IsAnyBlockBelowY(model.block_spawning_trigger_minimum_y))
            {
                SpawnBlocksRow();
            }
        } 
        else
        {
            blockMovementsInARow = 0;
        }
    }

    public UnityAction<Vector3> onDepthIncrease;
    private void IncreaseDepth()
    {
        //        var movement = new Vector3(0, Math.Min(model.baseSpeed * ((blockMovementsInARow / model.movementsPerSpeedMultiplier) + 1f), model.depthIncreaseSpeedLimit), 0) * Time.deltaTime;
        var movement = new Vector3(0, (model.baseSpeed + model.maximumBonusSpeed * Mathf.Min(Mathf.Sqrt(blockMovementsInARow / model.movementsToFullBonus), 1f))*Time.deltaTime, 0);
        gameModel.Depth += movement.y;

        MoveBlocks(movement);
        MoveFloatingTexts(movement);
        MoveBackground(movement);
        onDepthIncrease?.Invoke(movement);   
    }

    private void MoveBlocks(Vector3 delta)
    {
        foreach (BasicBlock block in model.blocks)
        {
            block.transform.position += delta;
        }
    }

    private void MoveFloatingTexts(Vector3 delta)
    {
        foreach (FloatingText text in FloatingTextSpawner.Instance.floatingTexts)
        {
            text.ObjectPosition += delta;
        }
    }

    private void MoveBackground(Vector3 delta)
    {
        for (int i = 0; i < model.movingBackgrounds.Length; i++)
        {
            model.movingBackgrounds[i].position += delta;
            if (model.movingBackgrounds[i].position.y >= 28f)
            {
                model.movingBackgrounds[i].position -= new Vector3(0, backgroundYChange, 0);
            }
        }
    }

    public void IncrementDestroyedBlocksCount()
    {
        model.DestroyedBlocks++;
    }

    bool IsAnyBlockAboveY(float y)
    {
        foreach (BasicBlock block in model.blocks)
        {
            if (block.transform.position.y > y)
            {
                return true;
            }
        }
        return false;
    }

    public float GetMinBlockY()
    {
        float minY = -18f;
        foreach (BasicBlock block in model.blocks)
        {
            if (block.transform.position.y < minY)
            {
                minY = block.transform.position.y;
            }
        }
        return minY;
    }

    bool IsAnyBlockBelowY(float y)
    {
        foreach (BasicBlock block in model.blocks)
        {
            if (block.transform.position.y < y)
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnBlocksRow()
    {
        blockSpawner.minExistingY = GetMinBlockY();
        blockSpawner.SpawnBlockRow(out List<BasicBlock> spawnedBlocks);

        for (int i = 0; i < spawnedBlocks.Count; i++)
        {
            SetupSpawnedBlock(spawnedBlocks[i]);
        }
    }
    public double GetDepthBlocksHealth()
    {
        return (gameModel.Depth/10) * Math.Pow(model.block_health_exponentiation_base, gameModel.Depth / 10);
    }

    public void SavePersistentData(PersistentData data)
    {
        data.blocksPositions = model._dynamic_blocks.GetComponentsInChildren<BasicBlock>(false).Select(block => new Vector2(block.transform.position.x,block.transform.position.y)).ToArray();
    }

    public void LoadPersistentData(PersistentData data)
    {
        if(data.blocksPositions != null)
        {
            // TODO: wczytaj tu poprawnie te nazwy typów klocków
            string[] blockTypeNames = new string[data.blocksPositions.Length];

            blockSpawner.SpawnBlocksOnPositions(data.blocksPositions, blockTypeNames, out List<BasicBlock> spawnedBlocks);

            for (int i = 0; i < spawnedBlocks.Count; i++)
            {
                SetupSpawnedBlock(spawnedBlocks[i]);
            }
        }
    }

    private void SetupSpawnedBlock(BasicBlock block) 
    {
        model.blocks.Add(block);
        block.onBlockDestroyed += OnBlockDestroyed;
    }

    private void OnBlockDestroyed(double value)
    {
        onBlockDestroyed.Invoke(value);
    }
}
