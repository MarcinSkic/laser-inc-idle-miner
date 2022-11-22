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

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)){
            TakeDamage(1);
        }
    }

    public void InitBlock(double maxHp)
    {
        this.maxHp = maxHp;
        hp = maxHp;
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
        onBlockDestroyed?.Invoke(maxHp);

        Pool.Release(this);
    }
}
