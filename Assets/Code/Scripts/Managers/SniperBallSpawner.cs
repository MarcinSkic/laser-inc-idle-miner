using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBallSpawner : BallSpawner<SniperBall>
{
    protected override void Get(SniperBall ball)
    {
        data.sniperBallCount++;
        ball.LoadData(data.GetSpd(), data.GetBallDamage(), data.speedBoost);
        base.Get(ball);
    }
    protected override void Release(SniperBall element)
    {
        data.sniperBallCount--;
        base.Release(element);
    }
}
