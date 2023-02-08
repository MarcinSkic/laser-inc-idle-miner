using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    [SerializeField] private FloatingText popupText;
    [SerializeField] private Transform parent;

    #region LEGACY
    public static FloatingTextController Instance;
    private void Awake()
    {
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

    public FloatingText CreateFloatingText(string text, Transform location)
    {
        FloatingText instance = Instantiate(popupText,parent);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);
        instance.transform.position = screenPosition;
        instance.SetText(text);
        instance.Init();
        return instance;
    }
    #endregion
}
