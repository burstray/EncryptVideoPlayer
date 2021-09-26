using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace RVideoPlay.Editor
{
    #region << 文 件 说 明 >>

/*----------------------------------------------------------------
// 文件名称：PrivateVideo
// 创 建 者：燃
// 创建时间：2020年12月12日 星期六 18:11
// 文件版本：V1.0.0
//===============================================================
// 功能描述：
//		
//
//----------------------------------------------------------------*/

    #endregion

    
    public class PlayWindow :OdinEditorWindow
    {
        public string path;

        private string Rlog = "请先选择要保护的视频文件";
        [MenuItem("My Game/保护视频")]
        private static void OpenWindow()
        {
            GetWindow<PlayWindow >().Show();
        }

        [Button(ButtonSizes.Medium)]
        public void SelectFile()
        {
            var filePanel = EditorUtility.OpenFilePanel("Load video", "", "");
            path = filePanel;
        }

        [Button(ButtonSizes.Medium)]
        [InfoBox("$Rlog")]
        public void DoIt()
        {
            if (path == null)
            {
                return;
            }
            var fileName = Path.GetFileName(path);
            var data = File.ReadAllBytes(Path.Combine(path));
            using (var myStream = new MyStream(Path.Combine(Application.streamingAssetsPath, "encypt_" + fileName),FileMode.Create))
            {
                myStream.Write(data, 0, data.Length);
            }

            Rlog = "已完成";
        }
        
    }
}