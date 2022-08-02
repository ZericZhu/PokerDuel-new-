using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CubeScript : MonoBehaviour
{
    public float followspeed;

    private int[] mySUITarray = new int[5];
    private int[] myNUMBERarray = new int[5];
    public GameObject slotParent;//main/minor
    public Text myScoreDisplay;//main/minor
    public int myCurrentIndex = 0;
    public int myScore = 0;
    private Coroutine MyInventoryCo = null;//main/minor
    private bool MyUIisPlaying = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CardInfo>() != null)
        {
            if (MyUIisPlaying)
            {
                StopCoroutine(MyInventoryCo);
                slotParent.GetComponent<Animator>().SetInteger("State", 0);
                //clear the ui
                for (int i = 0; i < slotParent.transform.childCount; i++)
                {
                    slotParent.transform.GetChild(i).GetComponent<Image>().material = GameMan.instance.UISlotEmpty;
                }
                MyUIisPlaying = false;
            }

            CardInfo tempCardInfo = other.gameObject.GetComponent<CardInfo>();

            mySUITarray[myCurrentIndex] = tempCardInfo.SUIT;
            myNUMBERarray[myCurrentIndex] = tempCardInfo.NUMBER;

            GameMan.instance.FlipAroundPos(tempCardInfo.Xpos, tempCardInfo.Ypos);

            slotParent.transform.GetChild(myCurrentIndex).GetComponent<Image>().material
                = other.transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material;

            myCurrentIndex++;
            if (myCurrentIndex == 5)
            {
                GameMan.instance.CalculateScore(ref myScore, mySUITarray, myNUMBERarray);
                myCurrentIndex = 0;
                MyInventoryCo = StartCoroutine(InventoryAnim(slotParent));
                myScoreDisplay.text = "" + myScore;
            }
            Destroy(other.gameObject);

            CameraFollow.instance.GetComponent<Camera>().fieldOfView += 1f;

            PlayerScript.instance.CmdSwitchTurn();
        }
    }



    IEnumerator InventoryAnim(GameObject temp_slotParent)
    {
        MyUIisPlaying = true;
        yield return null;

        //UIEffect
        slotParent.GetComponent<Animator>().SetInteger("State", 1);
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime;
            yield return null;
        }
        slotParent.GetComponent<Animator>().SetInteger("State", 0);
        //clear the ui
        for (int i = 0; i < slotParent.transform.childCount; i++)
        {
            slotParent.transform.GetChild(i).GetComponent<Image>().material = GameMan.instance.UISlotEmpty;
        }
        MyUIisPlaying = false;
    }







    public void StartFalling()
    {
        StartCoroutine(StartFallingCo());
    }

    public IEnumerator StartFallingCo()
    {
        float time = 0, duration = 1;
        Vector3 desirePos = new Vector3(-10, 5, 0);
        while (time < duration)
        {
            Vector3 LerpPos = Vector3.Lerp(transform.position, desirePos, followspeed);
            transform.position = LerpPos;
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = desirePos;
    }
}
