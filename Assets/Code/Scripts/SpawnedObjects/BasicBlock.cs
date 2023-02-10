using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class BasicBlock : MonoBehaviour, IPoolable<BasicBlock>
{
    [SerializeField]
    private BoxCollider boxCollider;
    public BoxCollider BoxCollider { get => boxCollider; }

    public ObjectPool<BasicBlock> Pool { get; set; }

    [SerializeField] protected BlockType type;
    protected double hp;
    protected double maxHp;
    protected double reward;
    protected double poisonPerSecond = 0;
    private FloatingText floatingRepeatedText = null;
    private double repeatedTotalValue = 0;

    public void FixedUpdate()
    {
        if (poisonPerSecond > 0)
        {
            // TODO - can be optimized
            TakeDamage(poisonPerSecond * Time.deltaTime, true);
        }
    }

    public void InitBlock(double baseHp, BlockType blockType)
    {
        maxHp = baseHp*blockType.hpMultiplier;
        hp = maxHp;
        reward = maxHp * blockType.rewardMultiplier;
        type = blockType;
        poisonPerSecond = 0;
        Debug.Log($"Init: {hp}hp {maxHp}maxHp {baseHp}baseHp {reward}reward {type}blockType");
    }

    public void DisplayDamageTaken(double damage, bool repeating = false)
    {
        if (repeating) {
            if (floatingRepeatedText == null)
            {
                FloatingTextSpawner.Instance.Spawn(out var huh);
                floatingRepeatedText = huh;
                floatingRepeatedText.Init(transform.position,true);
            }

            repeatedTotalValue += damage;
            floatingRepeatedText.SetText(repeatedTotalValue.ToString("F2"));
        } else {
            FloatingTextSpawner.Instance.SpawnDefault(damage.ToString("F2"), transform);
        }
    }

    public void TakeDamage(double damage, bool repeating=false)
    {
        hp -= damage;

        if (SettingsModel.Instance.DisplayFloatingText)
        {
            DisplayDamageTaken(damage, repeating);
        }

        if (hp <= 0 && gameObject.activeSelf)
        {
            RemoveBlock();
        }
    }

    public void TakePoison(double damagePerSecond)
    {
        poisonPerSecond += damagePerSecond;
    }

    /// <summary>
    /// Double is maxHp that is used to money calculations
    /// </summary>
    public UnityAction<double> onBlockDestroyed;  //TODO-FUTURE: Maybe change it to transfer data packet if it will be used for upgrades
    private void RemoveBlock()
    {
        poisonPerSecond = 0;
        repeatedTotalValue = 0;

        switch (type.name)
        {
            case "normal":
                StatisticsModel.Instance.MinedNormalBlocks = StatisticsModel.Instance.MinedNormalBlocks + 1;
                break;
            case "copper":
                StatisticsModel.Instance.MinedCopperBlocks = StatisticsModel.Instance.MinedCopperBlocks + 1;
                break;
            case "iron":
                StatisticsModel.Instance.MinedIronBlocks = StatisticsModel.Instance.MinedIronBlocks + 1;
                break;
            case "gold":
                StatisticsModel.Instance.MinedGoldBlocks = StatisticsModel.Instance.MinedGoldBlocks + 1;
                break;
            case "diamond":
                StatisticsModel.Instance.MinedDiamondBlocks = StatisticsModel.Instance.MinedDiamondBlocks + 1;
                break;
            case "uranium":
                StatisticsModel.Instance.MinedUraniumBlocks = StatisticsModel.Instance.MinedUraniumBlocks + 1;
                break;
        }

        onBlockDestroyed?.Invoke(reward);
        if (floatingRepeatedText != null)
        {
            floatingRepeatedText.Deinit();
            floatingRepeatedText = null;
        }
        Pool.Release(this);
    }
}
