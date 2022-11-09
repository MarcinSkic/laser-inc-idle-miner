using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBall : BasicBall
{
    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.tag == "border" && gameController._dynamic_blocks.childCount > 0)
        {
            var target = FindTarget();

            rb.velocity = (target.transform.position - transform.position).normalized * (float)data.GetSpd() * data.speedBoost;
        }
    }

    private BasicBlock FindTarget()
    {
        var blocks = gameController._dynamic_blocks.GetComponentsInChildren<BasicBlock>();
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
