using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBallSpawner : BallSpawner<BasicBall>
{
    protected override void Get(BasicBall ball)
    {
        data.basicBallCount++;  //TODO: Move to model when done architecture
        ball.LoadData(data.GetSpd(), data.GetBallDamage());      
        base.Get(ball);
    }

    protected override void Release(BasicBall ball)
    {
        data.basicBallCount--;
        base.Release(ball);
    }
}
