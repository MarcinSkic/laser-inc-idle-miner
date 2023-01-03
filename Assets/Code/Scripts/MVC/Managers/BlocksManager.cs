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
            condition = CheckForBlocksAboveY(blocks, 5f);   //TODO-FILIP Magic number
        }
        else
        {
            condition = CheckForBlocksAboveY(blocks, 4.5f); //TODO-FILIP Magic number
        }

        if (!condition)
        {
            foreach (BasicBlock block in blocks)
            {
                block.transform.position += new Vector3(0, model.speed, 0) * Time.deltaTime; // TODO: temp
            }
            var bgTextures = model.movingBorderTexturesParent.GetComponentsInChildren<Transform>(false); //TODO: Very Temp
            for (int i = 1; i < bgTextures.Length; i++) // i=1 ¿eby nie ³apa³o parenta
            {
                // Translate by³by z³y bo parent ma scale i rotation
                bgTextures[i].position += new Vector3(0, model.speed, 0) * Time.deltaTime; // TODO: temp
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


    bool CheckForBlocksBelowY(BasicBlock[] blocks, float y = -21)   //TODO-FILIP Magic number
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
        if (!CheckForBlocksBelowY(blocks))
        {

            blockSpawner.SpawnBlockRow(out List<BasicBlock> spawnedBlocks);

            for (int i = 0; i < spawnedBlocks.Count; i++)
            {
                spawnedBlocks[i].AssignEvents(onBlockDestroyed);
            }
        }
    }
    public double GetDepthBlocksHealth()
    {
        return (gameModel.Depth/10) * Math.Pow(1.03, gameModel.Depth / 10);  //TODO-FILIP 1.03 = Magic number
    }
}
