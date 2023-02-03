using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBallSpawner : BallSpawner<PoisonBall, BallData>
{
    protected override void Get(PoisonBall ball)
    {
        ball.SetDataReference(ballsModel.ballsData[UpgradeableObjects.PoisonBall]);
        base.Get(ball);
    }

    protected override void Release(PoisonBall element)
    {
        base.Release(element);
    }
}
