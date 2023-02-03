using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoisonBall : BaseBall<BallData>, IPoolable<PoisonBall>, IUpgradeable<BallData>
{
    public new ObjectPool<PoisonBall> Pool { get; set; }

    protected override void TryDealDamage(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<BasicBlock>(out var block))
        {
            block.TakeDamage(Data.values[UpgradeableValues.Damage]);
            // TODO - what should be the proportion of poison damage vs bounce damage?
            block.TakePoison(Data.values[UpgradeableValues.Damage]);
        }
    }
}
