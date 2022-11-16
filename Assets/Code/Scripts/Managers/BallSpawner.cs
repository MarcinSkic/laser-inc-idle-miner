using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BallSpawner<T> : BaseSpawner<T> where T : BasicBall, IPoolable<T>
{
    [Space(5)]
    [Header("BALL SPAWNER")]
    [SerializeField] protected Vector2 spawnArea;   //TODO: Make it universal for all balls in model
    //TODO: Also make instantionsParent also universal for all balls

    [Header("TEMP")]
    [SerializeField] protected Data data;
    [SerializeField] protected GameController gameController;

    public override T Spawn()
    {
        T ball = base.Spawn();
        ball.transform.position = new Vector3(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y), 0);
        return ball;
    }

    protected override void Get(T ball)
    {
        ball.InitBall();
        base.Get(ball);
    }

    protected override T Create()
    {
        T ball = base.Create();
        ball.gameController = gameController;   //TODO: Remove when done architecture
        return ball;
    }
}
