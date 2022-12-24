using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BombBall : BaseBall<BombBallData>, IPoolable<BombBall>, IUpgradeable<BombBallData>
{
    public new ObjectPool<BombBall> Pool { get; set; }

    private Transform blocksParent;

    public void SetVariables(Transform blocksParent)
    {
        this.blocksParent = blocksParent;
    }

    protected override void TryDealDamage(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<BasicBlock>(out _)){
            var blocks = blocksParent.GetComponentsInChildren<BasicBlock>(false);

            foreach (var block in blocks) {
                if(Vector3.Distance(block.BoxCollider.ClosestPoint(transform.position),transform.position) < Data.explosionSize)
                {
                    block.TakeDamage(Data.damage);
                } 
            }
        }
    }

    
}

[System.Serializable]
public class BombBallData : BaseBallData
{
    public int explosionSize;
}
