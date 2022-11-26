using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradesManager : MonoBehaviour
{
    [SerializeField]
    private UpgradesModel model;
    [SerializeField]
    private Data data;

    [SerializeField] private BasicBallSpawner basicBallSpawner;
    [SerializeField] private BombBallSpawner bombBallSpawner;
    [SerializeField] private SniperBallSpawner sniperBallSpawner;

    private void Start()
    {
        ProcessUpgrades();
    }

    private void ProcessUpgrades()
    {
        var upgrades = model.upgrades;
        foreach(var upgrade in upgrades)
        {
            switch (upgrade.type)
            {
                case UpgradeType.ValuesUpgrade:
                    upgrade.onUpgrade += OnValuesUpgrade;
                    break;
                case UpgradeType.SpawnUpgrade:
                    upgrade.onUpgrade += OnSpawnUpgrade;
                    break;
            }
        }
    }

    private void OnValuesUpgrade(Upgrade upgrade)
    {
        switch (upgrade.upgradedObjects)
        {
            case UpgradedObjects.AllBalls:
                foreach(var ballData in data.ballsData)
                {
                    UpgradeBall(upgrade, ballData);
                }
                break;

            case UpgradedObjects.SpecifiedBalls:
                foreach(var ballType in upgrade.specifiedObjects)
                {
                    UpgradeBall(upgrade, data.ballsData.Find(ball => ball.name == ballType));
                }
                break;
        }
    }

    private void OnSpawnUpgrade(Upgrade upgrade)
    {
        foreach(var ballType in upgrade.specifiedObjects)
        {

            switch (ballType)
            {
                case "basic":
                    basicBallSpawner.Spawn(out var ball);
                    model.getUpgrade("UniversalSpeed").AddOnUpgrade(ball.Upgrade);
                    break;
                case "bomb":
                    bombBallSpawner.Spawn(out var ball2);
                    model.getUpgrade("UniversalSpeed").AddOnUpgrade(ball2.Upgrade);
                    break;
                case "sniper":
                    sniperBallSpawner.Spawn(out var ball3);
                    model.getUpgrade("UniversalSpeed").AddOnUpgrade(ball3.Upgrade);
                    break;
            }
        }
    }

    private void UpgradeBall(Upgrade upgrade, BaseBallData ball)
    {
        foreach(var valueType in upgrade.upgradedValuesNames)
        {
            switch (valueType)  //TODO-FT-VALUES_INTO_DICTIONARY
            {
                case "speed":
                    UpgradeValue(upgrade, ball.speed);
                    break;
                case "damage":
                    UpgradeValue(upgrade, ball.damage);
                    break;
                default:
                    Debug.LogWarning(string.Format("Ball upgrade abort, missing case for {0} value",valueType));
                    break;
            }
        }
    }

    private void UpgradeValue(Upgrade upgrade, UpgradeableData<double> value)
    {
        switch (upgrade.formula)
        {
            case ValueUpgradeFormula.Add:
                value.value += upgrade.changeValue;
                break;
            case ValueUpgradeFormula.Multiply:
                value.value *= upgrade.changeValue;
                break;
        }
    }

    [ContextMenu("TestUpgrade")]
    void TestUpgrade()
    {
        /*model.basicBallCount.upgrade.TryUpgrade(out _);
        model.bombBallCount.upgrade.TryUpgrade(out _);
        model.sniperBallCount.upgrade.TryUpgrade(out _);*/
    }

    [ContextMenu("Speeeeeeeeeeeeed")]
    void UpgradeSpeed()
    {
        //model.universalSpeed.upgrade.TryUpgrade(out _);
    }
}