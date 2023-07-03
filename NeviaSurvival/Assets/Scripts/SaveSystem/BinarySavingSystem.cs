using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class BinarySavingSystem
{
    public static void SavePlayer(Player player)
    {
        SavingProcess(player);
            
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/nevia.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static async void SavingProcess(Player player)
    {
        player.links.ui.mainMenuPanel.SetActive(false);
        Time.timeScale = 0.1f;
        player.links.ui.savingGameText.SetActive(true);
        await UniTask.Delay(100);
        Time.timeScale = 1;
        player.links.ui.savingGameText.SetActive(false);
        player.links.ui.mainMenuPanel.SetActive(true);
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/nevia.save";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found at " + path);
            return null;
        }
    }
}
