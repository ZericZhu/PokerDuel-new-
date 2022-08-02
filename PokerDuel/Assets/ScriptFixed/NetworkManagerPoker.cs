using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkManagerPoker : NetworkManager
{
    public static NetworkConnectionToClient[] ConnArray = new NetworkConnectionToClient[2];
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn,player);//this grant the joined client authority, hence, they can send command to the server
        GameMan.instance.TargetSetTeamID(conn, numPlayers);
        if (numPlayers == 2)
        {
            GameMan.instance.RpcCardsInitiation(CardManager.instance.CardString);
            GameMan.instance.canMoveTeam = 1;
        }
    }

}
