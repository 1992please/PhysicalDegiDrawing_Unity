using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerController : NetworkBehaviour
{
    bool bDown;
    bool bUp;
    bool bRight;
    bool bLeft;

    Vector2 Axis;

    ArtPiece OwnedArtPiece;

    public override void OnStartServer()
    {
        bDown = false;
        bUp = false;
        bRight = false;
        bLeft = false;
    }

    private void Update()
    {
        if (isServer && OwnedArtPiece)
        {
            SetPosition();
        }

    }

    [Client]
    void TransmitControls()
    {
        if (isLocalPlayer)
        {
            SetPosition();
        }
    }

    [Server]
    public void SetPosition()
    {
        if (!HasArtPiece())
            return;

        Vector2 Axis;
        Axis.x = (bRight ? 0 : 1) - (bLeft ? 0 : 1);
        Axis.y = (bUp ? 0 : 1) - (bDown ? 0 : 1);
        OwnedArtPiece.UpdatePosition(Axis);
    }

    [Command]
    void CmdDoSpecialMove()
    {

    }

    [Command]
    void CmdClickRight(bool Value)
    {
        bRight = Value;
        print("Right");
    }

    [Command]
    void CmdSetUp(bool Value)
    {
        bUp = Value;
    }

    [Command]
    void CmdSetLeft(bool Value)
    {
        bLeft = Value;
    }

    [Command]
    void CmdSetDown(bool Value)
    {
        bDown = Value;
    }

    [Server]
    public bool HasArtPiece()
    {
        return OwnedArtPiece;
    }

    [Server]
    public void SetArtPiece(ArtPiece OwnedPiece)
    {
        OwnedArtPiece = OwnedPiece;
    }
}
