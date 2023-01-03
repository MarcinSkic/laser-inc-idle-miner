using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBallSpawner : BallSpawner<BombBall, BallData>
{
    protected override void Get(BombBall ball)
    {
        ball.SetDataReference(ballsModel.ballsData[UpgradeableObjects.BombBall]);
        base.Get(ball);
    }

    protected override void Release(BombBall element)
    {
        base.Release(element);
    }
}
