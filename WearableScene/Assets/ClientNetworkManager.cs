using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
public class ClientNetworkManager : NetworkManager
{
    [SerializeField]
    private InputField Input;
	// Use this for initialization
	void Start () {
        StartClient();
	}

    public void OnClickReady()
    {
        IntegerMessage msg = new IntegerMessage(int.Parse(Input.text));
        ClientScene.AddPlayer(ClientScene.readyConnection, 0, msg);
    }
}
