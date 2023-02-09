using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

public class BlocksModel : MonoBehaviour
{
    public float speed = 5f;
    public float maximum_block_movement_y = 5f;
    public float block_movement_trigger_difference_y = 0.5f;
    public double block_health_exponentiation_base = 1.03;
    public float block_spawning_trigger_minimum_y = -21f;

    public Transform _dynamic_blocks;
    public Transform floatingTexts;
    public Transform _dynamic_balls;
    public GameObject movingBorderTexturesParent;

    public UnityAction<double> onDestroyedBlocksCountChange;

    [SerializeField]
    private long destroyedBlocks = 0;
    public long destroyedBlocksCount
    {
        get { return destroyedBlocks; }
        set
        {
            destroyedBlocks = value;
            onDestroyedBlocksCountChange?.Invoke(destroyedBlocks);
        }
    }
}
