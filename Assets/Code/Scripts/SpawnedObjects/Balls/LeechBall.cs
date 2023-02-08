using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LeechBall : BaseBall<BallData>, IPoolable<LeechBall>, IUpgradeable<BallData>
{
    public new ObjectPool<LeechBall> Pool { get; set; }

    private BasicBlock leechedBlock;
    private Vector3 lastLeechedBlockPosition;
    private Vector3 velocityToRegain;

    public void Update()
    {
        if (leechedBlock && leechedBlock.isActiveAndEnabled)
        {
            // move leech together with its block
            Vector3 newLeechedBlockPosition = leechedBlock.transform.position;
            if (lastLeechedBlockPosition != newLeechedBlockPosition)
            {
                transform.position += newLeechedBlockPosition - lastLeechedBlockPosition;
            }
            lastLeechedBlockPosition = newLeechedBlockPosition;

            leechedBlock.TakeDamage(Data.values[UpgradeableValues.Damage] * Time.deltaTime, true);
        }
    }

    private void StopLeeching(double _)
    {
        // reset leeching variables
        leechedBlock = null;

        // restore normal movement
        rb.isKinematic = false;
        rb.velocity = velocityToRegain;
        SetVelocity();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // if collided with a block
        if (collision.gameObject.TryGetComponent<BasicBlock>(out var block))
        {
            // start leeching
            leechedBlock = block;
            leechedBlock.onBlockDestroyed += StopLeeching;

            // turn off normal movement, switch to following the block
            lastLeechedBlockPosition = leechedBlock.transform.position;
            velocityToRegain = rb.velocity;
            rb.isKinematic = true;
        } else
        {
            SetVelocity();
        }
    }
}