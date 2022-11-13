using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseMenuButton : MonoBehaviour
{
    public GameObject menu;
    public void ShowMenu() => GameObject.FindObjectOfType<GameController>().ShowMenu(menu);
}
