using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using System;

public class WaitPanel : BasePanel
{
    //返回待机页参数
    private float BackTime;
    private float Back_Time;
    private bool IsBack;

    //open方法执行的速度是比Start快的
    protected override void Start()
    {
        base.Start();
        if (Config.Instance)
        {
           // BackTime = Config.Instance.configData.Backtime;
        }
    }

    public override void InitFind()
    {
        base.InitFind();
    }

    public override void InitEvent()
    {
        base.InitEvent();
    }

    public override void Open()
    {
        base.Open();
        CountDownClose();
    }

    public override void Hide()
    {
        base.Hide();
        CountDownStart();
    }

    /// <summary>
    /// 开启倒计时
    /// </summary>
    public void CountDownStart()
    {
        IsBack = true;
        Back_Time = BackTime;
    }

    /// <summary>
    /// 关闭倒计时
    /// </summary>
    public void CountDownClose()
    {
        IsBack = false;
        Back_Time = 0;
    }

    private void Update()
    {
        if (Back_Time > 0 && IsBack)
        {
            Back_Time -= Time.deltaTime;

            if (Back_Time <= 0)
            {
                IsBack = false;
                GC.Collect();
                UIState.SwitchPanel(PanelName.SettingPanel);
            }
            /*凡是跟鼠标点击相关的使用这个GetMouseButton */
#if UNITY_STANDALONE_WIN
            //点击刷新倒计时
            if (Input.GetMouseButton(0))
            {
                Back_Time = BackTime;
            }
            /*凡是跟触摸屏幕相关的使用这个Touch 包括触摸屏*/
#elif UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
            if(Input.touchCount > 0)
            {
                Back_Time = BackTime;
            }
#endif
        }
    }
}
