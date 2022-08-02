using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerScript : NetworkBehaviour
{
    public static PlayerScript instance;
    private void Start()
    {
        if (isServer)
        {
            instance = this;
        }
        else
        {
            if (isLocalPlayer)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }
    }
    private void Update()
    {
        if (GameMan.instance.canMoveTeam != GameMan.MyteamID) return;
        if (GameMan._isMoving) return;

        if (Input.GetKeyDown(KeyCode.A)) CmdIWannaMove(Vector3.left);
        else if (Input.GetKeyDown(KeyCode.D)) CmdIWannaMove(Vector3.right);
        else if (Input.GetKeyDown(KeyCode.W)) CmdIWannaMove(Vector3.forward);
        else if (Input.GetKeyDown(KeyCode.S)) CmdIWannaMove(Vector3.back);
    }

    //Only execute on Server, but called on client
    [Command]
    public void CmdIWannaMove(Vector3 MoveDirection)
    {
        GameMan.instance.RpcMoveSpecific(GameMan.instance.canMoveTeam, MoveDirection);
    }
    [Command]
    public void CmdSwitchTurn()
    {
        if (GameMan.instance.canMoveTeam == 1)
        {
            GameMan.instance.canMoveTeam = 2;
        }
        else
        {
            GameMan.instance.canMoveTeam = 1;
        }
    }

}
