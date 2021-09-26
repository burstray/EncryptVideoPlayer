using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEncrypt : MonoBehaviour
{
#if UNITY_STANDALONE_WIN
    /// <summary>
    /// 屏幕分辨率
    /// </summary>
    [Header("仅用来记录屏幕分辨率")]
    public Vector2Int resolution = new Vector2Int(1920, 1080);
    /// <summary>
    /// 是否全屏
    /// </summary>
    [Header("是否全屏")]
    public bool fullScreen = true;

#endif

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(resolution.x,resolution.y,fullScreen);
        StateManager.ChangeState(new EncryptState());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
