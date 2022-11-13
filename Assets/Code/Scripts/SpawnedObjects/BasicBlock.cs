using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BasicBlock : MonoBehaviour
{
    public double hp;
    public double maxHp;
    public GameController gameController;
    public Data data;
    public ObjectPool<BasicBlock> pool { private get; set; }

    [SerializeField]    
    private BoxCollider boxCollider;
    public BoxCollider BoxCollider { get => boxCollider;}

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)){   //TODO: Check on mobile
            TakeDamage(1);
        }
    }

    public void TakeDamage(double damage)
    {
        hp -= damage;
        if (data.displayFloatingText)
        {
            FloatingTextController.CreateFloatingText(damage.ToString(), transform);
        }
    }

    private void Update()
    {
        if (hp <= 0)
        {
            OnBlockDestroyed();
        }
    }

    private void OnBlockDestroyed()
    {
        gameController.AddMoney(maxHp);
        pool.Release(this);
    }
}
