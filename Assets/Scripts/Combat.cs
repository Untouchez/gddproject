using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Combat : State
{
    public Animator anim;
    public NavMeshAgent agent;
    public State chasePlayer;

    public float rotSpeed;
    public float attackRange;
    public string myAttack;

    public float test;
    public bool test2;

    Player player;
    public override void MyStart()
    {
        player = Player.instance;
    }

    public override void MyUpdate()
    {
        test2 = isAttacking();
        Vector3 dir = anim.transform.forward.normalized;
        Vector3 dirToPlayer = (player.transform.position - anim.transform.position);
        test = Vector3.Dot(dir, dirToPlayer);

        if (Vector3.Distance(anim.transform.position, player.transform.position) >= attackRange && !isAttacking()) {
            anim.ResetTrigger("attack");
            anim.SetTrigger("cancel");
            myStateManager.ChangeState(chasePlayer);
        } else {
            dir = anim.transform.forward.normalized;
            dirToPlayer = (player.transform.position - anim.transform.position).normalized;
            agent.SetDestination(transform.position);
            if (Vector3.Dot(dir,dirToPlayer) >= 0.9f) {
                anim.SetTrigger("attack");
                agent.SetDestination(transform.position);
            } else {
                anim.ResetTrigger("attack");
                RotateToPlayer();
            }
        }
    }

    public bool isAttacking()
    {
        return (anim.GetCurrentAnimatorStateInfo(0).IsName(myAttack));
    }

    public void RotateToPlayer()
    {
        // Determine which direction to rotate towards
        Vector3 targetDirection = player.transform.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = rotSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(anim.transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        anim.transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
