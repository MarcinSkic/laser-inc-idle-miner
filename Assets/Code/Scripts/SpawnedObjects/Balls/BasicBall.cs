using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BasicBall : BaseBall<BasicBallData>, IPoolable<BasicBall>, IUpgradeable<BasicBallData>
{
    public new ObjectPool<BasicBall> Pool { get; set ; }
}

[System.Serializable]
public class BasicBallData : BaseBallData{}