using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    [SerializeField] private FloatingText popupText;
    [SerializeField] private Transform parent;

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

    public void CreateFloatingText(string text, Transform location)
    {
        FloatingText instance = Instantiate(popupText,parent);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);
        instance.transform.position = screenPosition;
        instance.SetText(text);
    }
}
