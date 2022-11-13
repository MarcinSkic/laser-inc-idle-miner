using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    private static FloatingText popupText;
    private static GameObject parent;

    public static void Initialize()
    {
        if (!parent)
        {
            parent = GameObject.Find("Canvas/PopupTexts");
        }
        if (!popupText)
        {
            popupText = Resources.Load<FloatingText>("Prefabs/PopupTextParent");
        }
    }

    public static void CreateFloatingText(string text, Transform location)
    {
        Initialize();
        FloatingText instance = Instantiate(popupText);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);
        instance.transform.SetParent(parent.transform, false);
        instance.transform.position = screenPosition;
        instance.SetText(text);
    }
}
