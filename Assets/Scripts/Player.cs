using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public Transform rotTarget;
    public AttackMagnet attackMagnet;
    public Hitbox rf; //right fist
    public Hitbox lf; //left fist
    public Hitbox sw; //sword

    public float speed;
    public bool isAttacking;
    Rigidbody rb;
    Animator anim;
    Transform cameraFollow;
    public float rotSpeed;
    float acceleration = 10;
    float decceleration = 12;
    Vector3 rawInput;
    Vector3 input;
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
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

    private void FixedUpdate()
    {
        if (CanMove()) {
            Vector3 direction = new Vector3(input.x, 0, input.y);
            rb.velocity = transform.TransformDirection(direction * speed * Time.deltaTime);
        }
        else
            rb.velocity = Vector3.zero;
    }

    public void Attack()
    {
        rb.velocity = Vector3.zero; //lose momentum

        if (attackMagnet.Close.Count != 0) { //if theres an enemy close
            Vector3 targetPos = attackMagnet.Close[0].transform.position; 
            Vector3 targetDir = (transform.position - targetPos).normalized;
            rotTarget = attackMagnet.Close[0].transform;
            transform.DOMove(targetPos + targetDir, 0.5f); //take 0.5seconds to move to the enemy
        }
        anim.SetTrigger("attack");
        StartCoroutine(isAttackingCheck());
    }

    IEnumerator isAttackingCheck()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.21f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f || !anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"));
        rotTarget = null;
        isAttacking = false;
    }

    public void Aiming()
    {
        if (rotTarget) {
            transform.forward = Vector3.Slerp(transform.forward, (rotTarget.position - transform.position).normalized, rotSpeed * Time.deltaTime) ;
            return;
        }
        if (CanMove()) {
            transform.forward = Vector3.Slerp(transform.forward, cameraFollow.forward, rotSpeed * Time.deltaTime);
            return;
        }

    }

    public void Locomotion()
    {
        if (!CanMove()) {
            input = Vector3.Slerp(input, Vector3.zero, decceleration * Time.deltaTime);
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputY", 0);
            return;
        }

        rawInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        if (rawInput != Vector3.zero)
            input = Vector3.Slerp(input, rawInput, acceleration * Time.deltaTime);
        else
            input = Vector3.Slerp(input, Vector3.zero, decceleration * Time.deltaTime);

        anim.SetFloat("InputX", input.x);
        anim.SetFloat("InputY", input.y);
    }

    public bool CanMove()
    {
        return !isAttacking;
    }

    public void OpenCollider(string where)
    {
        int damageMultiplier = where[2] - '0';
        string hitBoxToEnable = where.Substring(0, 2);
        switch (hitBoxToEnable)
        {
            case "rf":
                rf.EnableCollider(damageMultiplier);
                break;
            case "lf":
                lf.EnableCollider(damageMultiplier);
                break;          
            case "sw":
                sw.EnableCollider(damageMultiplier);
                break;
        }
    }    

    public void CloseColliders()
    {
        rf.DisableCollider();
        lf.DisableCollider();
        sw.DisableCollider();
    }

    public void FootR()
    {

    }

    public void FootL()
    {

    }
}
