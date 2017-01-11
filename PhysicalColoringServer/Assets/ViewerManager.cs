using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerManager : MonoBehaviour {
    int playerCount;

    [SerializeField]
    private GameObject SpawnedArtPiece;

    [SerializeField]
    private Mesh[] MeshArts;

    public static ViewerManager singlton;

    private Dictionary<int, ArtPiece> ArtBook = new Dictionary<int, ArtPiece>();

    private Dictionary<int, PlayerController> Players = new Dictionary<int, PlayerController>();

    ServerSocketAsync ServerSocket;

    private void Awake()
    {
        if (!singlton)
            singlton = this;

        ServerSocket = GetComponent<ServerSocketAsync>();
    }

    void Update()
    {
        if (ServerSocket.IsDataRecieved())
        {
            ObjectToTransfer obj = ServerSocket.ReadObjectAndClear();
            OnRecievedArt(obj);
        }
    }

    public void OnRecievedArt(ObjectToTransfer art)
    {
        if(!ArtBook.ContainsKey(art.PlayerID))
        {
            ArtPiece SpawnedArt = Instantiate<GameObject>(SpawnedArtPiece, Vector3.zero, Quaternion.identity).GetComponent<ArtPiece>();
            SpawnedArt.SetArt(art.GetTexture(), MeshArts[art.ObjectID]);
            ArtBook.Add(art.PlayerID, SpawnedArt);

            if (Players.ContainsKey(art.PlayerID))
            {
                Players[art.PlayerID].SetArtPiece(SpawnedArt);
            }
        }
    }

    public void OnPlayerJoined(PlayerController Player, int PlayerID)
    {
        Players.Add(PlayerID, Player);
        if(ArtBook.ContainsKey(PlayerID))
        {
            Player.SetArtPiece(ArtBook[PlayerID]);
        }
    }
}
