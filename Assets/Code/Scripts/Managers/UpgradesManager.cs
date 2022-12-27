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
                case "Basic":
                    {   
                        basicBallSpawner.Spawn(out var ball);
                        model.GetUpgrade("UniversalSpeed").onValueUpdate += ball.Upgrade;
                        model.GetUpgrade("BasicSpeed").onValueUpdate += ball.Upgrade;
                        upgrade.onValueUpdate.Invoke(basicBallSpawner.active.ToString()); //TODO-FT-CURRENT: Pull from Data?;
                        break;
                    }
                case "Bomb":
                    {
                        bombBallSpawner.Spawn(out var ball);
                        model.GetUpgrade("UniversalSpeed").onValueUpdate += ball.Upgrade;
                        model.GetUpgrade("BombSpeed").onValueUpdate += ball.Upgrade;
                        ball.SetVariables(blocksParent);
                        upgrade.onValueUpdate.Invoke(bombBallSpawner.active.ToString()); //TODO-FT-CURRENT: Pull from Data?;
                        break;
                    }   
                case "Sniper":
                    {
                        sniperBallSpawner.Spawn(out var ball);
                        model.GetUpgrade("UniversalSpeed").onValueUpdate += ball.Upgrade;
                        model.GetUpgrade("SniperSpeed").onValueUpdate += ball.Upgrade;
                        ball.SetVariables(blocksParent);
                        upgrade.onValueUpdate.Invoke(sniperBallSpawner.active.ToString()); //TODO-FT-CURRENT: Pull from Data?;
                        break;
                    }
                default:
                    Debug.LogWarningFormat("Missing ball of type {0} to spawn ", ballType);
                    break;
            }
        }
    }

    private void SetFirstUpgradeButtonValue(Upgrade upgrade)
    {
        if(upgrade.specifiedObjects.Count == 1 && upgrade.upgradedValuesNames.Count == 1 && upgrade.upgradedObjects == UpgradedObjects.SpecifiedBalls)
        {
            var value = GetValueByName(upgrade.upgradedValuesNames[0], data.ballsData.Find(ball => ball.name == upgrade.specifiedObjects[0]));
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
