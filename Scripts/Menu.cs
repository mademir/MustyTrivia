using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject ContinueBtn;
    public GameObject RestartBtn;
    public GameObject ExitBtn;
    public GameObject LevelBtn;
    public GameObject MenuAll;
    public GameObject UI;
    public bool restart;

    public void Continue()
    {
        UI.GetComponent<Canvas>().planeDistance = 43;
        MenuAll.SetActive(false);
    }
    public void Restart() => restart = true;
    public void Exit() => Application.Quit();
    public void Level() {
        try
        {
            NetworkClient.localPlayer.GetComponent<MultPlayerController>().CmdSetLevel((int)LevelBtn.GetComponent<Slider>().value);
            LevelBtn.transform.GetChild(3).GetComponent<Text>().text = "Level: " + ((int)LevelBtn.GetComponent<Slider>().value).ToString();
        }
        catch { }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuAll.SetActive(!MenuAll.activeSelf); //Toggle menu
            UI.GetComponent<Canvas>().planeDistance = (MenuAll.activeSelf) ? 15 : 43;
        }
    }

}
