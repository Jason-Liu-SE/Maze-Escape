using UnityEngine;
using System.IO;

public static class SaveManager {
    public static string directory = "/Config/";

    public static void Save(Keybinds saveObject, string fileName) {
        // determining the "dynamic" directory
        string persistentPath = Application.dataPath + directory;

        // checking if the directory exists 
        if (!Directory.Exists(persistentPath))
            Directory.CreateDirectory(persistentPath);

        // storing the data we want to save in JSON format
        string json = JsonUtility.ToJson(saveObject);

        // writing to the file
        File.WriteAllText(persistentPath + fileName, json);
    }

    public static Keybinds Load(string fileName) {
        string path = Application.dataPath + directory + fileName;
        Keybinds saveObject = new Keybinds();

        // checking if the path exists
        if (File.Exists(path)) {
            // storing the file data. The data should be recorded in json format
            string json = File.ReadAllText(path);

            // converting the json data into class data
            saveObject = JsonUtility.FromJson<Keybinds>(json);
        } else 
            Debug.Log("There is no config file at the specified directory: " + path);

        return saveObject;
    }   
}
