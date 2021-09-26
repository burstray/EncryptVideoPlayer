using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using UnityEngine.UI;
using RVideoPlay;
using RenderHeads.Media.AVProVideo;
using System.IO;
using System;

public class DisplayPanel : BasePanel
{
    public MediaPlayer mediaPlayer;
    private List<string> PlayPath = new List<string>();
    private VideoData videoData;
    private int playIndex;
    private string savePath;

    protected override void Awake()
    {
        base.Awake();
        if(Config.Instance)
        {
            videoData = Config.Instance.configData.videoData;
        }
    }

    public override void InitFind()
    {
        base.InitFind();
        mediaPlayer = FindTool.FindChildComponent<MediaPlayer>(transform,"VideoGroup/VideoPlayer");
    }

    public override void Open()
    {
        base.Open();
        EventManager.AddUpdateListener(MTFrame.MTEvent.UpdateEventEnumType.Update,"Aupdate",Aupdate);

        if(videoData == null)
            return;

        PlayPath = UIManager.GetPanel<SettingPanel>(WindowTypeEnum.ForegroundScreen).ReadPlayPath;
        InitMediaPlayer(videoData.PlayMode);
    }

    private void Aupdate(float timeProcess)
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            UIState.SwitchPanel(PanelName.SettingPanel);
        }
    }

    public override void Hide()
    {
        base.Hide();
        EventManager.RemoveUpdateListener(MTFrame.MTEvent.UpdateEventEnumType.Update,"Aupdate",Aupdate);
        Reset();
    }

    private void InitMediaPlayer(int num)
    {
        Screen.SetResolution(videoData.ScreenX,videoData.ScreenY,true);
        switch (num)
        {
            case 0:
                PlayVideo();
                mediaPlayer.Events.AddListener(OnVideoEvent);
                mediaPlayer.m_Loop = false;
                break;
            case 1:
                SwtichVideo();
                mediaPlayer.m_Loop = true;
                break;
            default:
                break;
        }
    }

    private void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.FinishedPlaying:
                PlayVideo();
                break;
        }
    }

    private void SwtichVideo()
    {
        PlayVideo();
        StartCoroutine(CountDown(videoData.SwitchTime, SwtichVideo));
    }

    private IEnumerator CountDown(int Count,Action action)
    {
        int num = Count;
        while(num > 0)
        {
            yield return new WaitForSeconds(1);
            num--;
            if(num <= 0)
            {
                action.Invoke();
            }
        }
    }

    private void PlayVideo()
    {
        string path = Application.streamingAssetsPath + "/EncryptVideo/" + PlayPath[playIndex] + ".ery";
        playIndex = (playIndex + 1) % PlayPath.Count;
        //using (var fileStream = new MyStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false))
        //{
        //    var bytes = new byte[fileStream.Length];
        //    fileStream.Read(bytes, 0, (int)fileStream.Length);
        //    mediaPlayer.OpenVideoFromBuffer(bytes, true);
        //}
        MyStream fsRead = new MyStream(path, FileMode.Open);
        //解密的数据
        byte[] readRaws = new byte[fsRead.Length];
        var dataRead = fsRead.Read(readRaws, 0, (int)fsRead.Length);
        fsRead.Dispose();

        //将解密的数据保存到系统缓存文件
        savePath = Application.persistentDataPath + "/" + "c0.dll";
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        FileStream fs = new FileStream(savePath, FileMode.Create);
        using (BinaryWriter w = new BinaryWriter(fs))
        {
            w.Write(readRaws);
        }
        fs.Close();
        fs.Dispose();

        mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL,
            savePath, true);
    }

    private void Reset()
    {
        mediaPlayer.CloseVideo();
        mediaPlayer.Events.RemoveAllListeners();
        StopAllCoroutines();
        playIndex = 0;
    }
}
