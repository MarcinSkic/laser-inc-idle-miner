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

    [Header("TEMP")]
    [SerializeField] private Data data;
    [SerializeField] private GameController gameController;

    private ObjectPool<BasicBall> basicPool;
    private ObjectPool<BombBall> bombPool;
    private ObjectPool<SniperBall> sniperPool;

    private void Awake()
    {
        basicPool = new ObjectPool<BasicBall>(CreateBasic, GetBasic, ReleaseUniversal);
        bombPool = new ObjectPool<BombBall>(CreateBomb,GetBomb,ReleaseUniversal);
        sniperPool = new ObjectPool<SniperBall>(CreateSniper, GetSniper, ReleaseUniversal);
    }

    public void Update()
    {
        //active = basicPool.CountActive;
        //inactive = basicPool.CountInactive;
    }

    public void SpawnBall(BallType ballType)
    {
        BasicBall ball = null;
        switch (ballType)
        {
            case BallType.Basic:
                data.basicBallCount++;
                ball = basicPool.Get();
                break;
            case BallType.Bomb:
                ball = bombPool.Get();
                data.bombBallCount++;
                break;
            case BallType.Sniper:
                ball = sniperPool.Get();
                data.sniperBallCount++;
                break;
        }
        
        ball.transform.position = new Vector3(Random.Range(-spawnArea.x, spawnArea.x), Random.Range(-spawnArea.y, spawnArea.y), 0);
    }

    private BasicBall CreateBasic()
    {
        var ball = Instantiate(basicPrefab, ballsParent);
     
        return CreateUniversal(ball);
    }

    private BombBall CreateBomb()
    {
        var ball = Instantiate(bombPrefab, ballsParent);

        return (BombBall)CreateUniversal(ball);
    }

    private SniperBall CreateSniper()
    {
        var ball = Instantiate(sniperPrefab, ballsParent);

        return (SniperBall)CreateUniversal(ball);
    }

    private BasicBall CreateUniversal(BasicBall ball)
    {
        ball.gameController = gameController;
        ball.Pool = basicPool;

        return ball;
    }

    private void GetBasic(BasicBall ball)
    {
        ball.InitBall(data.GetSpd(), data.GetBulletDamage());
        GetUniversal(ball);
    }

    private void GetBomb(BombBall ball)
    {
        ball.InitBall(data.GetSpd(), data.GetBulletDamage(),data.explosionSize);
        GetUniversal(ball);
    }

    private void GetSniper(SniperBall ball)
    {
        ball.InitBall(data.GetSpd(), data.GetBulletDamage(),data.speedBoost);
        GetUniversal(ball);
    }

    private void GetUniversal(BasicBall ball)
    {
        ball.gameObject.SetActive(true);
    }

    private void ReleaseUniversal<T>(T ball) where T : BasicBall
    {
        ball.gameObject.SetActive(false);
    }
}
