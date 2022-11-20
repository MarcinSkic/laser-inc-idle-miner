using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SniperBall : BaseBall<SniperBallData>, IPoolable<SniperBall>, IUpgradeable<SniperBallData>
{
    public new ObjectPool<SniperBall> Pool { get; set; }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        var blocks = gameController._dynamic_blocks.GetComponentsInChildren<BasicBlock>(false);
        if (collision.gameObject.tag == "border" && blocks.Length > 0)
        {
            var target = FindTarget();

            rb.velocity = (target.transform.position - transform.position).normalized * (float)Data.speed * Data.speedBoost;
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

[System.Serializable]
public class SniperBallData : BaseBallData
{
    public float speedBoost;
}
