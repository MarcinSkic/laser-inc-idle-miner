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

    protected double hp;
    protected double maxHp;
    protected double reward;
    public Data data;

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)){
            TakeDamage(1);
        }
    }

    public void InitBlock(double baseHp, double hpMultiplier, double rewardMultiplier)
    {
        maxHp = baseHp*hpMultiplier;
        hp = maxHp;
        reward = maxHp * rewardMultiplier;
    }

    public void AssignEvents(UnityAction<double> onBlockDestroyed)
    {
        this.onBlockDestroyed = onBlockDestroyed;
    }

    public void TakeDamage(double damage)
    {
        hp -= damage;

        if (hp <= 0 && gameObject.activeSelf)
        {
            RemoveBlock();
        }

        //TODO: Add to Settings.Instance -> data.displayFloatingText
        /*if (false)   
        {
            FloatingTextController.CreateFloatingText(damage.ToString(), transform);
        }*/
        
    }

    /// <summary>
    /// Double is maxHp that is used to money calculations
    /// </summary>
    private UnityAction<double> onBlockDestroyed;  //TODO-FUTURE: Maybe change it to transfer data packet if it will be used for upgrades
    private void RemoveBlock()
    {
        onBlockDestroyed?.Invoke(reward);
        Pool.Release(this);
    }
}
