using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBall : BasicBall
{
    protected float speedBoost;
    public void InitBall(double speed, double damage, float speedBoost)
    {
        base.InitBall(speed, damage);
        this.speedBoost = speedBoost;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        var blocks = gameController._dynamic_blocks.GetComponentsInChildren<BasicBlock>(false);
        if (collision.gameObject.tag == "border" && blocks.Length > 0)
        {
            var target = FindTarget();

            rb.velocity = (target.transform.position - transform.position).normalized * (float)speed * speedBoost;
        }
    }

    private BasicBlock FindTarget()
    {
        var blocks = gameController._dynamic_blocks.GetComponentsInChildren<BasicBlock>(false);
        
        var target = blocks[0];

        foreach(var block in blocks)
        {
            if(Vector3.Distance(block.BoxCollider.ClosestPoint(transform.position), transform.position) < (Vector3.Distance((target.transform.position), transform.position)))
            {
                target = block;
            }
        }

        return target;
    }
}
