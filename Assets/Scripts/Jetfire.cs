using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using UnityEngine;
using AOT;

public class Jetfire
{

    public static Queue<byte[]> ByteQueue = new Queue<byte[]>();
    public static String curr_message = "";

    delegate void JetfireConnectCallback();
    [MonoPInvokeCallback(typeof(JetfireConnectCallback))]
    private static void ConnectCallback()
    {
        Debug.Log("connect");
    }



    delegate void JetfireDisConnectCallback(string message);
    [MonoPInvokeCallback(typeof(JetfireDisConnectCallback))]
    private static void DisConnectCallback(string message)
    {
        Debug.Log("disconnect: " + message);
    }

    delegate void JetfireReceiveMessageCallback(string message);
    [MonoPInvokeCallback(typeof(JetfireReceiveMessageCallback))]
    private static void ReceiveMessageCallback(string message)
    {
        curr_message = message;
    }

    delegate void JetfireReceiveDataCallback(IntPtr pnt, ulong size);
    [MonoPInvokeCallback(typeof(JetfireReceiveDataCallback))]
    private static void ReceiveDataCallback(IntPtr pnt, ulong size)
    {
        if (ByteQueue.Count < 1)
        {
            byte[] bytes = new byte[size];
            Marshal.Copy(pnt, bytes, 0, (int)size);
            ByteQueue.Enqueue(bytes);
        }
    }


    #if UNITY_IOS && !UNITY_EDITOR
       private const string DllName = "__Internal";
    #else
       private const string DllName = "libJetfire";
    #endif




    [DllImport(DllName)]

    private static extern void JetfireOpen(
        string path,
        JetfireConnectCallback _connectCallback,
        JetfireDisConnectCallback _disConnectCallback,
        JetfireReceiveMessageCallback _receiveMessageCallback,
        JetfireReceiveDataCallback _receiveDataCallback
    );



    [DllImport(DllName)]
    private static extern void JetfireConnect();

    [DllImport(DllName)]
    private static extern void JetfireClose();

    [DllImport(DllName)]
    private static extern void JetfirePing();

    [DllImport(DllName)]
    private static extern void JetfireSendMsg(string msg);

    [DllImport(DllName)]
    private static extern void JetfireSendData(byte[] bytes, int size);

    [DllImport(DllName)]
    private static extern bool JetfireIsConnected();
    
    public static void Open(string path)
    {

        JetfireOpen(path,
            ConnectCallback,
            DisConnectCallback,
            ReceiveMessageCallback,
            ReceiveDataCallback);
    }

    public static void Connect()
    {
        JetfireConnect();
    }



    public static void Close()
    {
        JetfireClose();
    }



    public static void Ping()
    {
        JetfirePing();
    }



    public static void SendMsg(string msg)
    {
        JetfireSendMsg(msg);
    }



    public static void SendData(byte[] bytes)
    {
        JetfireSendData(bytes, bytes.Length);
    }



    public static bool IsConnected()
    {
        return JetfireIsConnected();
    }

}