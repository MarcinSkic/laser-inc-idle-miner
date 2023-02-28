using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagesManager : MonoBehaviour
{
    [SerializeField] private UIMessagePopup messagePopup;

    #region Singleton
    public static MessagesManager Instance;
    private void Awake()
    {
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

    private void Start()
    {
        messagePopup.Init();
    }

    public void DisplayMessage(string title, string value, Sprite icon = null, Color? iconColor = null, float delay = 5f)
    {
        messagePopup.DisplayMessage(title, value, icon,iconColor, delay);
    }
}
