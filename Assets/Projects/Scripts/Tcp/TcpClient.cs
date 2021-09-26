using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBY;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Sockets;
using System;

public class TcpClient : MonoBehaviour
{
    private static TcpClient _SocketClient;
    public AsyncTcpClient asyncTcpClient;
    int _port = 6000;
    string _ip;
    private Queue<string> GetVs = new Queue<string>();

    private bool IsConnet;
    private float _CountDown = 5.0f;
    private float CountDown;

    public static TcpClient Ins()
    {
        return _SocketClient;
    }

    private void Start()
    {
        _SocketClient = this;
        _ip = "127.0.0.1";
        asyncTcpClient = new AsyncTcpClient();
        asyncTcpClient.ConnectServer(_ip, _port);
        CountDown = _CountDown;
        asyncTcpClient.Receive += ReceiveMsg;
    }

    private void ReceiveMsg(string obj)
    {
        GetVs.Enqueue(obj);
    }

    private void Update()
    {
        lock (GetVs)
        {
            if (GetVs.Count > 0)
            {
                IsConnet = true;
                CountDown = _CountDown;
                Debug.Log(GetVs.Dequeue());
            }
            else
            {
                if (IsConnet)
                {
                    CountDown -= Time.deltaTime;
                    if (CountDown <= 0)
                    {
                        StartCoroutine(ReConnect());
                        IsConnet = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 向服务器发送消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void SentMsg(string str)
    {
        asyncTcpClient.SendMsg(str);
    }


    private void OnDestroy()
    {
        asyncTcpClient.Close();
    }

    public IEnumerator ReConnect()
    {
        asyncTcpClient.Close();
        yield return new WaitForEndOfFrame();
        asyncTcpClient.ConnectServer(_ip, _port);
        Debug.Log("开始重连");
    }
}
