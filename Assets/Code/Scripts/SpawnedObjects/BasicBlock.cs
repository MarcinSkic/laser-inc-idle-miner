using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class BasicBlock : MonoBehaviour
{
    [SerializeField]
    private BoxCollider boxCollider;
    public BoxCollider BoxCollider { get => boxCollider; }

    public ObjectPool<BasicBlock> Pool { private get; set; }

    protected double hp;
    protected double maxHp;


    [Header("TEMP")]
    public GameController gameController;
    public Data data;

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)){   //TODO: Check on mobile
            TakeDamage(1);
        }
    }

    public void InitBlock(double maxHp)
    {
        this.maxHp = maxHp;
        hp = maxHp;
    }

    public void TakeDamage(double damage)
    {
        hp -= damage;

        if (data.displayFloatingText)
        {
            FloatingTextController.CreateFloatingText(damage.ToString(), transform);
        }

        if (hp <= 0)
        {
            DestroyBlock();
        }
    }

    public UnityAction onBlockDestroyed;
    private void DestroyBlock()
    {
        onBlockDestroyed?.Invoke();

        gameController.AddMoney(maxHp); //TODO-CURRENT: Connect to action

        Pool.Release(this);
    }
}
