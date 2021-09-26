using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TcpSever:MonoBehaviour
{
    public Queue<string> UdpMsgs = new Queue<string>();
    public static TcpSever Instance;
    private List<Socket> Client = new List<Socket>();
    private Dictionary<Socket,Thread> ClientThread = new Dictionary<Socket,  Thread>();
    int _port = 6000;
    string _ip;
    Socket socketSend;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    public void StartServer () {
        bt_connnect_Click();
    }

    private void Start()
    {
        _ip = "127.0.0.1";
        bt_connnect_Click();
    }

    private void bt_connnect_Click()
    {
        try
        {
            //点击开始监听时 在服务端创建一个负责监听IP和端口号的Socket
            Socket socketWatch =new Socket(AddressFamily.InterNetwork,  SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(_ip);
            //创建对象端口
            IPEndPoint point =new IPEndPoint(ip, _port);
            socketWatch.Bind(point);//绑定端口号
            Debug.Log("监听成功!");
            socketWatch.Listen(20);//设置监听，最大同时连接10台
            //创建监听线程
            Thread thread =new Thread(Listen);
            thread.IsBackground =true;
            thread.Start(socketWatch);
        }
        catch { }
    }
    /// <summary>
    /// 等待客户端的连接 并且创建与之通信的Socket
    /// </summary>
    
    void Listen(object o)
    {
        try
        {
            Socket socketWatch = o as Socket;
            while (true)
            {
                socketSend = socketWatch.Accept();//等待接收客户端连接
                Debug.Log(socketSend.RemoteEndPoint.ToString() +":" +"连接成功!");
                Client.Add(socketSend);
                //开启一个新线程，执行接收消息方法
                Thread r_thread =new Thread(Received);
                r_thread.IsBackground =true;
                r_thread.Start(socketSend);
                ClientThread.Add(socketSend,r_thread);
            }
        }
        catch { }
    }

    /// <summary>
    /// 服务器端不停的接收客户端发来的消息
    /// </summary>
    /// <param name="o"></param>
    void Received(object o)
    {
        try
        {
            Socket socketSend = o as Socket;
            while (true)
            {
                //客户端连接服务器成功后，服务器接收客户端发送的消息
                byte[] buffer =new byte[1024 * 1024 * 3];
                //实际接收到的有效字节数
                int len = socketSend.Receive(buffer);
              
                if (len == 0)
                {
                    break;
                }
                string str = Encoding.UTF8.GetString(buffer, 0, len);
                #if UNITY_EDITOR
                Debug.Log("服务器打印：" + socketSend.RemoteEndPoint +":" + str);
                #endif

                UdpMsgs.Enqueue(str);
            }
        }
        catch { }
    }
    /// <summary>
    /// 服务器向客户端发送消息
    /// </summary>
    /// <param name="str"></param>
    public void Send(string str)
    {
        if(Client.Count > 0)
        {
#if UNITY_EDITOR
            Debug.Log("str ==" + str);
#endif
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < Client.Count; i++)
            {
                try
                {
                    Client[i].Send(buffer);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    if (Client[i] != null)
                    {
                        Client[i].Close();
                        if (ClientThread.ContainsKey(Client[i]))
                        {
                            if(ClientThread[Client[i]]!=null)
                            {
                                ClientThread[Client[i]].Interrupt();
                                ClientThread[Client[i]].Abort();
                                ClientThread[Client[i]] = null;
                                ClientThread.Remove(Client[i]);
                            }
                        }
                        Client[i] = null;
                        Client.RemoveAt(i);
                        i--;
                        if(i == Client.Count && Client.Count == 0)
                            break;
                    }
                }
            }
        }
    }
    private void Update()
    {
        lock (UdpMsgs)
        {
            if (UdpMsgs.Count > 0)
            {
                Debug.Log(UdpMsgs.Dequeue());
            }
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < Client.Count; i++)
        {
            if (Client[i] != null)
            {
                Client[i].Close();
                if (ClientThread.ContainsKey(Client[i]))
                {
                    if(ClientThread[Client[i]]!=null)
                    {
                        ClientThread[Client[i]].Interrupt();
                        ClientThread[Client[i]].Abort();
                        ClientThread[Client[i]] = null;
                        ClientThread.Remove(Client[i]);
                    }
                }
                Client[i] = null;
            }
        }
        Client.Clear();
        ClientThread.Clear();
    }
}
