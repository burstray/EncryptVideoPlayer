using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System;

public class SettingPanel : BasePanel
{
    public Button SelectAllBtn,ClearAllBtn,StartBtn,PasswordBtn;
    public Dropdown Dropdown;
    public InputField InputField_X,InputField_Y,SwtichTimeInputField,PasswordInputField;
    public Text Msg;
    public ScrollRect LeftScroll,RightScroll;
    public CanvasGroup SwitchTimeCanvas,SetResolution,PasswordCanvas;

    //设置页面数据
    public VideoData videoData;
    //左边滑动列表
    public Dictionary<string,GameObject> BtnDic = new Dictionary<string, GameObject>();
    //右边滑动列表字典
    public Dictionary<string,GameObject> TextDic = new Dictionary<string, GameObject>();
    //准备播放视频列表
    public List<string> ReadPlayPath = new List<string>();

    protected override void Start()
    {
        base.Start();
        if (Config.Instance)
        {
            if (Config.Instance.configData.videoData != null)
            {
                videoData = Config.Instance.configData.videoData;
                Init();
            }
            else
            {
                Config.Instance.configData.videoData = new VideoData();
                videoData = Config.Instance.configData.videoData;
                videoData.playVideoDic = new Dictionary<string, bool>();
                Dropdown.value = 0;
                SwitchTimeCanvas.Hide();
                InputField_X.text = Screen.width.ToString();
                InputField_Y.text = Screen.height.ToString();
                SwtichTimeInputField.text = "6";
            }
        }

        SetResolution.Hide();
        PasswordCanvas.Hide();
    }

    /// <summary>
    /// 读取数据初始化
    /// </summary>
    private void Init()
    {
        switch (videoData.PlayMode)
        {
            case 0:
                Dropdown.value = 0;
                SwitchTimeCanvas.Hide();
                break;
            case 1:
                Dropdown.value = 1;
                SwitchTimeCanvas.Open();
                break;
            default:
                break;
        }

        InputField_X.text = videoData.ScreenX.ToString();
        InputField_Y.text = videoData.ScreenY.ToString();
        SwtichTimeInputField.text = videoData.SwitchTime.ToString();

        List<string> VideoList = Config.Instance.EncryptVideoPath;

        for (int i = 0; i < VideoList.Count; i++)
        {
            if(videoData.playVideoDic.ContainsKey(VideoList[i]))
            {
                CreatButton(VideoList[i],videoData.playVideoDic[VideoList[i]]);
                if(videoData.playVideoDic[VideoList[i]])
                {
                    CreatText(VideoList[i]);
                }
            }
            else
            {
                CreatButton(VideoList[i],false);
                videoData.playVideoDic.Add(VideoList[i],false);
            }
        }
    }

    public override void InitFind()
    {
        base.InitFind();
        SelectAllBtn = FindTool.FindChildComponent<Button>(transform,"SelectAllBtn");
        ClearAllBtn = FindTool.FindChildComponent<Button>(transform,"ClearAllBtn");
        //EncryptBtn = FindTool.FindChildComponent<Button>(transform,"EncryptBtn");
        StartBtn = FindTool.FindChildComponent<Button>(transform,"StartBtn");
        PasswordBtn = FindTool.FindChildComponent<Button>(transform,"Password/bg/CloseBtn");

        Dropdown = FindTool.FindChildComponent<Dropdown>(transform,"PlayMode/Dropdown");

        InputField_X = FindTool.FindChildComponent<InputField>(transform,"Resolution/X/InputField");
        InputField_Y = FindTool.FindChildComponent<InputField>(transform,"Resolution/Y/InputField");
        SwtichTimeInputField = FindTool.FindChildComponent<InputField>(transform,"PlayMode/SwtichTime");
        PasswordInputField = FindTool.FindChildComponent<InputField>(transform,"Password/bg/InputField");

        Msg = FindTool.FindChildComponent<Text>(transform,"Msg");

        LeftScroll = FindTool.FindChildComponent<ScrollRect>(transform,"LeftHint/Scroll View");
        RightScroll = FindTool.FindChildComponent<ScrollRect>(transform,"CentralHint/Scroll View");

        SwitchTimeCanvas = FindTool.FindChildComponent<CanvasGroup>(transform,"PlayMode/SwtichTime");
        SetResolution = FindTool.FindChildComponent<CanvasGroup>(transform,"Resolution");
        PasswordCanvas = FindTool.FindChildComponent<CanvasGroup>(transform,"Password");
    }

