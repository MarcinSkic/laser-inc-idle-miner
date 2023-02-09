using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

public class RewardBat : MonoBehaviour
{
    [SerializeField] float xDeviation;
    [SerializeField] float yBorders;
    [SerializeField] float verticalSpeed;
    [SerializeField] float horizontalSpeed;
    int direction;
    [SerializeField] SpriteRenderer batSprite;
    [SerializeField] BatOption[] batOptions;
    private BatOption batOption;

    void Start()
    {
        transform.position = new Vector3(Random.Range(-xDeviation, xDeviation), -yBorders, Random.Range(3f, 5f));
        direction = Random.Range(0, 2) * 2 - 1; // either 1 or -1
        handleDirectionChange();
        batOption = batOptions[Random.Range(0, batOptions.Length)];
        batSprite.sprite = batOption.batSprite;
    }

    void FixedUpdate()
    {
        transform.position += new Vector3(horizontalSpeed*direction, verticalSpeed, 0)*Time.deltaTime;
        if (Mathf.Abs(transform.position.x) >= xDeviation)
        {
            direction = -direction;
            handleDirectionChange();
        }
        if (transform.position.y > yBorders)
        {
            Destroy(gameObject);
        }
    }

    void handleDirectionChange()
    {
        batSprite.flipX = direction<0;
    }

    public void getClicked()
    {
        if (batOption.needsAd)
        {
            Debug.Log($"watch an ad to get {batOption.rewardType}");
        }
        Debug.Log($"you get {batOption.rewardType}");
        Destroy(gameObject);
    }
}
