using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameSettings : MonoBehaviour {
    public float ZoomMouseSensitivity = 0;
    public float MouseSensitivity = 0;
    // Use this for initialization
    void Awake () {
        string filePath = Path.Combine(Application.streamingAssetsPath, "Settings.json");
        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            GameSettingsClass loadedData = JsonUtility.FromJson<GameSettingsClass>(dataAsJson);
            ZoomMouseSensitivity = loadedData.ZoomMouseSensitivity;
            MouseSensitivity = loadedData.MouseSensitivity;
        }
        GameObject.Find("MouseSensitivityValue").GetComponent<Text>().text = MouseSensitivity.ToString();
        GameObject.Find("ZoomMouseSensitivityValue").GetComponent<Text>().text = ZoomMouseSensitivity.ToString();
        GameObject.Find("MouseSensitivity").GetComponent<Slider>().value = MouseSensitivity;
        GameObject.Find("ZoomMouseSensitivity").GetComponent<Slider>().value = ZoomMouseSensitivity;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetMouseSEnsitivity(float input)
    {
        MouseSensitivity = input;
        GameObject.Find("MouseSensitivityValue").GetComponent<Text>().text = input.ToString();
    }
    public void SetZoomMouseSEnsitivity(float input)
    {
        ZoomMouseSensitivity = input;
        GameObject.Find("ZoomMouseSensitivityValue").GetComponent<Text>().text = input.ToString();
    }
    public void SaveSettings()
    {
        GameSettingsClass gsc = new GameSettingsClass()
        {
            ZoomMouseSensitivity = this.ZoomMouseSensitivity,
            MouseSensitivity = this.MouseSensitivity
        };
        string dataAsJson = JsonUtility.ToJson(gsc);
        System.IO.Directory.CreateDirectory(Application.streamingAssetsPath);
        string filePath = Path.Combine(Application.streamingAssetsPath, "Settings.json");
        
        File.WriteAllText(filePath, dataAsJson);

    }
}
public class GameSettingsClass
{
    public float ZoomMouseSensitivity = 0;
    public float MouseSensitivity = 0;
}
