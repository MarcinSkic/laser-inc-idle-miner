using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SniperBall : BaseBall<BallData>, IPoolable<SniperBall>, IUpgradeable<BallData>
{
    public new ObjectPool<SniperBall> Pool { get; set; }

    private Transform blocksParent;

    public void SetVariables(Transform blocksParent)
    {
        this.blocksParent = blocksParent;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        var blocks = blocksParent.GetComponentsInChildren<BasicBlock>(false);
        if (collision.gameObject.CompareTag("border") && blocks.Length > 0)
        {
            var target = FindTarget();

            rb.velocity = (float)Data.values[UpgradeableValues.Special] * (float)Data.values[UpgradeableValues.Speed] * (target.transform.position - transform.position).normalized;
        }
    }

    private BasicBlock FindTarget()
    {
        var blocks = blocksParent.GetComponentsInChildren<BasicBlock>(false);
        
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