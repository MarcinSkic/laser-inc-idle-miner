using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Here you store any global and constant variable, example usage: PlayerPrefs.GetString(Keys.[Key]);
/// </summary>
public static class Keys
{
    public const string SAVE_FILE_NAME = "save.dat";
}

[Flags]
public enum UpgradeableObjects {
    BasicBall = 0b_0000_0000_0001, 
    SniperBall = 0b_0000_0000_0010, 
    BombBall = 0b_0000_0000_0100,
    LeechBall = 0b_0000_0000_1000,
    PoisonBall = 0b_0000_0001_0000,
    ShadowBall = 0b_0000_0010_0000,
    AllBalls = BasicBall | SniperBall | BombBall | LeechBall | PoisonBall | ShadowBall,

    Miner = 0b_0001_0000_0000
}

[Flags]
public enum UpgradeableValues
{
    Speed = 0b0001,
    Damage = 0b0010,
    Special = 0b0100,
    ClickDamage = 0b1000,
}