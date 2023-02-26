using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextSpawner : BaseSpawner<FloatingText>
{
    #region Singleton
    public static FloatingTextSpawner Instance;
    public List<FloatingText> floatingTexts;
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
    public void ClearBeforePrestige()
    {
        floatingTexts.Clear();
    }

    public void SpawnDefault(string text, Transform location)
    {
        Spawn(out FloatingText floatingText);
        floatingText.SetText(text);
        floatingText.Init(location.position);
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
