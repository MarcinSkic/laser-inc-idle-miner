using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BallSpawner : MonoBehaviour
{
    public enum BallType { Basic, Bomb, Sniper}

    [SerializeField] private BasicBall basicPrefab;
    [SerializeField] private BombBall bombPrefab;
    [SerializeField] private SniperBall sniperPrefab;

    [SerializeField] private Transform ballsParent;
    [SerializeField] private Vector2 spawnArea;

    [Header("DEBUG")]
    [Header("Read Only")]
    [SerializeField] private int active;
    [SerializeField] private int inactive;

    [Header("TEMP")]
    [SerializeField] private Data data;
    [SerializeField] private GameController gameController;

    private ObjectPool<BasicBall> pool;
    private BasicBall ballToSpawn;

    private void Awake()
    {
        pool = new ObjectPool<BasicBall>(CreateBall, BallGet, BallRelease);
    }

    public void Update()
    {
        active = pool.CountActive;
        inactive = pool.CountInactive;
    }

    public void SpawnBall(BallType ballType)
    {
        switch (ballType)
        {
            case BallType.Basic:
                data.basicBallCount++;
                ballToSpawn = basicPrefab;
                break;
            case BallType.Bomb:
                ballToSpawn = bombPrefab;
                data.bombBallCount++;
                break;
            case BallType.Sniper:
                ballToSpawn = sniperPrefab;
                data.sniperBallCount++;
                break;
        }

        var ball = pool.Get();
        ball.transform.position = new Vector3(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y), 0);
    }

    private BasicBall CreateBall()
    {
        var ball = Instantiate(ballToSpawn, ballsParent);
        ball.gameController = gameController;
        ball.pool = pool;
        ball.data = data;

        return ball;
    }

    private void BallGet(BasicBall ball)
    {
        ball.gameObject.SetActive(true);
    }

    private void BallRelease(BasicBall ball)
    {
        ball.gameObject.SetActive(false);
    }
}
