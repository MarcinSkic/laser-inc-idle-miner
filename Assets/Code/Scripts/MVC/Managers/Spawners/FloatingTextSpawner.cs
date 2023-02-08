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
        Spawn(out FloatingText floatingText, location);
        floatingText.SetText(text);
        floatingText.Init();
    }

    public void Spawn(out FloatingText floatingText, Transform location)
    {
        base.Spawn(out FloatingText text);

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);
        text.transform.position = screenPosition;

        floatingText = text;
    }
}
