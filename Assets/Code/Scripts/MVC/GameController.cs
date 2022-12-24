using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static System.Math;
using UnityEngine.Events;

using System;

/// <summary>
/// Methods from this object should not be called by other objects. When such action direction is needed (for example UI or world events) it should connect methods to events HERE.
/// </summary>
public class GameController : BaseController<GameView>
{
    [Header("Models")]
    [SerializeField] private Data data;
    [SerializeField] private UpgradesModel upgradesModel;
    [SerializeField] private ResourcesModel resourcesModel;

    [Header("Managers")]
    [SerializeField] private BlockSpawner blockSpawner;
    [SerializeField] private ResourcesManager resourcesManager;
    [SerializeField] private OfflineManager offlineManager;

    [Header("Other")]   //Probably if you feel of putting something here, then it should have its own manager. This script should interact only with managers and his UI 
    [SerializeField] private Transform _dynamic_blocks;   //TODO-FT-ARCHITECTURE
    [SerializeField] private GameObject movingBorderTexturesParent;

    private bool isMoving = true;

    //KEEP MONOBEHAVIOUR METHODS (Start, Update etc.) ON TOP
    /// <summary>
    /// This Start should be considered root Start of game, all inits where order of operations is important should originate from here
    /// </summary>
    void Start()
    {
        CreateBallBars();

        ConnectToUpgradesEvents();  //TODO-FT-CURRENT: Move this functionality to upgrade manager?
        ConnectToResourceManagerEvents();
        ConnectToOfflineManagerEvents();
        ConnectToViewButtons();

        view.InitBottomButtonsEvent();
        ConnectBallBarsWithEvents();

        //------TEMP------
        //  |   |   |   |
        //  v   v   v   v
        if (data.debugSettings)
        {
            resourcesManager.Money = data.additionalStartingMoney;   //TODO-FT-RESOURCES
            data.roundNumber += data.additionalStartingRound;
        }
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        DisplayFPS();
        

        var blocks = _dynamic_blocks.GetComponentsInChildren<BasicBlock>(false); //TODO: Very Temp
        MoveBlocks(blocks); // TODO: not optimal
        CheckIfWaveFinished(blocks); // TODO: not optimal

        DebugInputSwitch();
    }

    private void DebugInputSwitch()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            view.debugWindow.SetActive(!view.debugWindow.activeSelf);

            if (view.debugWindow.activeSelf)
            {
                Time.timeScale = data.timeScale;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }

    public void ConnectToResourceManagerEvents()
    {
        resourcesManager.onMoneyChange += view.SetMoneyDisplay;
    }

    private void ConnectToOfflineManagerEvents()
    {
        offlineManager.onReturnFromOffline += OnReturnFromOffline;
    }

    private void ConnectToViewButtons()
    {
        view.InitButtons();

        view.offlineConfirmButton.onClick += delegate {
            resourcesManager.IncreaseMoney(resourcesModel.offlineMoney);
            view.offlineGetBonusButton.Activate();
            view.offlinePopup.SetActive(false); 
        };

        view.offlineGetBonusButton.onClick += delegate {
            //TODO-FEATURE: Play AD, and do bonus if successful
            view.offlineGetBonusButton.Deactivate();
            resourcesModel.offlineMoney *= 2;
            view.SetOfflineMoney(resourcesModel.offlineMoney); 
        };
    }

    private void OnReturnFromOffline(double seconds)
    {
        resourcesModel.offlineMoney = 420*seconds;    //TODO-FEATURE: CalculateOfflineMoney()
        view.ShowOfflineTimePopup(seconds, resourcesModel.offlineMoney);
    }

    private void CreateBallBars()
    {
        foreach(var ballType in data.ballsData)
        {
            view.CreateBallBar(ballType);
        }
    }

