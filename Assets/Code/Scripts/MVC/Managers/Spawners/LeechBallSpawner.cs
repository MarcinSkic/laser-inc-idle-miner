using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeechBallSpawner : BallSpawner<LeechBall,BallData>
{
    protected override void Get(LeechBall ball)
    {
        ball.SetDataReference(ballsModel.ballsData[UpgradeableObjects.LeechBall]);
        base.Get(ball);
    }
    protected override void Release(LeechBall element)
    {
        base.Release(element);
    }
}
