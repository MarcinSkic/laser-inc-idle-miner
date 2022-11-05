using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private Data data;
    public TMP_Text damageDisplay;
    public TMP_Text SpdDisplay;
    public TMP_Text ballCountDisplay;
    public TMP_Text enemyHpDisplay;

    private void Start()
    {
        SetDamageDisplay();
        SetSpdDisplay();
        SetBallCountDisplay();
        SetWaveDisplay();
    }

    public void SetDamageDisplay()
    {
        damageDisplay.text = $"bullet damage: {data.GetBulletDamage()}";
    }
    
    public void SetSpdDisplay()
    {
        SpdDisplay.text = $"bullet speed: {data.bulletSpeed}";
    }

    public void SetBallCountDisplay()
    {
        ballCountDisplay.text = $"balls: {data.basicBulletCount}, {data.bombBulletCount}, {data.sniperBulletCount}";
    }

    public void SetWaveDisplay()
    {
        enemyHpDisplay.text = $"block hp: {Math.Round(data.GetWaveEnemiesHealth(),2)}";
    }
}
