using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMessagePopup : MonoBehaviour
{
    [SerializeField] private UIButtonController[] closingButtons;
    [SerializeField] private GameObject iconObject;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;

    #region Singleton
    public static UIMessagePopup Instance;
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
        foreach(var btn in closingButtons)
        {
            btn.onClick += DisablePopup;
        }
    }

    public void DisplayMessage(string title, string value, Sprite icon = null, float delay = 5f)
    {
        gameObject.SetActive(true);

        this.title.text = title;
        description.text = value;
        if(icon != null)
        {
            this.icon.sprite = icon;
        } else
        {
            iconObject.SetActive(false);
        }

        StopAllCoroutines();
        if(delay > 0f)
        {
            StartCoroutine(Timer(delay));
        }
    }

    private IEnumerator Timer(float delay)
    {
        yield return new WaitForSeconds(delay);

        DisablePopup();
    }

    private void DisablePopup()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
