using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameMan : NetworkBehaviour
{
    #region GameLogic
    public static GameMan instance;
    public static int MyteamID;//this is different, but this cant be changed by client
    public GameObject Cube1, Cube2;
    [SerializeField] private float _rollSpeed = 5;
    public static bool _isMoving;




    private void Start()
    {
        Debug.Log("start GameMan");
        FlipAroundPos(4, 4);
        FlipAroundPos(5, 4);
    }
    private void OnEnable()
    {
        Debug.Log("enable GameMan");
        instance = this;
    }

    private IEnumerator Roll(Vector3 anchor, Vector3 axis, GameObject tempPlayer)
    {
        _isMoving = true;
        for (var i = 0; i < 90 / _rollSpeed; i++)
        {
            tempPlayer.transform.RotateAround(anchor, axis, _rollSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        _isMoving = false;
    }


    public void CubeEnter(Collider temp_collider)
    {
        if (temp_collider.gameObject.GetComponent<CardInfo>() != null)
        {
            //if (MyUIIsPlaying)
            //{
            //    StopCoroutine(MyInventoryCo);
            //    MyInventory.GetComponent<Animator>().SetInteger("State", 0);
            //    foreach (GameObject slot in mySlotArray)
            //    {
            //        slot.GetComponent<Image>().material = UISlotEmpty;
            //    }
            //    MyUIIsPlaying = false;
            //}

            //get the card
            CardInfo tempCardInfo = temp_collider.gameObject.GetComponent<CardInfo>();
            FlipAroundPos(tempCardInfo.Xpos, tempCardInfo.Ypos);
            PlayerScript.instance.CmdSwitchTurn();
        }
    }

    private void FlipAroundPos(int tempX, int tempY)
    {
        CheckArrayFlip(tempX + 1, tempY);
        CheckArrayFlip(tempX - 1, tempY);
        CheckArrayFlip(tempX, tempY + 1);
        CheckArrayFlip(tempX, tempY - 1);
    }
    private void CheckArrayFlip(int tempX, int tempY)
    {
        if (tempX >= 0 && tempX < 9 && tempY >= 0 && tempY < 9)
        {
            if (CardManager.instance.CardArray[tempX, tempY] != null)
            {
                if (CardManager.instance.CardArray[tempX, tempY].GetComponent<CardInfo>().IsFaceUp == false)
                {
                    CardManager.instance.CardArray[tempX, tempY].transform.GetChild(0).GetComponent<Animation>().Play();
                    CardManager.instance.CardArray[tempX, tempY].GetComponent<CardInfo>().IsFaceUp = true;
                }
            }
        }
    }
    #endregion

    #region NetWork
    //Only execute on client, but called on server
    //Boardcast message
    [ClientRpc]
    public void RpcCardsInitiation(string temp_CardString)
    {
        CardManager.instance.DisassembleCardString(temp_CardString);
    }
    [ClientRpc]
    public void RpcMoveSpecific(int teamID, Vector3 MoveDirection)
    {
        GameObject tempGameObject = teamID == 1 ? Cube1 : Cube2;
        var anchor = tempGameObject.transform.position + (Vector3.down + MoveDirection) * 5f;
        var axis = Vector3.Cross(Vector3.up, MoveDirection);
        StartCoroutine(Roll(anchor, axis, tempGameObject));
    }
    //Singlecast message
    [TargetRpc]
    public void TargetSetTeamID(NetworkConnection target, int teamID)
    {
        MyteamID = teamID;
    }







    #endregion
}
