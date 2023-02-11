using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ShadowBall : BaseBall<BallData>, IPoolable<ShadowBall>, IUpgradeable<BallData>
{
    public new ObjectPool<ShadowBall> Pool { get; set; }

    public void HandleDetectionFromTrigger(Collider collider)
    {
        if (collider.gameObject.TryGetComponent<BasicBlock>(out var block))
        {
            block.TakeDamage(Data.values[UpgradeableValues.Damage]);
        }
    }
}
