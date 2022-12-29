using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BallSpawner<Ball,Data> : BaseSpawner<Ball> where Ball : BaseBall<Data>, IPoolable<Ball> where Data : BallData
{
    [Space(5)]
    [Header("BALL SPAWNER")]
    [SerializeField] protected Vector2 spawnArea;   //TODO: Make it universal for all balls in model
    //TODO: Also make instantionsParent also universal for all balls

    [Header("TEMP")]
    [SerializeField] protected global::Data data;

    public override void Spawn(out Ball spawnedBall)
    {
       base.Spawn(out Ball ball);
       ball.transform.position = new Vector3(0, 6, 0) + new Vector3(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y), 0);
       spawnedBall = ball;
    }

    public virtual void Spawn()
    {
        Spawn(out _);
    }

    protected override void Get(Ball ball)
    {
        ball.InitBall();
        base.Get(ball);
    }

    protected override Ball Create()
    {
        Ball ball = base.Create();
        return ball;
    }
}
