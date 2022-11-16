using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BombBall : BasicBall, IPoolable<BombBall>
{
    public new ObjectPool<BombBall> Pool { get; set; }

    private float explosionSize;

    public void LoadData(double speed, double damage, float explosionSize)
    {
        this.speed = speed;
        this.damage = damage;
        this.explosionSize = explosionSize;
    }
    protected override void TryDealDamage(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<BasicBlock>(out _)){
            var blocks = gameController._dynamic_blocks.GetComponentsInChildren<BasicBlock>(false);

            foreach (var block in blocks) {
                if(Vector3.Distance(block.BoxCollider.ClosestPoint(transform.position),transform.position) < explosionSize)
                {
                    block.TakeDamage(damage); //TODO better damage pick
                } 
            }
        }
    }
}
