using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MessagesManager : MonoBehaviour
{
    [SerializeField] private UIMessagePopup messagePopup;
    [SerializeField] private UIConfirmPopup confirmPopup;

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
        confirmPopup.Init();
    }

    public void DisplayMessage(string title, string value, Sprite icon = null, Color? iconColor = null, float delay = 5f)
    {
        messagePopup.DisplayMessage(title, value, icon,iconColor, delay);
    }

    public void DisplayConfirmQuestion(string title, string description, UnityAction confirmAction,  UnityAction cancelAction = null)
    {
        confirmPopup.Display(title,description);
        confirmPopup.onResponse += r =>
        {
            if (r)
            {
                confirmAction?.Invoke();
            }
            else
            {
                cancelAction?.Invoke();
            }
        };
    }
}
