using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public enum BatRewardType
{
    money,
    premium,
    powerup
}

[System.Serializable]
public class BatOption
{
    public Sprite batSprite;
    public BatRewardType rewardType;
    public bool needsAd;
    [Tooltip("seconds of boost/afkReward, amount of premium currency")]
    public int[] rewardAmount;
}

public class RewardBat : MonoBehaviour
{
    [SerializeField] float xDeviation;
    [SerializeField] float yBorders;
    [SerializeField] float verticalSpeed;
    [SerializeField] float horizontalSpeed;
    int direction;
    [SerializeField] SpriteRenderer batSprite;
    [SerializeField] GameObject batFBX;
    [SerializeField] float obrotPrawo;
    [SerializeField] float obrotLewo;
    [SerializeField] float rotationSpeed;
    [SerializeField] BatOption[] batOptions;
    [SerializeField] GameObject coinGO;
    [SerializeField] GameObject crystalGO;
    [SerializeField] GameObject sphereGO;
    private BatOption batOption;
    public ResourcesManager resourcesManager;
    public AdManager adManager;

    void Start()
    {
        transform.position = new Vector3(Random.Range(-xDeviation, xDeviation), -yBorders, Random.Range(-3f, -5f));
        direction = Random.Range(0, 2) * 2 - 1; // either 1 or -1
        //handleDirectionChange();
        batOption = batOptions[Random.Range(0, batOptions.Length)];
        switch (batOption.rewardType)
        {
            case BatRewardType.money:
                coinGO.SetActive(true);
                break;
            case BatRewardType.premium:
                crystalGO.SetActive(true);
                break;
            case BatRewardType.powerup:
                sphereGO.SetActive(true);
                break;
        }
        //batSprite.sprite = batOption.batSprite;
        AudioManager.Instance.Play("bat_warning");
    }

    void HandleBatRotation()
    {
        if (direction == 1 && (batFBX.transform.rotation.eulerAngles.y > obrotPrawo))
        {
            batFBX.transform.Rotate(new Vector3(0, -rotationSpeed, 0));
        }
        if (direction != 1 && (batFBX.transform.rotation.eulerAngles.y < obrotLewo))
        {
            batFBX.transform.Rotate(new Vector3(0, rotationSpeed, 0));
        }
    }

    void FixedUpdate()
    {
        transform.position += new Vector3(horizontalSpeed*direction, verticalSpeed, 0)*Time.deltaTime;
        if (Mathf.Abs(transform.position.x) >= xDeviation)
        {
            direction = -direction;
            //handleDirectionChange();
        }
        if (transform.position.y > yBorders)
        {
            Destroy(gameObject);
        }
        HandleBatRotation();
    }

    //void handleDirectionChange()
    //{
    //    batSprite.flipX = direction<0;
    //}

    public void getClicked()
    {
        string debugString = "";
        if (batOption.needsAd)
        {
            debugString += "watch an ad to get ";
        } else
        {
            debugString += "you get ";
        }
        int value = batOption.rewardAmount[Random.Range(0, batOption.rewardAmount.Length)];
        debugString += $"{value}";
        // TODO: connect to ads
        if (batOption.needsAd)
        {
            adManager.TryShowBatAd(batOption.rewardType, value);
        } else
        {
            switch (batOption.rewardType)
            {
                case BatRewardType.money:
                    debugString += " seconds worth of offlineReward";
                    resourcesManager.IncreaseMoneyForOfflineByTime(value);
                    AudioManager.Instance.Play("caught_coins");
                    break;
                case BatRewardType.powerup:
                    debugString += " seconds of double laser power";
                    resourcesManager.IncreasePowerUpTimeLeft(value);
                    AudioManager.Instance.Play("caught_p_up");
                    break;
                case BatRewardType.premium:
                    debugString += " premium curency";
                    resourcesManager.IncreasePremiumCurrency(value);
                    AudioManager.Instance.Play("caught_crystal");
                    break;
            }
        }
        Debug.Log(debugString);
        AudioManager.Instance.Play("bat_caught");
        Destroy(gameObject);
    }
}
