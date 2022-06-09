using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : Health
{
    public static Player instance;
    public Transform rotTarget;
    public Hitbox rf; //right fist
    public Hitbox lf; //left fist
    public Hitbox sw; //sword
    public float rotSpeed;
    public bool isRolling;
    public float rollForce;
    public float speed;
    public bool isAttacking;

    Vector3 rollDirection;
    Rigidbody rb;
    Animator anim;
    AttackMagnet attackMagnet;
    Transform cameraFollow;
    float acceleration = 10;
    float decceleration = 21;
    Vector3 rawInput;
    Vector3 input;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        attackMagnet = AttackMagnet.instance;
        cameraFollow = CameraFollow.instance;
    }

    // Update is called once per frame
    void Update()
    {
        Locomotion();

        if (Input.GetMouseButtonDown(0) && CanAttack())
            Attack();

        if (Input.GetKeyDown(KeyCode.LeftShift) && rawInput!=Vector3.zero && CanMove())
            Roll();
    }

    private void FixedUpdate()
    {
        Aiming();

        if (isRolling)
            return;
        if (rawInput == Vector3.zero) {
            rb.velocity = Vector3.zero;
        } else {
            Vector3 direction = new Vector3(input.x, 0, input.y).normalized;
            rb.velocity = transform.TransformDirection(direction * speed);
        }
    }

    public void Attack()
    {    //called when youcclick mouse during update
         //stop moving
         //find closest viable target
         //move to the target
         //play attack anim

        rb.velocity = Vector3.zero; //lose momentum

        if (attackMagnet.Close.Count != 0) { //if theres an enemy close

            //take the Dot product of each enemy to see which object inside of Close is closest to the forward of the player
            Health closet = attackMagnet.Close[0];
            float maxLook = 0f;
            foreach(Health target in attackMagnet.Close) {
                float lookDir;
                if (new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) != Vector3.zero)  // if theres wasd movement
                    lookDir = Vector3.Dot(cameraFollow.TransformDirection(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))), (target.transform.position - transform.position).normalized);
                else
                    lookDir = Vector3.Dot(cameraFollow.forward.normalized, (target.transform.position - transform.position).normalized);
                if(lookDir >= maxLook) {
                    closet = target;
                    maxLook = lookDir;
                }
            }
            rotTarget = closet.transform;

            //move the player 1unit away from the target
            Vector3 targetPos = rotTarget.transform.position; 
            Vector3 targetDir = (transform.position - targetPos).normalized;

            transform.DOMove(targetPos + targetDir, 0.5f); //take 0.5 seconds to move to the enemy
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

    public void Locomotion()
    {
        //Called during update
        //gets wasd input and calculates momentum
        //moves player using rigibody
        //sets floats in animator to play moving animations
        rawInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);

        //if cant move stop moving player reset input
        if (!CanMove()) {
            input = Vector3.Slerp(input, Vector3.zero, decceleration * Time.deltaTime);
            anim.SetFloat("InputX", 0);
            anim.SetFloat("InputY", 0);
            return;
        }

        //change input and animations
        if (rawInput != Vector3.zero)
            input = Vector3.Slerp(input, rawInput, acceleration * Time.deltaTime);
        else
            input = Vector3.Slerp(input, Vector3.zero, decceleration * Time.deltaTime);

        anim.SetFloat("InputX", input.x);
        anim.SetFloat("InputY", input.y);
    }

    public void Aiming()
    {
        //gets called during fixed update
        //if therse a rot target, override the rotation and rotate towards the target
        //otherwise rotate towards the mouse

        if (isRolling)
        {
            transform.forward = Vector3.Slerp(transform.forward, rollDirection, rotSpeed * Time.deltaTime);
            return;
        }
        if (rotTarget)
        {
            transform.forward = Vector3.Slerp(transform.forward, (rotTarget.position - transform.position).normalized, rotSpeed * Time.deltaTime);
            return;
        }
        if (CanMove())
        {
            transform.forward = Vector3.Slerp(transform.forward, cameraFollow.forward, rotSpeed * Time.deltaTime);
            return;
        }

    }

    public void Roll() {
        rollDirection = transform.TransformDirection(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")));
        StartCoroutine(isRollingCheck());
        anim.SetTrigger("roll");
        rb.AddForce(rollDirection * rollForce);
    }

    IEnumerator isRollingCheck()
    {
        isRolling = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("roll") || anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.89f);
        isRolling = false;
    }

    public bool CanMove()
    {
        return !isAttacking && !isRolling;
    }

    public bool CanAttack()
    {
        return !isRolling;
    }

    public bool CanRoll()
    {
        return rawInput != Vector3.zero && !isRolling && !isAttacking;
    }
    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    public override void Heal(int heal)
    {

    }

    //called during when an animation is played
    #region AnimationEvents
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
    #endregion
}
