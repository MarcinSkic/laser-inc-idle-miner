using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBallSpawner : BallSpawner<SniperBall,SniperBallData>
{
    protected override void Get(SniperBall ball)
    {
        ball.SetDataReference(data.sniperBallData);
        base.Get(ball);
    }
    protected override void Release(SniperBall element)
    {
        base.Release(element);
    }
}
