using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isAttacking;
    [HideInInspector] public bool canMove;

    Animator anim;
    Transform cameraFollow;
    float acceleration = 10;
    float decceleration = 12;
    Vector3 rawInput;
    Vector3 input;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        cameraFollow = CameraFollow.instance;
    }

    // Update is called once per frame
    void Update()
    {
        Locomotion();
        Aiming();
        if (Input.GetMouseButtonDown(0))
            Attack();
    }

    public void Attack()
    {
        anim.SetTrigger("attack");
        StartCoroutine(isAttackingCheck());
    }

    IEnumerator isAttackingCheck()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.21f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f || !anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"));
        isAttacking = false;
    }

    public void Aiming()
    {
        transform.forward = cameraFollow.forward;
    }

    public void Locomotion()
    {
        rawInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        if (rawInput != Vector3.zero)
            input = Vector3.Slerp(input, rawInput, acceleration * Time.deltaTime);
        else
            input = Vector3.Slerp(input, Vector3.zero, decceleration * Time.deltaTime);

        if (canMove)
            return;
        anim.SetFloat("InputX", input.x);
        anim.SetFloat("InputY", input.y);
    }

    public void FootR()
    {

    }

    public void FootL()
    {

    }
}
