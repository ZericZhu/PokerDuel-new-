using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;
using UnityEngine.UI;

public class GameMan : NetworkBehaviour
{
    #region GameLogic
    [SyncVar(hook = nameof(TurnSwitch))]
    public int canMoveTeam = 0;

    public static GameMan instance;
    public static int MyteamID;//this is different, but this cant be changed by client
    public GameObject Cube1, Cube2;
    [SerializeField] private float _rollSpeed = 5;
    public static bool _isMoving;
    public GameObject MainSlotParent, MinorSlotParent;
    public Text MainScoreDisplay, MinorScoreDisplay;
    public Material UISlotEmpty;

    //timer bar stuff
    public Image timerImage;
    public Color TimerDefault, TimerRed;

    private void OnEnable()
    {
        instance = this;
    }

    private void Start()
    {
        FlipAroundPos(4, 4);
        FlipAroundPos(5, 4);
    }
    private void SetMainMinor()
    {
        if (MyteamID == 1)
        {
            Cube1.GetComponent<CubeScript>().slotParent = MainSlotParent;
            Cube1.GetComponent<CubeScript>().myScoreDisplay = MainScoreDisplay;
            Cube2.GetComponent<CubeScript>().slotParent = MinorSlotParent;
            Cube2.GetComponent<CubeScript>().myScoreDisplay = MinorScoreDisplay;
            CameraFollow.instance.target = Cube1.transform;
        }
        else
        {
            Cube2.GetComponent<CubeScript>().slotParent = MainSlotParent;
            Cube2.GetComponent<CubeScript>().myScoreDisplay = MainScoreDisplay;
            Cube1.GetComponent<CubeScript>().slotParent = MinorSlotParent;
            Cube1.GetComponent<CubeScript>().myScoreDisplay = MinorScoreDisplay;
            CameraFollow.instance.target = Cube2.transform;
        }
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


    public void FlipAroundPos(int tempX, int tempY)
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


    public void CalculateScore(ref int tempscore, int[] tempSUITarray, int[] tempNUMBERarray)
    {
        int Score = 0;
        int Combo = ComboIdentification(tempSUITarray, tempNUMBERarray);
        switch (Combo)
        {
            case 1://high card
                Score = 0;
                break;
            case 2://pair
                Score = 1;
                break;
            case 3://TwoPair
                Score = 5;
                break;
            case 4://Three
                Score = 15;
                break;
            case 5://Straight
                Score = 80;
                break;
            case 6://Flush
                Score = 160;
                break;
            case 7://FullHouse
                Score = 200;
                break;
            case 8://Four
                Score = 1000;
                break;
            case 9://StraightFlush
                Score = 3000;
                break;
        }
        tempscore += Score;
    }

    private int ComboIdentification(int[] tempSUITarray, int[] tempNUMBERarray)
    {
        if (tempSUITarray[0] == tempSUITarray[1] && tempSUITarray[2] == tempSUITarray[3] && tempSUITarray[4] == tempSUITarray[1] && tempSUITarray[4] == tempSUITarray[2])
        {
            //all the same suit
            if (IsStraight(tempNUMBERarray))
            {
                return 9;
            }
            return 6;
        }
        //different suits
        int DistinctNumber = tempNUMBERarray.Distinct().Count();
        if (DistinctNumber == 2)
        {
            if (IsFour(tempNUMBERarray))
            {
                return 8;
            }
            return 7;
        }
        else if (DistinctNumber == 3)
        {
            if (IsThree(0, tempNUMBERarray))
            {
                return 4;
            }
            return 3;
        }
        else if (DistinctNumber == 4)
        {
            return 2;
        }
        else if (IsStraight(tempNUMBERarray))
        {
            return 5;
        }
        return 1;
    }

    private bool IsStraight(int[] tempNUMBERarray)
    {
        Array.Sort(tempNUMBERarray);
        if (tempNUMBERarray[0] + 1 == tempNUMBERarray[1] && tempNUMBERarray[1] + 1 == tempNUMBERarray[2] && tempNUMBERarray[2] + 1 == tempNUMBERarray[3] && tempNUMBERarray[3] + 1 == tempNUMBERarray[4])
        {
            return true;
        }
        return false;
    }

    private bool IsFour(int[] tempNUMBERarray)
    {
        int count = 0;
        int checkvalue = tempNUMBERarray[0];
        for (var i = 0; i < 5; i++)
        {
            if (tempNUMBERarray[i] == checkvalue) count++;
        }
        if (count == 4 || count == 1)
        {
            return true;
        }
        return false;
    }

    private bool IsThree(int tempIndex, int[] tempNUMBERarray)
    {
        int count = 0;
        int checkvalue = tempNUMBERarray[tempIndex];
        for (var i = 0; i < 5; i++)
        {
            if (tempNUMBERarray[i] == checkvalue) count++;
        }
        if (count == 2)
        {
            return false;
        }
        if (count == 3)
        {
            return true;
        }

        return IsThree(tempIndex + 1, tempNUMBERarray);
    }

    public void TurnSwitch(int oldturn, int newTurn)
    {
        if(newTurn == MyteamID)
        {
            timerImage.color = TimerRed;
            //timer mechanism
            timerImage.GetComponent<Animation>().Rewind();
            timerImage.GetComponent<Animation>().Play();
        }
        else
        {
            timerImage.color = TimerDefault;
            timerImage.GetComponent<Animation>().Stop();
            timerImage.transform.localScale = Vector3.one;
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
        SetMainMinor();
    }







    #endregion
}
