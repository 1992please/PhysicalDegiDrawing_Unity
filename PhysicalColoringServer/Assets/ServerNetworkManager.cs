using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
public class ServerNetworkManager : NetworkManager
{
    private void Start()
    {
        StartServer();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        int PlayerID = extraMessageReader.ReadMessage<IntegerMessage>().value;
        GameObject player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        PlayerController PC = player.GetComponent<PlayerController>();
        ViewerManager.singlton.OnPlayerJoined(PC, PlayerID);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        print("Player Added: " + PlayerID);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, UnityEngine.Networking.PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        print("Player Removed");
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        print("Disconnect");
    }
}
