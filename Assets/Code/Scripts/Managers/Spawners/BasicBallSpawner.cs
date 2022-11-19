using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBallSpawner : BallSpawner<BasicBall,BasicBallData>
{
    protected override void Get(BasicBall ball)
    {
        data.basicBallCount++;
        ball.SetDataReference(data.basicBallData);
        base.Get(ball);
    }

    protected override void Release(BasicBall ball)
    {
        data.basicBallCount--;
        base.Release(ball);
    }
}
