using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SerializableTypes;
using Random = System.Random;
using System.Linq;
using Helpers;

[System.Serializable]
public class JSONWriter
{
    public string fileName = null;
    public string dirName = null;
    public bool append_zero_to_filename = false;

    [SerializeField, ReadOnlyInsp] private string filePath;
    private bool _is_active = false;
    public bool is_active => _is_active;

    public bool Initialize() {
        string dname = $"{Application.persistentDataPath}/{Helpers.SaveSystemMethods.GetCurrentDateTime()}";
        if (dirName != null && dirName.Length > 0) {
            dname = $"{Application.persistentDataPath}/{dirName}";
        }
        Helpers.SaveSystemMethods.CheckOrCreateDirectory(dname);

        string fname = (fileName != null && fileName.Length > 0) ? fileName : System.DateTime.Now.ToString("HH-mm-ss");
        filePath = (append_zero_to_filename) ? Path.Combine(dname, fname+"_0.json") : Path.Combine(dname, fname+".json");

        int counter = 1;
        while(File.Exists(filePath)) {
            filePath = Path.Combine(dname, fname+$"_{counter}.json");
            counter++;
        }

        _is_active = true;
        return true;
    }

    public static string ConvertToJSON<T>(T data) {
        return JsonUtility.ToJson(data, true);
    }
    
    public static T ConvertFromJSON<T>(string data) {
        return JsonUtility.FromJson<T>(data);
    }
    
    public bool SaveJSON(string json) {
        if (!_is_active) return false;
        if (filePath.EndsWith(".json")) File.WriteAllText(filePath, json);
        else File.WriteAllText(filePath + ".json", json);
        return true;
    }
    public bool SaveJSON<T>(T data) {
        if (!_is_active) return false;
        string json = ConvertToJSON<T>(data);
        if (filePath.EndsWith(".json")) File.WriteAllText(filePath, json);
        else File.WriteAllText(filePath + ".json", json);
        return true;
    }
    
    public bool LoadJSON<T>(out T output) {
        if (!_is_active) {
            output = default(T);
            return false;
        }
        string actualFilePath = (filePath.EndsWith(".json")) ? filePath : filePath+".json";
        if (Helpers.SaveSystemMethods.CheckFileExists(actualFilePath)) {
            string fileContents = File.ReadAllText(actualFilePath);
            output = ConvertFromJSON<T>(fileContents);
            return true;
        } else {
            output = default(T);
            return false;
        }
    }

    public void Disable() {
        _is_active = false;
    }
}