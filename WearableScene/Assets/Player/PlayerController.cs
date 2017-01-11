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

    public override void OnStartLocalPlayer()
    {
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
}
