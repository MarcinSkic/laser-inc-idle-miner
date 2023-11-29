using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IconType { PowerUp, Money, PremiumCurrency }

public class FloatingTextSpawner : BaseSpawner<FloatingText>
{
    [Header("FLOATING TEXT SPAWNER")]
    #region Singleton
    public static FloatingTextSpawner Instance;

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion
    public List<FloatingText> floatingTexts;
    public bool disableSpawning = false;

    [Header("Icons")]
    [SerializeField] private Sprite powerUpIcon;
    [SerializeField] private Sprite moneyIcon;
    [SerializeField] private Sprite premiumCurrencyIcon;

    [Header("Scales")]
    public float batRewardScale = 3f;
    public void ClearBeforePrestige()
    {
        floatingTexts.Clear();
    }

    public void DisableHanging()
    {
        floatingTexts.ForEach(f => f.ObjectPosition = new Vector3(1000, 0, 1000));
    }

    public void SpawnDefault(string text, Transform location, float scale=1f, IconType? iconType=null)
    {
        Spawn(out FloatingText floatingText);
        floatingText.SetText(text);
        floatingText.SetScale(scale);
        floatingText.Init(location.position);

        var iconScale = Vector3.one;
        var icon = iconType switch
        {
            IconType.PowerUp => powerUpIcon,
            IconType.Money => moneyIcon,
            IconType.PremiumCurrency => premiumCurrencyIcon,
            _ => null
        };

        if(icon == powerUpIcon)
        {
            iconScale = new Vector3(1.5f, 1.5f, 1);
        }

        floatingText.SetIcon(icon,iconScale);
    }

    protected override void Get(FloatingText element)
    {
        floatingTexts.Add(element);
        base.Get(element);
    }

    protected override void Release(FloatingText element)
    {
        base.Release(element);
        floatingTexts.Remove(element);
    }
}
