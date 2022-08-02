using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerScript : NetworkBehaviour
{
    [SyncVar]
    public int canMoveTeam = 1;
    public static PlayerScript instance;
    private void Start()
    {
        instance = this;
    }
    private void Update()
    {
        if (canMoveTeam != GameMan.MyteamID) return;
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
        GameMan.instance.RpcMoveSpecific(canMoveTeam,MoveDirection);
    }
    [Command]
    public void CmdSwitchTurn()
    {
        if (canMoveTeam == 1)
        {
            canMoveTeam = 2;
            return;
        }
        canMoveTeam = 1;
    }
}