    public override void InitEvent()
    {
        base.InitEvent();
        Dropdown.onValueChanged.AddListener(OnModeChange);

        SelectAllBtn.onClick.AddListener(OnSelectAllBtn);
        ClearAllBtn.onClick.AddListener(OnClearAllBtn);
        //EncryptBtn.onClick.AddListener(OnEncryptBtn);
        StartBtn.onClick.AddListener(OnStartBtn);
        PasswordBtn.onClick.AddListener(CloseBtn);

        InputField_X.onValueChanged.AddListener(ValueChangeX);
        InputField_Y.onValueChanged.AddListener(ValueChangeY);
        SwtichTimeInputField.onValueChanged.AddListener(ValueChangeTime);
        PasswordInputField.onValueChanged.AddListener(ValueChangePassword);
    }

    /// <summary>
    /// 生成按钮
    /// </summary>
    /// <param name="st"></param>
    /// <param name="color"></param>
    public void CreatButton(string st, bool color)
    {
        if (!BtnDic.ContainsKey(st))
        {
            GameObject button = Instantiate(PoolManager.Instance.ButtonPrefabs); //PoolManager.Instance.GetPool(MTFrame.MTPool.PoolType.Button);
            button.transform.SetParent(LeftScroll.content);
            button.transform.SetAsLastSibling();
            button.GetComponent<SetButton>().Init(st, color);
            BtnDic.Add(st, button);
        }
    }

    ///// <summary>
    ///// 删除按钮
    ///// </summary>
    ///// <param name="st"></param>
    //public void DestroyButton(string st)
    //{
    //    if(BtnDic.ContainsKey(st))
    //    {
    //        Destroy(BtnDic[st]);
    //        //PoolManager.Instance.AddPool(MTFrame.MTPool.PoolType.Button,BtnDic[st]);
    //        BtnDic.Remove(st);
    //    }

    //    if (TextDic.ContainsKey(st))
    //    {
    //        Destroy(TextDic[st]);
    //        //PoolManager.Instance.AddPool(MTFrame.MTPool.PoolType.Text, TextDic[st]);
    //        TextDic.Remove(st);
    //    }

    //    if (videoData.playVideoDic.ContainsKey(st))
    //    {
    //        videoData.playVideoDic.Remove(st);
    //        st = Application.streamingAssetsPath + "/EncryptVideo/"+ st + ".ery";
    //        if (FileHandle.Instance.IsExistFile(st))
    //        {
    //            FileHandle.Instance.ClearFile(st);
    //        }
    //    }
    //}

    /// <summary>
    /// 生成Text
    /// </summary>
    /// <param name="st"></param>
    private void CreatText(string st)
    {
        if (!TextDic.ContainsKey(st))
        {
            GameObject Temp = Instantiate(PoolManager.Instance.TextPrefabs); 
            //PoolManager.Instance.GetPool(MTFrame.MTPool.PoolType.Text);
            Temp.transform.SetParent(RightScroll.content);
            Temp.transform.SetAsLastSibling();
            Temp.GetComponent<Text>().text = st;
            TextDic.Add(st, Temp);
        }
    }

    /// <summary>
    /// 删除Text
    /// </summary>
    /// <param name="st"></param>
    public void DestroyText(string st)
    {
        PoolManager.Instance.AddPool(MTFrame.MTPool.PoolType.Text, TextDic[st]);
        if(videoData.playVideoDic.ContainsKey(st))
        {
            videoData.playVideoDic[st] = false;
        }
    }

    private void ValueChangeTime(string arg0)
    {
        if (string.IsNullOrEmpty(arg0))
        {
            return;
        }

        int temp = int.Parse(arg0.Trim());
        if(temp == 0)
        {
            SwtichTimeInputField.text = "6";
            videoData.SwitchTime = 6;
            Msg.text = "间隔时间不能为0！";
            return;
        }

        videoData.SwitchTime = temp;
    }

    private void ValueChangeY(string arg0)
    {
        if (string.IsNullOrEmpty(arg0))
        {
            return;
        }

        int temp = int.Parse(arg0.Trim());
        if(temp == 0)
        {
            Msg.text = "分辨率Y不能为0！";
            InputField_Y.text = videoData.ScreenY.ToString();
        }
        else
        {
            videoData.ScreenY = temp;
        }
    }

