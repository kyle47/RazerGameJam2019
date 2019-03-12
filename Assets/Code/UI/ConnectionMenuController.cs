using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Mirror;

public class ConnectionMenuController : MonoBehaviour
{
    public static ConnectionMenuController Instance;

    public NetworkManager NetworkManager;
    public TelepathyTransport Transport;

    public InputField JoinNetworkAddressInput;
    public InputField JoinPortInput;
    public InputField HostPortInput;

    public Text FeedbackText;

    public GameObject ConnectionMenu;

    private void Awake()
    {
        if(Instance)
        {
            GameObject.Destroy(gameObject);
            return;
        }

        Instance = this;

        JoinNetworkAddressInput.text = NetworkManager.networkAddress;
        JoinPortInput.text = Transport.port.ToString();
    }

    protected void SetPort(string portText)
    {
        if (ushort.TryParse(portText, out ushort port))
        {
            Transport.port = port;
        }
        else
        {
            FeedbackText.text = $"'{portText}' is not a valid port";
            return;
        }
    }

    public void Button_Connect()
    {
        FeedbackText.text = "";
        SetPort(JoinPortInput.text);
        
        var client = NetworkManager.client ?? NetworkManager.StartClient();
        var address = JoinNetworkAddressInput.text;

        client.Connect(address);

        StartCoroutine(CheckConnection_Coroutine(3.0f, OnConnectionSuccess, OnConnectionFailed));
    }

    public void Button_Host()
    {
        FeedbackText.text = "";
        SetPort(HostPortInput.text);
        var client = NetworkManager.StartHost();

        StartCoroutine(CheckConnection_Coroutine(3.0f, OnConnectionSuccess, OnConnectionFailed));
    }

    public void OnConnectionFailed()
    {
        var client = NetworkManager.client;
        FeedbackText.text = "Failed to connect to server";
    }

    public void OnConnectionSuccess()
    {
        var client = NetworkManager.client;
        ConnectionMenu.SetActive(false);
    }

    protected IEnumerator CheckConnection_Coroutine(float timeout, Action success, Action failure)
    {
        FeedbackText.text = "Connecting...";
        var endTime = Time.time + timeout;
        while(Time.time < endTime)
        {
            if((NetworkManager.client != null) && NetworkManager.client.isConnected)
            {
                break;
            }
            yield return null;
        }

        if ((NetworkManager.client != null) && NetworkManager.client.isConnected)
        {
            success();
        }
        else
        {
            failure();
        }
    }
}
