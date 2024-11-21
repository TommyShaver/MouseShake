using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveData
{
    private static SaveLoadData _saveData = new SaveLoadData();

    [System.Serializable]
    public struct SaveLoadData
    {
       public SceneRecords sceneRecords;
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/Message" + ".json";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true));
    }

    private static void HandleSaveData()
    {
        MosueMovement.Instance.Save(ref _saveData.sceneRecords);
    }
    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());
        _saveData = JsonUtility.FromJson<SaveLoadData>(saveContent);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        MosueMovement.Instance.Load(ref _saveData.sceneRecords);
    }
}