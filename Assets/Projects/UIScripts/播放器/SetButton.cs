using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetButton : MonoBehaviour
{
    public Text text;
    //public Image image;
    public Button SelfBtn;

    private void Awake()
    {
        text = FindTool.FindChildComponent<Text>(transform,"Text");
        //image = GetComponent<Image>();
        SelfBtn = GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SelfBtn.onClick.AddListener(() =>
        {
            if(text.color == Color.red)
            {
                text.color = Color.white;
            }
            else
            {
                text.color = Color.red;
            }
            UIManager.GetPanel<SettingPanel>(WindowTypeEnum.ForegroundScreen).JudeReadPlayPath(text.text);
        });
    }

    public void Init(string st,bool color)
    {
        text.text = st;
        if(color)
        {
            text.color = Color.red;
        }
        else
        {
            text.color = Color.white;
        }
    }
}
