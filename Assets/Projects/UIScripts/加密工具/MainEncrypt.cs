using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEncrypt : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Screen.SetResolution(resolution.x,resolution.y,fullScreen);
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
