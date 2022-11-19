using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBallSpawner : BallSpawner<BombBall,BombBallData>
{
    protected override void Get(BombBall ball)
    {
        data.bombBallCount++;
        ball.SetDataReference(data.bombBallData);
        base.Get(ball);
    }

    protected override void Release(BombBall element)
    {
        data.sniperBallCount--;
        base.Release(element);
    }
}
