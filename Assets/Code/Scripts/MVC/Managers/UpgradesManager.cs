using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class UpgradesManager : MonoBehaviour
{
    [Header("Models")]
    [SerializeField]
    private UpgradesModel model;
    [SerializeField]
    private BallsModel ballsModel;

    [Header("Managers")]
    [SerializeField] private BasicBallSpawner basicBallSpawner;
    [SerializeField] private BombBallSpawner bombBallSpawner;
    [SerializeField] private SniperBallSpawner sniperBallSpawner;
    [SerializeField] private LeechBallSpawner leechBallSpawner;
    [SerializeField] private PoisonBallSpawner poisonBallSpawner;
    [SerializeField] private ShadowBallSpawner shadowBallSpawner;

    [SerializeField] private Transform blocksParent;

    private void Awake()
    {
        model.TransformScriptablesIntoUpgrades();
    }

    public void ProcessUpgrades()
    {
        foreach(var upgrade in model.upgrades.Values)
        {
            switch (upgrade.type)
            {
                case UpgradeType.ValuesUpgrade:
                    upgrade.AddDoUpgrade(OnValuesUpgrade);
                    SetFirstUpgradeButtonValue(upgrade);    //UI setup
                    break;
                case UpgradeType.SpawnUpgrade:
                    upgrade.AddDoUpgrade(OnSpawnUpgrade);
                    upgrade.onValueUpdate?.Invoke("0");  //UI setup
                    break;
            }
        }
    }

    private void OnValuesUpgrade(Upgrade upgrade)
    {
        switch (upgrade.upgradedObjects)
        {
            case <= UpgradeableObjects.AllBalls:

                foreach (var pair in ballsModel.ballsData) {
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
            var speedUpgrades = model.upgrades.Values.Where(upgrade => upgrade.upgradedValues.HasFlag(UpgradeableValues.Speed));

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.BasicBall))
            {
                basicBallSpawner.Spawn(out var ball);
                upgrade.onValueUpdate.Invoke(basicBallSpawner.active.ToString()); //TODO-FT-CURRENT: Pull from Data?;

                foreach (var speedUpgrade in speedUpgrades)
                {
                    speedUpgrade.onValueUpdate += ball.Upgrade;
                }
            }

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.BombBall))
            {
                bombBallSpawner.Spawn(out var ball);
                ball.SetVariables(blocksParent);
                upgrade.onValueUpdate.Invoke(bombBallSpawner.active.ToString());

                foreach (var speedUpgrade in speedUpgrades)
                {
                    speedUpgrade.onValueUpdate += ball.Upgrade;
                }
            }

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.SniperBall))
            {
                sniperBallSpawner.Spawn(out var ball);
                ball.SetVariables(blocksParent);
                upgrade.onValueUpdate.Invoke(sniperBallSpawner.active.ToString());

                foreach (var speedUpgrade in speedUpgrades)
                {
                    speedUpgrade.onValueUpdate += ball.Upgrade;
                }
            }

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.LeechBall))
            {
                leechBallSpawner.Spawn(out var ball);
                ball.SetVariables(blocksParent);
                upgrade.onValueUpdate.Invoke(leechBallSpawner.active.ToString());

                foreach (var speedUpgrade in speedUpgrades)
                {
                    speedUpgrade.onValueUpdate += ball.Upgrade;
                }
            }

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.PoisonBall))
            {
                poisonBallSpawner.Spawn(out var ball);
                // ball.SetVariables(blocksParent);
                upgrade.onValueUpdate.Invoke(poisonBallSpawner.active.ToString());

                foreach (var speedUpgrade in speedUpgrades)
                {
                    speedUpgrade.onValueUpdate += ball.Upgrade;
                }
            }

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.ShadowBall))
            {
                shadowBallSpawner.Spawn(out var ball);
                // ball.SetVariables(blocksParent);
                upgrade.onValueUpdate.Invoke(shadowBallSpawner.active.ToString());

                foreach (var speedUpgrade in speedUpgrades)
                {
                    speedUpgrade.onValueUpdate += ball.Upgrade;
                }
            }
        } 
        else
        {
            Debug.LogWarningFormat("Missing ball of type {0} to spawn ", upgrade.upgradedObjects);
        }
    }

    private void SetFirstUpgradeButtonValue(Upgrade upgrade)
    {
        if((upgrade.upgradedObjects <= UpgradeableObjects.AllBalls && ((int)upgrade.upgradedObjects % 2 == 0 || upgrade.upgradedObjects == UpgradeableObjects.BasicBall)) && ((int)upgrade.upgradedValues % 2 == 0 || upgrade.upgradedValues == UpgradeableValues.Speed))
        {
            var value = GetValueByType(upgrade.upgradedValues, ballsModel.ballsData[upgrade.upgradedObjects]);
            upgrade.onValueUpdate.Invoke(value.value.ToString());
        } 
        else
        {
            upgrade.onValueUpdate?.Invoke(upgrade.upgradeValue.ToString());
        }
    }

    private UpgradeableData<double> GetValueByType(UpgradeableValues type, BallData ball)
    {
        return ball.values[type];
    }

    private void UpgradeBall(Upgrade upgrade, BallData ball)
    {
        foreach(var pair in ball.values)
        {
            if (upgrade.upgradedValues.HasFlag(pair.Key))
            {
                UpgradeValue(upgrade, pair.Value);
            }
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

    public void SavePersistentData(PersistentData data)
    {
        data.upgrades = model.upgrades.Values.ToArray();
    }

    public void LoadPersistentData(PersistentData data)
    {
        if (data.upgrades != null)
        {
            foreach (var upgrade in data.upgrades)
            {
                model.upgrades[upgrade.name] = upgrade;
            }
        }
    }
}
