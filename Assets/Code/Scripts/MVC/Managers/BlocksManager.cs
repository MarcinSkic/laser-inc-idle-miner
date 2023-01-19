using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class BlocksManager : MonoBehaviour
{
    [SerializeField] private BlocksModel model;
    [SerializeField] private GameModel gameModel;
    [SerializeField] private BlockSpawner blockSpawner;

    public UnityAction<double> onBlockDestroyed;

    private bool isMoving = true;

    public int blockMovementsInARow = 0;

    private void Update()
    {
        var blocks = model._dynamic_blocks.GetComponentsInChildren<BasicBlock>(false); //TODO: Very Temp
        MoveBlocks(blocks); // TODO: not optimal
        CheckIfWaveFinished(blocks); // TODO: not optimal
    }

    private void MoveBlocks(BasicBlock[] blocks)
    {
        bool condition;
        if (isMoving)
        {
            condition = !CheckForBlocksAboveY(blocks, model.maximum_block_movement_y);
        }
        else
        {
            condition = !CheckForBlocksAboveY(blocks, model.maximum_block_movement_y - model.block_movement_trigger_difference_y);
        }

        if (condition)
        {
            blockMovementsInARow++;
            foreach (BasicBlock block in blocks)
            {
                block.transform.position += new Vector3(0, model.speed, 0) * Time.deltaTime;
            }
            var bgTextures = model.movingBorderTexturesParent.GetComponentsInChildren<Transform>(false);
            for (int i = 1; i < bgTextures.Length; i++)
            {
                // Translate by�by z�y bo parent ma scale i rotation
                Vector3 blockMovement = new Vector3(0, model.speed, 0) * Time.deltaTime;
                bgTextures[i].position += blockMovement;
                if (bgTextures[i].position.y >= 28f)
                {
                    bgTextures[i].position -= new Vector3(0, 16.8f, 0);
                }
            }

            gameModel.Depth += model.speed * Time.deltaTime;

            isMoving = true;
        }
        else
        {
            blockMovementsInARow = 0;
            isMoving = false;
        }
    }

    bool CheckForBlocksAboveY(BasicBlock[] blocks, float y)
    {
        foreach (BasicBlock block in blocks)
        {
            if (block.transform.position.y > y)
            {
                return true;
            }
        }
        return false;
    }

    public float GetMinBlockY(BasicBlock[] blocks)
    {
        float minY = -18f;
        foreach (BasicBlock block in blocks)
        {
            if (block.transform.position.y < minY)
            {
                minY = block.transform.position.y;
            }
        }
        return minY;
    }

    bool CheckForBlocksBelowY(BasicBlock[] blocks, float y)
    {
        foreach (BasicBlock block in blocks)
        {
            if (block.transform.position.y < y)
            {
                return true;
            }
        }
        return false;
    }

    private void CheckIfWaveFinished(BasicBlock[] blocks)
    {
        if (!CheckForBlocksBelowY(blocks, model.block_spawning_trigger_minimum_y))
        {
            blockSpawner.minExistingY = GetMinBlockY(blocks);
            blockSpawner.SpawnBlockRow(out List<BasicBlock> spawnedBlocks);

            for (int i = 0; i < spawnedBlocks.Count; i++)
            {
                spawnedBlocks[i].AssignEvents(onBlockDestroyed);
            }
        }
    }
    public double GetDepthBlocksHealth()
    {
        return (gameModel.Depth/10) * Math.Pow(model.block_health_exponentiation_base, gameModel.Depth / 10);
    }
}
