using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : BaseController<MenuView>
{

    private void Start()
    {
        Activate();
    }

    public override void Activate()
    {
        base.Activate();
        view.GameStartButton.Init();
        view.GameStartButton.onClick += StartGame;
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
}
