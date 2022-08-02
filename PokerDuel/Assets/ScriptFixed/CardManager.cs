using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class CardManager : MonoBehaviour
{
    [Header("Spade, Heart, Diamond, Club")]
    public static CardManager instance;
    public Material[] CardFront = new Material[52];
    public Vector3[] CardPos = new Vector3[52];
    public GameObject[,] CardArray = new GameObject[10,10];
    public GameObject CardPrefab;
    public string CardString = "";
    private void Awake()
    {
        instance = this;
        for (int i = 0; i < CardPos.Length; i++)
        {
            CardPos[i] = this.transform.GetChild(i).transform.localPosition;
        }
            CardGeneration();
    }

    private void CardGeneration()
    {
        for (int i = 0; i < CardFront.Length; i++)
        {
            GameObject TempCard = Instantiate(CardPrefab, this.transform);
            TempCard.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = CardFront[i];
            TempCard.transform.position = CardPos[i];
            if (i < 13)
            {
                TempCard.GetComponent<CardInfo>().SUIT = 1;
                TempCard.GetComponent<CardInfo>().NUMBER = i+1;
            }
            else if(i < 26)
            {
                TempCard.GetComponent<CardInfo>().SUIT = 2;
                TempCard.GetComponent<CardInfo>().NUMBER = i - 13 + 1;
            }
            else if (i < 39)
            {
                TempCard.GetComponent<CardInfo>().SUIT = 3;
                TempCard.GetComponent<CardInfo>().NUMBER = i - 26 + 1;
            }
            else
            {
                TempCard.GetComponent<CardInfo>().SUIT = 4;
                TempCard.GetComponent<CardInfo>().NUMBER = i - 39 + 1;
            }

            if (i == 0 || i == 13 || i == 26 || i == 39)
            {
                TempCard.GetComponent<CardInfo>().NUMBER = 14;
            }
            int tempX = (int)((CardPos[i].x - 40)/-10);
            int tempY = (int)((CardPos[i].z + 40) / 10);
            TempCard.GetComponent<CardInfo>().Xpos = tempX;
            TempCard.GetComponent<CardInfo>().Ypos = tempY;
            CardArray[tempX, tempY] = TempCard;
            TempCard.name = $"X: {tempX} Y: {tempY}";
        }
        //randomize their position

        for (int i = 0; i < 60; i++)
        {
            GameObject FirstCard = CardArrayExist();
            GameObject SecondCard = CardArrayExist();
            CardInfo FirstCardInfo = FirstCard.GetComponent<CardInfo>();
            CardInfo SecondCardInfo = SecondCard.GetComponent<CardInfo>();

            int FirstXpos = FirstCardInfo.Xpos;
            int FirstYpos = FirstCardInfo.Ypos;
            Vector3 FirstPosition = FirstCard.transform.position;

            FirstCardInfo.Xpos = SecondCardInfo.Xpos;
            FirstCardInfo.Ypos = SecondCardInfo.Ypos;
            FirstCard.transform.position = SecondCard.transform.position;
            CardArray[FirstCardInfo.Xpos, FirstCardInfo.Ypos] = FirstCard;

            SecondCardInfo.Xpos = FirstXpos;
            SecondCardInfo.Ypos = FirstYpos;
            SecondCardInfo.transform.position = FirstPosition;
            CardArray[SecondCardInfo.Xpos, SecondCardInfo.Ypos] = SecondCard;

        CardString = AssembleCardString();
        }
    }

    private GameObject CardArrayExist()
    {
        int tempX = Random.Range(0, CardArray.GetLength(0));
        int tempY = Random.Range(0, CardArray.GetLength(1));
        if (CardArray[tempX, tempY] != null)
        {
            return CardArray[tempX, tempY];
        }
        return CardArrayExist();
    }

    private string AssembleCardString()
    {
        string TempString = "";
        for (int i = 0; i < CardArray.GetLength(0); i++)
        {
            for (int j = 0; j < CardArray.GetLength(1); j++)
            {
                if (CardArray[i, j] != null)
                {
                switch (CardArray[i, j].GetComponent<CardInfo>().SUIT)
                {
                    case 1:
                        TempString += "_1";
                        break;
                    case 2:
                        TempString += "_2";
                        break;
                    case 3:
                        TempString += "_3";
                        break;
                    case 4:
                        TempString += "_4";
                        break;
                }
                int tempInt = CardArray[i, j].GetComponent<CardInfo>().NUMBER;
                if (tempInt < 10)
                {
                    TempString += "0" + tempInt;
                }
                else
                {
                    TempString += tempInt;
                }
                }
                else
                {
                    TempString += "0000";
                }
            }
        }
        return TempString;
    }
    public void DisassembleCardString(string AllString)
    {
        for (int index = 0; index < 100; index++)
        {
            string tempString = AllString.Substring(index * 4, 4);
            if (tempString.StartsWith("_"))
            {
                GameObject tempGameObject = CardArray[index / 10, index % 10];
                int tempNumber = int.Parse(tempString.Substring(2, 2));
                int tempSuit = int.Parse(tempString.Substring(1, 1));
                tempGameObject.GetComponent<CardInfo>().NUMBER = tempNumber;//14
                tempGameObject.GetComponent<CardInfo>().SUIT = tempSuit;//2
                int tempIndex = tempNumber != 14 ? (tempSuit - 1) * 13 + tempNumber - 1 : (tempSuit - 1) * 13 + tempNumber - 14;
                tempGameObject.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = CardFront[tempIndex];
            }
        }
    }
}
