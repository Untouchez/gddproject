using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public static Transform instance;
    public float rotSpeed;
    // Update is called once per frame

    void Awake()
    {
        instance = this.transform;
    }
    void Update()
    {
        transform.position = target.position;
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") *Time.deltaTime*rotSpeed, 0));
    }
}
