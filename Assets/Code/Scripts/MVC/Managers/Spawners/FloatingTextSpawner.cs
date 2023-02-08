using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextSpawner : BaseSpawner<FloatingText>
{
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
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    public void SpawnDefault(string text, Transform location)
    {
        Spawn(out FloatingText floatingText);
        floatingText.SetText(text);
        floatingText.Init(location.position);
    }
}
