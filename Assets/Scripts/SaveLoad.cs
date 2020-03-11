using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{
    public static List<SaveFile> savedGames = new List<SaveFile>();

    //it's static so we can call it from anywhere
    public static void Save()
    {
        SaveLoad.savedGames.Add(SaveFile.current);
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located
        FileStream file = File.Create(Application.dataPath + "/save.corpse"); //you can call it anything you want
        bf.Serialize(file, SaveLoad.savedGames);
        file.Close();
    }

    public static void Load()
    {
        if (File.Exists(Application.dataPath + "/save.corpse"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/save.corpse", FileMode.Open);
            SaveLoad.savedGames = (List<SaveFile>)bf.Deserialize(file);
            file.Close();
        }
    }
}