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

    [SerializeField] private Transform blocksParent;

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
                    upgrade.AddDoUpgrade(OnValuesUpgrade);
                    SetFirstUpgradeButtonValue(upgrade);    //UI setup
                    break;
                case UpgradeType.SpawnUpgrade:
                    upgrade.AddDoUpgrade(OnSpawnUpgrade);
                    upgrade.onValueUpdate.Invoke("0");  //UI setup
                    break;
            }
        }
    }

    private void OnValuesUpgrade(Upgrade upgrade)
    {
        switch (upgrade.upgradedObjects)
        {
            case <= UpgradeableObjects.AllBalls:

                foreach (var pair in data.ballsData) {
                    if (upgrade.upgradedObjects.HasFlag(pair.Key))
                    {
                        UpgradeBall(upgrade, pair.Value);
                    }
                }
                break;
        }
    }

    private void OnSpawnUpgrade(Upgrade upgrade)
    {
        if(upgrade.upgradedObjects <= UpgradeableObjects.AllBalls)
        {
            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.BasicBall))
            {
                basicBallSpawner.Spawn(out var ball);
                model.GetUpgrade("UniversalSpeed").onValueUpdate += ball.Upgrade;
                model.GetUpgrade("BasicSpeed").onValueUpdate += ball.Upgrade;
                upgrade.onValueUpdate.Invoke(basicBallSpawner.active.ToString()); //TODO-FT-CURRENT: Pull from Data?;
            }

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.BombBall))
            {
                bombBallSpawner.Spawn(out var ball);
                model.GetUpgrade("UniversalSpeed").onValueUpdate += ball.Upgrade;
                model.GetUpgrade("BombSpeed").onValueUpdate += ball.Upgrade;
                ball.SetVariables(blocksParent);
                upgrade.onValueUpdate.Invoke(bombBallSpawner.active.ToString());
            }

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.SniperBall))
            {
                sniperBallSpawner.Spawn(out var ball);
                model.GetUpgrade("UniversalSpeed").onValueUpdate += ball.Upgrade;
                model.GetUpgrade("SniperSpeed").onValueUpdate += ball.Upgrade;
                ball.SetVariables(blocksParent);
                upgrade.onValueUpdate.Invoke(sniperBallSpawner.active.ToString());
            }
        } 
        else
        {
            Debug.LogWarningFormat("Missing ball of type {0} to spawn ", upgrade.upgradedObjects);
        }
    }

    private void SetFirstUpgradeButtonValue(Upgrade upgrade)
    {
        if((upgrade.upgradedObjects <= UpgradeableObjects.AllBalls && ((int)upgrade.upgradedObjects % 2 == 0 || upgrade.upgradedObjects == UpgradeableObjects.BasicBall)) && upgrade.upgradedValuesNames.Count == 1)
        {
            var value = GetValueByName(upgrade.upgradedValuesNames[0], data.ballsData[upgrade.upgradedObjects]);
            upgrade.onValueUpdate.Invoke(value.value.ToString());
        } 
        else
        {
            upgrade.onValueUpdate?.Invoke(upgrade.upgradeValue.ToString());
        }
    }

    private UpgradeableData<double> GetValueByName(string name, BaseBallData ball)
    {
        switch (name)  //TODO-FT-DICTIONARIES
        {
            case "Speed":
                return ball.speed;
            case "Damage":
                return ball.damage;
            default:
                Debug.LogWarning(string.Format("Ball upgrade abort, missing case for {0} value", name));
                return null;
        }
    }

    private void UpgradeBall(Upgrade upgrade, BaseBallData ball)
    {
        foreach(var valueType in upgrade.upgradedValuesNames)
        {
            var value = GetValueByName(valueType, ball);
            if(value == null) return;
            UpgradeValue(upgrade, value);
        }
    }

    private void UpgradeValue(Upgrade upgrade, UpgradeableData<double> value)
    {
        switch (upgrade.formula)
        {
            case ValueUpgradeFormula.Add:
                value.value += upgrade.changeValue;
                upgrade.upgradeValue += upgrade.changeValue;
                break;
            case ValueUpgradeFormula.Multiply:
                value.value *= upgrade.changeValue;
                upgrade.upgradeValue *= upgrade.changeValue;
                break;
        }
        if (upgrade.onUpgradeButtonsShowUpgradeInternalValue)
        {
            upgrade.onValueUpdate.Invoke(string.Format("{0:f2}", upgrade.upgradeValue));
            //upgrade.onValueUpdate.Invoke(string.Format("{0:#.0e0}", upgrade.upgradeValue));
        } 
        else
        {
            upgrade.onValueUpdate.Invoke(string.Format("{0:f2}", value.value));
        }
    }
}
