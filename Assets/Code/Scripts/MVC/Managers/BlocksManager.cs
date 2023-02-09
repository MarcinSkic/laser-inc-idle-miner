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

    private bool isMoving = true;

    public int blockMovementsInARow = 0;

    [SerializeField] private float backgroundYChange;

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
            foreach(FloatingText text in model.floatingTexts.GetComponentsInChildren<FloatingText>(false))
            {
                text.ObjectPosition += new Vector3(0, model.speed, 0) * Time.deltaTime;
            }
            /*foreach(BaseBall<BallData> ball in model._dynamic_balls.GetComponentsInChildren<BaseBall<BallData>>(false))
            {
                ball._rb.velocity += new Vector3(0, model.speed, 0) * Time.deltaTime;
            }*/
            var bgTextures = model.movingBorderTexturesParent.GetComponentsInChildren<Transform>(false);
            for (int i = 1; i < bgTextures.Length; i++)
            {
                // Translate by³by z³y bo parent ma scale i rotation
                Vector3 blockMovement = new Vector3(0, model.speed, 0) * Time.deltaTime;
                bgTextures[i].position += blockMovement;
                if (bgTextures[i].position.y >= 28f)
                {
                    bgTextures[i].position -= new Vector3(0, backgroundYChange, 0);
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

    public void incrementDestroyedBlocksCount()
    {
        model.destroyedBlocksCount = model.destroyedBlocksCount+1;
        model.onDestroyedBlocksCountChange?.Invoke(model.destroyedBlocksCount);
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
                spawnedBlocks[i].onBlockDestroyed += onBlockDestroyed;
            }
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
                spawnedBlocks[i].onBlockDestroyed += onBlockDestroyed;
            }
        }
    }
}
