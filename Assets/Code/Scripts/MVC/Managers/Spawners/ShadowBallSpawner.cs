using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBallSpawner : BallSpawner<ShadowBall, BallData>
{
    protected override void Get(ShadowBall ball)
    {
        ball.SetDataReference(ballsModel.ballsData[UpgradeableObjects.ShadowBall]);
        base.Get(ball);
    }

    protected override void Release(ShadowBall element)
    {
        base.Release(element);
    }
}
