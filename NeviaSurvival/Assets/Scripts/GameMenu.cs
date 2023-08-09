using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    UILinks ui;
    Links links;
    int cameraMode = 0;

    private void Start()
    {
        ui = FindObjectOfType<UILinks>();
        links = FindObjectOfType<Links>();
        ui.startMenuPanel.SetActive(true);

    }

    public void PlayGame()
    {
        MainMenu();
    }

    public void StartGame()
    {
        links.itemRandomizer.SpawnRandomItemsInStorages();
        links.player.gameObject.SetActive(true);
        links.ScottyCamera.gameObject.SetActive(false);
        ui.startMenuPanel.SetActive(false);
        ui.statusPanel.SetActive(true);
        if (isRandomSpawnPoint) links.player.RandomSpawnPoint();
        links.player.Death();
        links.questHandler.AddStartQuests();

    }

    public void ContinueGame()
    {
        links.player.isStart = false;
        links.dayNight.isLoadGame = true;
        links.saveLoad.LoadPlayer();
        
        ui.startMenuPanel.SetActive(false);
        ui.statusPanel.SetActive(true);
        
        links.player.ConfigureGame();
    }

    public void SaveGame()
    {
        links.saveLoad.SavePlayer();
    }

    public void ExitGame()
    {
        Application.Quit();
        //EditorApplication.ExitPlaymode();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public bool isRandomSpawnPoint;
    public void RandomSpawnPoint()
    {
        if (isRandomSpawnPoint) isRandomSpawnPoint = false;
        else isRandomSpawnPoint = true;
    }

    public void MainMenu()
    {
        if (ui.mainMenuPanel.activeSelf) { ui.mainMenuPanel.SetActive(false); links.mousePoint.isPointUI = false; }
        else { ui.mainMenuPanel.SetActive(true); }
    }

    public void AboutPanel()
    {
        if (ui.aboutPanel.activeSelf) { ui.aboutPanel.SetActive(false); links.mousePoint.isPointUI = false; }
        else ui.aboutPanel.SetActive(true);
    }

    public void InfoPanel()
    {
        if (ui.infoPanel.activeSelf) { ui.infoPanel.SetActive(false); links.mousePoint.isPointUI = false; }
        else ui.infoPanel.SetActive(true);
    }    

    public void QuestPanel()
    {
        if (ui.questPanel.activeSelf) { ui.questPanel.SetActive(false); links.mousePoint.isPointUI = false; }
        else ui.questPanel.SetActive(true);
    }    

    public void Equipment()
    {
        if (ui.equipmentPanel.activeSelf) { ui.equipmentPanel.SetActive(false); links.mousePoint.isPointUI = false; }
        else ui.equipmentPanel.SetActive(true);
    }   
    
    public void Backpack()
    {
        if (ui.inventoryPanel.activeSelf) { ui.inventoryPanel.SetActive(false); links.mousePoint.isPointUI = false; }
        else ui.inventoryPanel.SetActive(true);
    }

    public void Help()
    {
        if (ui.helpPanel.activeSelf) { ui.helpPanel.SetActive(false); links.mousePoint.isPointUI = false; }
        else ui.helpPanel.SetActive(true);
    }

    public void StatusPanel()
    {
        if (ui.statusPanel.activeSelf) { ui.statusPanel.SetActive(false); links.mousePoint.isPointUI = false; }
        else ui.statusPanel.SetActive(true);
    }

    public void BuildingMenu()
    {
        if (ui.buildingPanel.activeSelf) { ui.buildingPanel.SetActive(false); links.mousePoint.isPointUI = false; }
        else ui.buildingPanel.SetActive(true);
    }
    
    public void Map()
    {
        if (ui.mapPanel.activeSelf) { ui.mapPanel.SetActive(false); links.mousePoint.isPointUI = false; }
        else ui.mapPanel.SetActive(true);
    }

    void Update()
    {
        if (!links.player.isStart)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                BuildingMenu();
            }
            if (Input.GetKeyDown(KeyCode.CapsLock))
            {
                Map();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Equipment();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                Backpack();
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                StatusPanel();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                InfoPanel();
            }
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Help();
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                if (ui.fpsIndicator.gameObject.activeSelf) ui.fpsIndicator.gameObject.SetActive(false);
                else ui.fpsIndicator.gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                if (Time.timeScale == 1)
                {
                    Time.timeScale = 0;
                    ui.pauseText.SetActive(true);

                }
                else
                {
                    Time.timeScale = 1;
                    ui.pauseText.SetActive(false);
                }
            }
            
            if (Input.GetKeyDown(KeyCode.F4))
            {
                if (cameraMode == 0)
                {
                    links.cameraController.CameraMode2();
                    cameraMode = 1;
                }
                else if (cameraMode == 1)
                {
                    links.cameraController.CameraMode3();
                    links.PlayerMesh.SetActive(false);
                    cameraMode = 2;
                }
                else if (cameraMode == 2)
                {
                    links.cameraController.CameraMode1();
                    links.PlayerMesh.SetActive(true);
                    cameraMode = 0;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            QuestPanel();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MainMenu();
        }
    }
}
