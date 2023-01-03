using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBallSpawner : BallSpawner<BasicBall, BallData>
{
    protected override void Get(BasicBall ball)
    {
        ball.SetDataReference(ballsModel.ballsData[UpgradeableObjects.BasicBall]);
        base.Get(ball);
    }

    protected override void Release(BasicBall ball)
    {
        base.Release(ball);
    }
}
