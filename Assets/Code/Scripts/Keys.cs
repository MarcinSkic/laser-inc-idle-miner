using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Here you store any global and constant variable, example usage: PlayerPrefs.GetString(Keys.[Key]);
/// </summary>
public static class Keys
{

}

[Flags]
public enum UpgradeableObjects {
    BasicBall = 0b_0000_0000_0001, 
    SniperBall = 0b_0000_0000_0010, 
    BombBall = 0b_0000_0000_0100, 
    AllBalls = BasicBall | SniperBall | BombBall,

    Miner = 0b_0001_0000_0000
}

[Flags]
public enum UpgradeableValues
{
    Speed = 0b0001,
    Damage = 0b0010,
    Special = 0b0100
}