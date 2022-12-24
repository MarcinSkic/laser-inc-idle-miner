using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBallSpawner : BallSpawner<BombBall,BombBallData>
{
    protected override void Get(BombBall ball)
    {
        ball.SetDataReference(data.bombBallData);
        base.Get(ball);
    }

    protected override void Release(BombBall element)
    {
        base.Release(element);
    }
}
