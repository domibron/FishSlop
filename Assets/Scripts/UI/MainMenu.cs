using System;
using kcp2k;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputField;

    NetRoomManager netRoomManager;

    void Start()
    {
        netRoomManager = NetRoomManager.singleton;
    }

    public void StartHostWithClient()
    {
        netRoomManager.StartHost();
    }

    public void StartClient()
    {
        string[] input = inputField.text.Split(':');

        print("Entered: " + inputField.text);

        KcpTransport transport = (KcpTransport)netRoomManager.transport;


        ushort port = transport.Port;

        if (input.Length >= 2)
        {
            if (ushort.TryParse(input[1], out ushort res))
            {
                if (res > 1024 && res < 49151)
                {
                    port = res;
                }
            }
        }

        transport.Port = port;



        if (string.IsNullOrEmpty(inputField.text))
            netRoomManager.networkAddress = "localhost";
        else
            netRoomManager.networkAddress = input[0];

        netRoomManager.StartClient();
    }
}
