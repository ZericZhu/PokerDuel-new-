using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public static CameraFollow Instance;
    public Vector3 Offset;
    public float followspeed;
    private void Start()
    {
        Instance = this;
        Offset = transform.position - target.position;
    }
    private void LateUpdate()
    {
        Vector3 desirePos = target.position + Offset;
        Vector3 LerpPos = Vector3.Lerp(transform.position, desirePos, followspeed);
        transform.position = LerpPos;

    }
}
