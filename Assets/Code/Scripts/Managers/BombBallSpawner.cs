using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBallSpawner : BallSpawner<BombBall>
{
    protected override void Get(BombBall ball)
    {
        data.bombBallCount++;
        ball.LoadData(data.GetSpd(), data.GetBallDamage(),data.explosionSize);
        base.Get(ball);
    }

    protected override void Release(BombBall element)
    {
        data.bombBallCount--;
        base.Release(element);
    }
}
