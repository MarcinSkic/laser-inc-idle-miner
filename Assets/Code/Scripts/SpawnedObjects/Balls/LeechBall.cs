using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LeechBall : BaseBall<BallData>, IPoolable<LeechBall>, IUpgradeable<BallData>
{
    public new ObjectPool<LeechBall> Pool { get; set; }

    private Transform blocksParent;

    public void SetVariables(Transform blocksParent)
    {
        this.blocksParent = blocksParent;
    }
}