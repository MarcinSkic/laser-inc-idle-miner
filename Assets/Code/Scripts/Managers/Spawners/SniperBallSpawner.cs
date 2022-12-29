using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBallSpawner : BallSpawner<SniperBall,BallData>
{
    protected override void Get(SniperBall ball)
    {
        ball.SetDataReference(data.ballsData[UpgradeableObjects.SniperBall]);
        base.Get(ball);
    }
    protected override void Release(SniperBall element)
    {
        base.Release(element);
    }
}