    private void ConnectBallBarsWithEvents()
    {
        foreach(var ballBar in view.ballBars)
        {
            foreach(var buttonUpgrade in ballBar.buttonUpgrades)
            {
                var upgrade = upgradesModel.getUpgrade(buttonUpgrade.upgradeName);
                if(upgrade != null)
                {
                    buttonUpgrade.Init();
                    buttonUpgrade.onClick += upgrade.TryUpgrade;    //Button -> Tries to upgrade an Upgrade

                    buttonUpgrade.SetUpgradeCost(upgrade);   //Set initial values
                    resourcesManager.onMoneyChange += buttonUpgrade.ChangeStateBasedOnMoney;

                    upgrade.AddDoUpgrade(buttonUpgrade.SetUpgradeCost);  //Upgrade -> Updates Button values
                    upgrade.onValueUpdate += buttonUpgrade.SetUpgradeValue;  //TODO-FUTURE-BUG: There should be check if the button uses upgrade internal value or universal value, if universal then it should connect to not yet existing system of sending event on value change
                } 
                else
                {
                    Debug.LogWarningFormat("ButtonUpgrade needs: \"{0}\" upgrade, but it couldn't find it in upgrades collection.\n " +
                        "Maybe UpgradesModel misses scriptable upgrade instance or there is a typo in upgrade name?",buttonUpgrade.upgradeName);
                }   
            }
        }
    }

    private void ConnectToUpgradesEvents()
    {
        foreach(var upgrade in upgradesModel.upgrades)
        {
            upgrade.AddOnTryUpgrade(TryBuyUpgrade);
        }
    }

    private void MoveBlocks(BasicBlock[] blocks)
    {
        bool condition;
        if (isMoving)
        {
            condition = CheckForBlocksAboveY(blocks, 5f);
        } else
        {
            condition = CheckForBlocksAboveY(blocks, 4.5f);
        }

        if (!condition)
        {
            foreach (BasicBlock block in blocks)
            {
                block.transform.position += new Vector3(0, 5.0f, 0) * Time.deltaTime; // TODO: temp
            }
            var bgTextures = movingBorderTexturesParent.GetComponentsInChildren<Transform>(false); //TODO: Very Temp
            for (int i=1; i< bgTextures.Length; i++) // i=1 żeby nie łapało parenta
            {
                // Translate byłby zły bo parent ma scale i rotation
                bgTextures[i].position += new Vector3(0, 5.0f, 0) * Time.deltaTime ; // TODO: temp
                if (bgTextures[i].position.y >= 28f)
                {
                    bgTextures[i].position -= new Vector3(0, 16.8f, 0);
                }
            }

            data.realDepth += 5f * Time.deltaTime;
            view.depthMeter.SetDepth(data.realDepth);

            isMoving = true;
        } 
        else
        {
            isMoving = false;
        }
    }

    bool CheckForBlocksAboveY(BasicBlock[] blocks, float y)
    {
        foreach (BasicBlock block in blocks)
        {
            if (block.transform.position.y > y)
            {
                return true;
            }
        }
        return false;
    }


    bool CheckForBlocksBelowY(BasicBlock[] blocks, float y = -21)
    {
        foreach (BasicBlock block in blocks)
        {
            if (block.transform.position.y < y)
            {
                return true;
            }
        }
        return false;
    }

    private void CheckIfWaveFinished(BasicBlock[] blocks)
    {
        if (!CheckForBlocksBelowY(blocks))
        {

            data.depth += data.depthPerWave;
            view.SetBlocksHpDisplay(data.GetDepthBlocksHealth());

            blockSpawner.SpawnBlockRow(out List <BasicBlock> spawnedBlocks);

            for (int i=0; i<spawnedBlocks.Count; i++)
            {
                spawnedBlocks[i].AssignEvents(OnBlockDestroyed);
            }
        }
    }

    private void OnBlockDestroyed(double money)
    {
        resourcesManager.IncreaseMoney(money);
        //TODO-FEATURE: Count destroyed blocks for upgrades/rewards

    }

    private int fpsRefreshCounter = 0;

    void DisplayFPS()
    {
        float current;
        if (fpsRefreshCounter == 0)
        {
            fpsRefreshCounter = 100;
            current = Time.frameCount / Time.time;
            view.avg_FpsDisplay.text = $"avg FPS: {Mathf.Round(current*10)/10f}";
            view.fpsDisplay.text = $"FPS: {Mathf.Round(1 / Time.deltaTime)}";
        } 
        else
        {
            fpsRefreshCounter--;
        }
    }

    public void TryBuyUpgrade(Upgrade upgrade)
    {
        if (!resourcesManager.TryDecreaseMoney(upgrade.cost))
        {
            return;
        }

        upgrade.DoUpgrade();
    }
}
