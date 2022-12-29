using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BasicBall : BaseBall<BallData>, IPoolable<BasicBall>, IUpgradeable<BallData>
{
    public new ObjectPool<BasicBall> Pool { get; set ; }
}