using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;
    public float acceleration = 2;
    public float decceleration = 4;
    public Vector3 rawInput;
    public Vector3 input;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        anim.SetFloat("InputX", input.x);
        anim.SetFloat("InputY", input.y);
    }

    public void Inputs()
    {
        rawInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        if (rawInput != Vector3.zero) {
            input = Vector3.Slerp(input, rawInput, acceleration*Time.deltaTime);
        }else
            input = Vector3.Slerp(input, Vector3.zero, decceleration*Time.deltaTime);
    }


}
