using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using UnityEngine.UI;
using RVideoPlay;
using RenderHeads.Media.AVProVideo;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.Video;

public class DisplayPanel : BasePanel
{
    public MediaPlayer mediaPlayer;
    private List<string> PlayPath = new List<string>();
    private VideoData videoData;
    public int playIndex;
    private int PlayCount;
    private string savePath;

    private string SaveA, SaveB, SaveC;
    private bool IsEncrypt,IsPlaying;
    private CancellationTokenSource _sayHelloTokenSource;

    protected override void Awake()
    {
        base.Awake();
        if(Config.Instance)
        {
            videoData = Config.Instance.configData.videoData;
        }

        SaveA = Application.persistentDataPath + "/" + "a.dll";
        SaveB = Application.persistentDataPath + "/" + "b.dll";
        SaveC = Application.persistentDataPath + "/" + "c.dll";
    }

    public override void InitFind()
    {
        base.InitFind();
        mediaPlayer = FindTool.FindChildComponent<MediaPlayer>(transform,"VideoGroup/MediaPlayer");
    }

    public override void InitEvent()
    {
        base.InitEvent();
        mediaPlayer.Events.AddListener(OnVideoEvent);
        mediaPlayer.m_Loop = true;
    }

    public override void Open()
    {
        base.Open();
        EventManager.AddUpdateListener(MTFrame.MTEvent.UpdateEventEnumType.Update,"Aupdate",Aupdate);
        if(videoData == null)
            return;

        PlayPath = UIManager.GetPanel<SettingPanel>(WindowTypeEnum.ForegroundScreen).ReadPlayPath;
        InitMediaPlayer();
    }

    private void Aupdate(float timeProcess)
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            UIState.SwitchPanel(PanelName.SettingPanel);
        }

        //if(Input.GetKeyDown(KeyCode.E))
        //{
        //    IsPlaying = false;
        //    SwtichVideo();
        //}
    }

    public override void Hide()
    {
        base.Hide();
        EventManager.RemoveUpdateListener(MTFrame.MTEvent.UpdateEventEnumType.Update,"Aupdate",Aupdate);
        Reset();
        Cursor.visible = true;
    }

    private void InitMediaPlayer()
    {
        Cursor.visible = false;
        Reset();
        Screen.SetResolution(videoData.ScreenX,videoData.ScreenY,true);
        Decryption();
    }

    private void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            //case MediaPlayerEvent.EventType.FinishedPlaying:
            //    if (IsPlaying)
            //        IsPlaying = false;

            //    PlayNextVideo();
                //break;
            case MediaPlayerEvent.EventType.Error:
                SwtichVideo();
                break;
            default:
                break;
        }
    }

    private void SwtichVideo()
    {
        IsPlaying = false;
        PlayVideo();
        Decryption();
    }

    private IEnumerator CountDown(int Count, Action action)
    {
        int num = Count;
        while (num > 0)
        {
            yield return new WaitForSeconds(1);
            num--;
            if (num <= 0)
            {
                action.Invoke();
            }
        }
    }

    private void PlayVideo()
    {
        if (IsEncrypt)
            return;

        mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL,savePath, true);
        IsPlaying = true;
        StopAllCoroutines();
        StartCoroutine(CountDown(videoData.SwitchTime, SwtichVideo));
    }


    /// <summary>
    /// 异步解密
    /// </summary>
    private async void Decryption()
    {
        //只有一个视频的时候特殊处理
        if(PlayPath.Count == 1 && PlayCount>=1)
        {
            PlayCount++;
            if (PlayCount % 5 == 0)
            {
                GC.Collect();
            }
            return;
        }

        if (IsEncrypt)
            return;

        IsEncrypt = true;
        string path = Application.streamingAssetsPath + "/" + Config.VideoFoldName + "/" + PlayPath[playIndex] + Config.FileSuffix;
        _sayHelloTokenSource = new CancellationTokenSource();
        await Task.Run(() =>
        {
            try
            {
                MyStream fsRead = new MyStream(path, FileMode.Open);
                //解密的数据
                byte[] readRaws = new byte[fsRead.Length];
                fsRead.Read(readRaws, 0, (int)fsRead.Length);
                fsRead.Close();
                fsRead.Dispose();

                //将解密的数据保存到系统缓存文件
                if (PlayCount % 3 == 0)
                {
                    savePath = SaveA;
                }
                else if (PlayCount % 3 == 1)
                {
                    savePath = SaveB;
                }
                else
                {
                    savePath = SaveC;
                }

                FileStream fs = new FileStream(savePath, FileMode.Create);
                BinaryWriter w = new BinaryWriter(fs);
                w.Write(readRaws);
                w.Close();
                w.Dispose();

                fs.Close();
                fs.Dispose();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                IsEncrypt = false;
                Decryption();
                return;
            }

        }, _sayHelloTokenSource.Token);

        if(!IsOpen)
        {
            Reset();
            return;
        }

        IsEncrypt = false;
        playIndex = (playIndex + 1) % PlayPath.Count;
        PlayCount++;

        //视频播放完成但是下一个视频还没加载完成的特殊处理
        if (PlayCount > 1 && !IsPlaying)
        {
            Debug.Log("视频加载完成！");
            SwtichVideo();
        }

        //第一次播放特殊处理
        if (PlayCount == 1)
        {
            SwtichVideo();
        }

        //每播五个视频就调一下GC
        if(PlayCount % 5 == 0)
        {
            GC.Collect();
        }
    }

    private void Reset()
    {
        if(mediaPlayer!=null)
        {
            mediaPlayer.CloseVideo();
        }
       
        playIndex = 0;
        PlayCount = 0;
        if(_sayHelloTokenSource!=null)
        {
            Debug.Log("取消Task");
            _sayHelloTokenSource.Cancel();
            _sayHelloTokenSource.Dispose();
            _sayHelloTokenSource = null;
        }
        StopAllCoroutines();
        IsEncrypt = false;
        IsPlaying = false;
    }

    private void OnApplicationQuit()
    {
        if (File.Exists(SaveA))
        {
            File.Delete(SaveA);
        }

        if (File.Exists(SaveB))
        {
            File.Delete(SaveB);
        }

        if (File.Exists(SaveC))
        {
            File.Delete(SaveC);
        }

        if(_sayHelloTokenSource!=null)
        {
            _sayHelloTokenSource.Cancel();
            _sayHelloTokenSource.Dispose();
            _sayHelloTokenSource = null;
        }

        StopAllCoroutines();
    }
}