    private void ValueChangeX(string arg0)
    {
        if (string.IsNullOrEmpty(arg0))
        {
            return;
        }

        int temp = int.Parse(arg0.Trim());
        if(temp == 0)
        {
            Msg.text = "分辨率X不能为0！";
            InputField_X.text = videoData.ScreenX.ToString();
        }
        else
        {
            videoData.ScreenX = temp;
        }
    }

    private void ValueChangePassword(string arg0)
    {
        if (string.IsNullOrEmpty(arg0))
        {
            return;
        }

        if(arg0 == "123456789zs")
        {
            SetResolution.Open();
            PasswordCanvas.Hide(0.5f);
        }
    }

    /// <summary>
    /// 全选按钮
    /// </summary>
    private void OnSelectAllBtn()
    {
        if(BtnDic!=null && BtnDic.Count > 0)
        {
            foreach (var item in BtnDic)
            {
                if (videoData.playVideoDic.ContainsKey(item.Key))
                {
                    videoData.playVideoDic[item.Key] = true;
                }

                if (BtnDic.ContainsKey(item.Key))
                {
                    BtnDic[item.Key].GetComponent<SetButton>().Init(item.Key, true);
                }

                CreatText(item.Key);
            }
        }
    }

    /// <summary>
    /// 全部清除
    /// </summary>
    private void OnClearAllBtn()
    {
        if (TextDic != null && TextDic.Count > 0)
        {
            foreach (var item in TextDic)
            {
                if (videoData.playVideoDic.ContainsKey(item.Key))
                {
                    videoData.playVideoDic[item.Key] = false;
                }

                if(BtnDic.ContainsKey(item.Key))
                {
                    BtnDic[item.Key].GetComponent<SetButton>().Init(item.Key,false);
                }

                DestroyText(item.Key);
            }

            TextDic.Clear();
        }
    }

    //private void OnEncryptBtn()
    //{
    //    UIState.SwitchPanel(PanelName.EncryptPanel);
    //}

    private void OnStartBtn()
    {
        if (SetResolution.alpha == 1)
        {
            if (string.IsNullOrEmpty(InputField_X.text))
            {
                Msg.text = "分辨率设置X不能为空！";
                return;
            }

            if (string.IsNullOrEmpty(InputField_Y.text))
            {
                Msg.text = "分辨率设置Y不能为空！";
                return;
            }
        }

        if (Dropdown.value ==1 && string.IsNullOrEmpty(SwtichTimeInputField.text))
        {
            Msg.text = "切换时间不能为空!";
            return;
        }

        Text[] texts = RightScroll.content.GetComponentsInChildren<Text>();
        if (texts.Length == 0)
        {
            Msg.text = "播放列表不能为空!";
            return;
        }
        else
        {
            ReadPlayPath.Clear();
            for (int i = 0; i < texts.Length; i++)
            {
                ReadPlayPath.Add(texts[i].text);
                string path = Application.streamingAssetsPath + "/EncryptVideo/"+ texts[i].text + ".ery";
                if(!FileHandle.Instance.IsExistFile(path))
                {
                    Msg.text = "文件丢失!请将--[" + texts[i].text + "]--从播放列表中去除。";
                    return;
                }
            }
        }

        UIState.SwitchPanel(PanelName.DisplayPanel);
        Msg.text = "";
    }

    private void CloseBtn()
    {
        PasswordCanvas.Hide();
    }

    /// <summary>
    /// 判断添加/删除准备播放列表的视频路径
    /// </summary>
    /// <param name="path"></param>
    public void JudeReadPlayPath(string path)
    {
        if(TextDic.ContainsKey(path))
        {
            DestroyText(path);
            TextDic.Remove(path);
        }
        else
        {
            CreatText(path);
            if (videoData.playVideoDic.ContainsKey(path))
            {
                videoData.playVideoDic[path] = true;
            }
        }
    }

    /// <summary>
    /// 播放模式切换
    /// </summary>
    /// <param name="value"></param>
    private void OnModeChange(int value)
    {
        if (value == 1)
        {
            SwitchTimeCanvas.Open();
        }
        else if (value == 0)
        {
            SwitchTimeCanvas.Hide();
        }

        videoData.PlayMode = Dropdown.value;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            PasswordCanvas.Open();
            PasswordInputField.text = "";
            EventSystem.current.SetSelectedGameObject(PasswordInputField.gameObject);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {

        }
    }

    /// <summary>
    /// 关闭时保存数据
    /// </summary>
    private void OnApplicationQuit()
    {
        Config.Instance.SaveData();
    }
}
