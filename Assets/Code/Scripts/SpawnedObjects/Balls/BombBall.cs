using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBall : BasicBall
{
    private float explosionSize;
    public void InitBall(double speed, double damage, float explosionSize)
    {
        this.explosionSize = explosionSize;
        base.InitBall(speed, damage);
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
