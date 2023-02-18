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

    public void SetupUpgrades()
    {
        foreach(var upgrade in model.upgrades.Values)
        {
            switch (upgrade.type)
            {
                case UpgradeType.ValuesUpgrade:
                    upgrade.doUpgrade += OnValuesUpgrade;
                    SetFirstUpgradeButtonValue(upgrade);    //UI setup
                    break;
                case UpgradeType.SpawnUpgrade:
                    upgrade.doUpgrade += OnSpawnUpgrade;
                    upgrade.onValueUpdate?.Invoke("0");  //UI setup
                    break;
            }

            if (!upgrade.isUnlocked)    //Maybe set true by LoadPersistentData or scriptable in future 
            {
                upgrade.isUnlocked = true;  //If no requirements then unlocked...

                if (!SettingsModel.Instance.removeUpgradesRequirements)
                {
                    foreach (var requirement in upgrade.requirements)
                    {
                        upgrade.isUnlocked = false; //...but if there are requirements then locked
                        RequirementsManager.Instance.ConnectRequirementToValueEvent(requirement);
                        requirement.onStateChanged += upgrade.CheckIfUnlocked;

                        upgrade.leftRequirements++;
                    }
                }           

                upgrade.onUnlock += OnUpgradeUnlocked;
            }

        }
    }

    public void SetInitialUpgradesValues()
    {
        foreach (var upgrade in model.upgrades.Values)
        {
            switch (upgrade.type)
            {
                case UpgradeType.ValuesUpgrade:
                    SetFirstUpgradeButtonValue(upgrade);    //UI setup
                    break;
                case UpgradeType.SpawnUpgrade:
                    upgrade.onValueUpdate?.Invoke("0");  //UI setup
                    break;
            }
        }
    }

    public void ExecuteLoadedUpgrades()
    {
        foreach(var upgrade in model.upgrades.Values)
        {
            for(int i = 0; i < upgrade.currentLevel; i++)
            {
                upgrade.DoLoadedUpgrade();
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
                upgrade.onValueUpdate?.Invoke(basicBallSpawner.active.ToString()); //TODO-UGLY: Should be from BallsModel like: public int basicBallCount => basicBallSpawner.active 

                foreach (var speedUpgrade in speedUpgrades)
                {
                    speedUpgrade.onValueUpdate += ball.Upgrade;
                }
            }

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.BombBall))
            {
                bombBallSpawner.Spawn(out var ball);
                ball.SetVariables(blocksParent);
                upgrade.onValueUpdate?.Invoke(bombBallSpawner.active.ToString());

                foreach (var speedUpgrade in speedUpgrades)
                {
                    speedUpgrade.onValueUpdate += ball.Upgrade;
                }
            }

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.SniperBall))
            {
                sniperBallSpawner.Spawn(out var ball);
                ball.SetVariables(blocksParent);
                upgrade.onValueUpdate?.Invoke(sniperBallSpawner.active.ToString());

                foreach (var speedUpgrade in speedUpgrades)
                {
                    speedUpgrade.onValueUpdate += ball.Upgrade;
                }
            }

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.LeechBall))
            {
                leechBallSpawner.Spawn(out var ball);
                upgrade.onValueUpdate?.Invoke(leechBallSpawner.active.ToString());

                foreach (var speedUpgrade in speedUpgrades)
                {
                    speedUpgrade.onValueUpdate += ball.Upgrade;
                }
            }

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.PoisonBall))
            {
                poisonBallSpawner.Spawn(out var ball);
                upgrade.onValueUpdate?.Invoke(poisonBallSpawner.active.ToString());

                foreach (var speedUpgrade in speedUpgrades)
                {
                    speedUpgrade.onValueUpdate += ball.Upgrade;
                }
            }

            if (upgrade.upgradedObjects.HasFlag(UpgradeableObjects.ShadowBall))
            {
                shadowBallSpawner.Spawn(out var ball);
                upgrade.onValueUpdate?.Invoke(shadowBallSpawner.active.ToString());

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
            upgrade.onValueUpdate?.Invoke(value.value.ToString());
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
            upgrade.onValueUpdate?.Invoke(NumberFormatter.Format(upgrade.upgradeValue));
            //upgrade.onValueUpdate.Invoke(string.Format("{0:#.0e0}", upgrade.upgradeValue));
        } 
        else
        {
            upgrade.onValueUpdate?.Invoke(NumberFormatter.Format(value.value));
        }
    }

    private void OnUpgradeUnlocked(Upgrade upgrade)
    {
        Debug.Log($"Unlocked: Title \"{upgrade.title}\" | Name \"{upgrade.name}\"");

        foreach(var requirement in upgrade.requirements)
        {
            RequirementsManager.Instance.DisconnectRequirementFromValueEvent(requirement);
            requirement.onStateChanged -= upgrade.CheckIfUnlocked;
        }

        upgrade.onUnlock = null;
    }

    public void SavePersistentData(PersistentData data)
    {
        data.upgrades = model.upgrades.Values.Select(upgrade => new PersistentUpgrade(upgrade.name, upgrade.currentLevel, upgrade.isUnlocked)).ToArray();
    }

    public void LoadPersistentData(PersistentData data)
    {
        if (data.upgrades != null)
        {
            foreach (var persistentUpgrade in data.upgrades)
            {
                if (model.upgrades.ContainsKey(persistentUpgrade.name))
                {
                    model.upgrades[persistentUpgrade.name].currentLevel = persistentUpgrade.currentLevel;
                    model.upgrades[persistentUpgrade.name].isUnlocked = persistentUpgrade.isUnlocked;
                } 
                else
                {
                    Debug.LogWarning($"Can't load data of {persistentUpgrade.name}, there is no such upgrade in dictionary. On game pause/close this data will be overwritten!");
                }
            }
        }
    }
}
