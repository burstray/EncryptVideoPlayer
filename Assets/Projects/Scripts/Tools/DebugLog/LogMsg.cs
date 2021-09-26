using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*C:\Users\rjb\AppData\LocalLow\公司名 此路径可以查看程序的Log信息*/

public class LogMsg : MonoBehaviour
{
    public static LogMsg Instance;

    [Header("是否输出Debug信息")]
    public bool IsLog = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if(Config.Instance)
        {
            //IsLog = Config.Instance.configData.IsLogMsg;
        }
    }

    public void Log(string str)
    {
        if(IsLog)
        {
            Debug.Log(str);
        }   
    }
}
