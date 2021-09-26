using System.IO;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

#region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：OpenVideo
// 创 建 者：燃
// 创建时间：2020年12月12日 星期六 14:15
// 文件版本：V1.0.0
//===============================================================
// 功能描述：项目需要依赖avpro，odin，注意的是需要保护的视频文件不能大于2g，因为c#的底层问题
//		
//
//----------------------------------------------------------------*/

#endregion

namespace RVideoPlay
{
    public class OpenVideo : MonoBehaviour
    {
        private MediaPlayer _mediaPlayer;

        [SerializeField] private string FileName;

        private void Start()
        {
            if (FileName == null)
            {
                Debug.LogError("播放文件的名字不能为空！");
                return;
            }

            _mediaPlayer = GetComponent<MediaPlayer>();

            var path = Application.streamingAssetsPath + "/EncryptVideo/" + FileName + ".mp4";
            using (var fileStream = new MyStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024 * 4, false))
            {
                var bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, (int) fileStream.Length);
                _mediaPlayer.OpenVideoFromBuffer(bytes, true);
            }
        }
    }
}