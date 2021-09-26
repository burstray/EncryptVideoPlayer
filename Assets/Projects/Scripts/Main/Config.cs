using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ConfigData
{
    /// <summary>
    /// 是否显示鼠标
    /// </summary>
    public bool isCursor;

    /// <summary>
    /// 是否开启软件前置
    /// </summary>
    public bool isOpenPrepose;

    public VideoData videoData;
}

public class VideoData
{
    public Dictionary<string,bool> playVideoDic;

    public int ScreenX;
    public int ScreenY;

    public int PlayMode;

    public int SwitchTime;
}


public class Config : MonoBehaviour
{
    public static Config Instance;
    
    public ConfigData configData  = new ConfigData();

    private string File_name = "config.txt";
    private string Path;

    public List<string> EncryptVideoPath = new List<string>();

    private void Awake()
    {
        Instance = this;
        configData = new ConfigData();
#if UNITY_STANDALONE_WIN
        Path = Application.streamingAssetsPath + "/" + File_name;
        if (FileHandle.Instance.IsExistFile(Path))
        {
            string st = FileHandle.Instance.FileToString(Path);
            Debug.Log(st);
            configData = JsonConvert.DeserializeObject<ConfigData>(st);
        }

        FileHandle.Instance.IsExisFolder(Application.streamingAssetsPath + "/EncryptVideo");
        EncryptVideoPath = FileHandle.Instance.GetVideoPath(Application.streamingAssetsPath + "/EncryptVideo");

#elif UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
        Path = Application.persistentDataPath + "/" + File_name;
        if(FileHandle.Instance.IsExistFile(Path))
        {
            string st = FileHandle.Instance.FileToString(Path);
            configData = JsonConvert.DeserializeObject<ConfigData>(st);
        }
        else
        {
            Path = Application.streamingAssetsPath + "/" + File_name;
            if (FileHandle.Instance.IsExistFile(Path))
            {
                string st = FileHandle.Instance.FileToString(Path);
                configData = JsonConvert.DeserializeObject<ConfigData>(st);
            }
        }
#endif
    }

    //    private void OnDestroy()
    //    {
    //#if UNITY_EDITOR
    //        SaveData();
    //#endif
    //    }

    private void Start()
    {
        
    }

    public void SaveData()
    {
#if UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
         Path = Application.persistentDataPath + "/" + File_name;
#endif
        string st = JsonConvert.SerializeObject(configData);
        FileHandle.Instance.SaveFile(st, Path);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
