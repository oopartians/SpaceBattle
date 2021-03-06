﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Chat : MonoBehaviour {
    public InputField input;
    public Text chatText;


    public void Send(string msg)
    {
        string message = NetworkValues.name + ":" + input.text;
        Client.instance.Send(NetworkDecorator.AttachHeader(NetworkHeader.CHAT, message));
        AddMessage(message);
    }


    public void AddMessage(string m)
    {
        if (chatText.text.Length > 0)
        {
            chatText.text += "\n";
        }
        string m2 = m.Replace(Convert.ToChar(0x0).ToString(), "");
        chatText.text += m2;
    }

    void Start()
    {
        Client.instance.onMessageReceived.Add(OnMessageReceived);
    }

    void OnMessageReceived(NetworkDecorator.NetworkMessage m)
    {
        if (m.header == NetworkHeader.CHAT)
        {
            AddMessage(m.message);
        }
    }


    void OnDestroy()
    {
        Client.instance.onMessageReceived.Remove(OnMessageReceived);
    }
}
