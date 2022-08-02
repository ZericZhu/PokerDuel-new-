using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CubeScript : MonoBehaviour
{
    public float followspeed;

    private int[] mySUITarray = new int[5];
    private int[] myNUMBERarray = new int[5];
    public GameObject slotParent;
    public Text myScoreDisplay;
    private bool MyUIisPlaying;

    private void OnTriggerEnter(Collider other)
    {
        GameMan.instance.CubeEnter(other);
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
