using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
  [SerializeField]
  private Button _backBtn;
  [SerializeField]
  private Sprite _mutedSprite;
  [SerializeField]
  private Sprite _volumeSprite;
  [SerializeField]
  private Image _mutedBtnImg;
  [SerializeField]
  private AudioSource _bgMusic;
  [SerializeField]
  private GameObject _pages;

  private GameObject _currentPage;
  private LogScroller _logScroller;
  private bool _muted;
  private Stack<GameObject> _pagesStack = new Stack<GameObject>();

  private void Awake () {
    _logScroller = transform.root.GetComponent<UIState>().logScroller;

    _muted = PlayerPrefs.GetInt("muted", 0) > 0 ? true : false;
    _SetMuteBtnIcon();

    // deactivate all pages and set first page as active
    foreach(Transform child in _pages.transform) {
      child.gameObject.SetActive(false);
    }
    _currentPage = _pages.transform.GetChild(0).gameObject;
    _currentPage.SetActive(true);
  }

  public void NavToPage (string pageName) {
    GameObject target = _FindChild(_pages.transform, pageName);
    // _LogText(pageName + " " + !!target);
    if (target != null) {
      _currentPage.SetActive(false);
      _pagesStack.Push(_currentPage);
      _currentPage = target;
      target.SetActive(true);
      _backBtn.interactable = true;
    } else {
      _LogText("ERR: Missing page '" + pageName + "'");
    }
  }
  public void OnBackBtnClick () {
    if (_pagesStack.Count > 0) {
      _currentPage.SetActive(false);
      GameObject target = _pagesStack.Pop();
      _currentPage = target;
      target.SetActive(true);

      if (_pagesStack.Count == 0) {
        _backBtn.interactable = false;
      }
    } else {
      _LogText("ERR: On last page?");
    }
  }
  public void OnToggleMuteBtnClick () {
    _muted = !_muted;
    PlayerPrefs.SetInt("muted", _muted ? 1 : 0);
    _SetMuteBtnIcon();
  }

  // private
  private GameObject _FindChild (Transform target, string name) {
    foreach (Transform child in target) {
      if (child.gameObject.name == name) {
        return child.gameObject;
      }
    }
    return null;
  }
  private void _LogText (string text) {
    _logScroller.Log(text);
  }
  private void _SetMuteBtnIcon () {
    Sprite targetSprite = _muted ? _mutedSprite : _volumeSprite;

    _mutedBtnImg.sprite = targetSprite;
    _bgMusic.mute = _muted;
  }
}
