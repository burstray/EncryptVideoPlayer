using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.IO;
using RVideoPlay;
using System.Threading.Tasks;

public class EncryptPanel : BasePanel
{
    public Button StartBtn,ReturnBtn;
    public Text Msg;
    public bool IsEncrypt;

    protected override void Start()
    {
        base.Start();
        FileHandle.Instance.IsExisFolder(Application.streamingAssetsPath + "/" + Config.VideoFoldName);
        //FileObject fileObject = FileManager.Read(Application.streamingAssetsPath + "/EncryptVideo/123.mt", MTFrame.MTFile.FileFormatType.mp4);
        //Debug.Log(fileObject.Buffet.Length);
    }

    public override void InitFind()
    {
        base.InitFind();
        StartBtn = FindTool.FindChildComponent<Button>(transform,"StartBtn");
        ReturnBtn = FindTool.FindChildComponent<Button>(transform,"ReturnBtn");
        Msg = FindTool.FindChildComponent<Text>(transform,"Msg");
    }

    public override void InitEvent()
    {
        base.InitEvent();
        StartBtn.onClick.AddListener(OnStartBtn);

        ReturnBtn.onClick.AddListener(OnStarFolder);
    }

    public override void Open()
    {
        base.Open();
        Msg.text = "";
    }

    private async void OnStartBtn()
    {
        if(IsEncrypt)
            return;

        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        openFileName.filter = "mp4文件(*.mp4)\0*.mp4";
        openFileName.file = new string(new char[256]);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\'); //默认路径
        openFileName.title = "窗口标题";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

        if (LocalDialog.GetSaveFileName(openFileName))
        {
            IsEncrypt = true;
            string path = openFileName.file;
            if (string.IsNullOrEmpty(path))
            {
                Msg.text = "文件路径为空，请选择文件！";
                return;
            }

            var fileName = Path.GetFileNameWithoutExtension(path);

            Msg.text = "视频加密中,请耐心等待......";
            await Task.Run(() =>
            {
                var data = File.ReadAllBytes(path);
                //FileManager.Write(Application.streamingAssetsPath + "/EncryptVideo/"+fileName+Config.FileSuffix,data,MTFrame.MTFile.FileFormatType.mp4,MTFrame.MTFile.EncryptModeType.None);
                using (var myStream = new MyStream(
                    Application.streamingAssetsPath + "/" + Config.VideoFoldName + "/" + fileName + Config.FileSuffix,
                    FileMode.Create))
                {
                    myStream.Write(data, 0, data.Length);
                }
            });

            Msg.text = "文件" + fileName + "加密成功！";
            Msg.text = Msg.text + "\n" + "请在" + Application.streamingAssetsPath + "/" + Config.VideoFoldName + "下查看加密文件！";
            IsEncrypt = false;
        }
    }

    private  async void OnStarFolder()
    {
        if (IsEncrypt)
            return;

        string path = WindowsExplorer.GetPathFromWindowsExplorer();  
        if (!string.IsNullOrEmpty(path))
        {
            List<string> vs = FileHandle.Instance.GetVideoPath2(path);
            if (vs.Count > 0)
            {
                Msg.text = "视频加密中,请耐心等待......";
                IsEncrypt = true;
                await Task.Run(() =>
                {
                    for (int i = 0; i < vs.Count; i++)
                    {
                        var data = File.ReadAllBytes(vs[i]);
                        //FileManager.Write(Application.streamingAssetsPath + "/EncryptVideo/"+Path.GetFileNameWithoutExtension(vs[i])+Config.FileSuffix,data,MTFrame.MTFile.FileFormatType.mp4,MTFrame.MTFile.EncryptModeType.None);
                        using (var myStream = new MyStream(
                            Application.streamingAssetsPath + "/" + Config.VideoFoldName + "/" + Path.GetFileNameWithoutExtension(vs[i]) + Config.FileSuffix,
                            FileMode.Create))
                        {
                            myStream.Write(data, 0, data.Length);
                        }
                    }
                });

                Msg.text = "加密完成，请在" + Application.streamingAssetsPath + "/" + Config.VideoFoldName + "下查看加密文件！";
                IsEncrypt = false;
            }
            else
            {
                Msg.text = "该路径下无可加密的视频！";
            }
        }
        else
        {
            Msg.text = "该路径为空，请重新选择文件夹！";
        }
    }
}
