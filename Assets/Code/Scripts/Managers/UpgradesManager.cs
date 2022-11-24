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
        LoadStartDataUpgrades();
        AddUpgradeablesValues();
        AddUpgradeableActions();
    }

    //TODO-FT-SAVEFILE: Here it will be changed to check for saved progress
    void LoadStartDataUpgrades()
    {
        model.universalSpeed.LoadStartData();
        model.universalDamage.LoadStartData();
        model.basicBallCount.LoadStartData();
        model.bombBallCount.LoadStartData();
        model.sniperBallCount.LoadStartData();
    }

    void AddUpgradeablesValues()
    {
        //model.universalSpeed.upgrade.AddUpgradeable(data.basicBallData.speed, data.bombBallData.speed,data.sniperBallData.speed);
        //model.universalDamage.upgrade.AddUpgradeable(data.basicBallData.damage, data.bombBallData.damage, data.sniperBallData.damage);
    }

    void AddUpgradeableActions()
    {
        /*model.basicBallCount.upgrade.AddUpgradeable(new UpgradeableData<UnityAction>(delegate { 
            basicBallSpawner.Spawn(out var ball);
            model.universalSpeed.upgrade.AddOnUpgrade(ball.Upgrade);
        }));
        model.bombBallCount.upgrade.AddUpgradeable(new UpgradeableData<UnityAction>(delegate {
            bombBallSpawner.Spawn(out var ball);
            model.universalSpeed.upgrade.AddOnUpgrade(ball.Upgrade);
        }));
        model.sniperBallCount.upgrade.AddUpgradeable(new UpgradeableData<UnityAction>(delegate {
            sniperBallSpawner.Spawn(out var ball);
            model.universalSpeed.upgrade.AddOnUpgrade(ball.Upgrade);
        }));*/
    }



    [ContextMenu("TestUpgrade")]
    void TestUpgrade()
    {
        model.basicBallCount.upgrade.TryUpgrade(out _);
        model.bombBallCount.upgrade.TryUpgrade(out _);
        model.sniperBallCount.upgrade.TryUpgrade(out _);
    }

    [ContextMenu("Speeeeeeeeeeeeed")]
    void UpgradeSpeed()
    {
        model.universalSpeed.upgrade.TryUpgrade(out _);
    }
}
