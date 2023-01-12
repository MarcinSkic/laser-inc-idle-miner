using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LeechBall : BaseBall<BallData>, IPoolable<LeechBall>, IUpgradeable<BallData>
{
    public new ObjectPool<LeechBall> Pool { get; set; }

    private BasicBlock leechedBlock;
    private Vector3 lastLeechedBlockPosition;
    private bool leeching = false;
    private Vector3 velocityToRegain;

    // TODO - unnecessary? or add homing?
    private Transform blocksParent;

    public void FixedUpdate()
    {
        if (leechedBlock && leechedBlock.isActiveAndEnabled)
        {
            // deal {damage} damage/s
            leechedBlock.TakeDamage(Data.values[UpgradeableValues.Damage]*Time.deltaTime);

            // move leech together with its block
            Vector3 newLeechedBlockPosition = leechedBlock.transform.position;
            if (lastLeechedBlockPosition != newLeechedBlockPosition)
            {
                // TODO - find a better way?
                // stop leeching if block is already repooled and went to the bottom
                if ((newLeechedBlockPosition - lastLeechedBlockPosition).magnitude > 1)
                {
                    /*Debug.LogWarning(lastLeechedBlockPosition - newLeechedBlockPosition);*/
                    StopLeeching();
                }
                else
                {
                    // move
                    transform.position += newLeechedBlockPosition - lastLeechedBlockPosition;
                }
            }
            lastLeechedBlockPosition = newLeechedBlockPosition;
        } 
        // if block has been destroyed
        else if (leeching)
        {
            StopLeeching();
        }
    }

    private void StopLeeching()
    {
        // reset leeching variables
        leeching = false;
        leechedBlock = null;

        // restore normal movement
        rb.isKinematic = false;
        rb.velocity = velocityToRegain;
        SetVelocity();
    }

    // TODO - unnecessary? or add homing?
    public void SetVariables(Transform blocksParent)
    {
        this.blocksParent = blocksParent;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // if collided with a block
        if (collision.gameObject.TryGetComponent<BasicBlock>(out var block))
        {
            // start leeching
            leechedBlock = block;
            leeching = true;

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