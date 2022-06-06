using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : State
{
    public Animator anim;
    public NavMeshAgent agent;
    public State Combat;
    public float attackRange;
    Player player;
    public void Start()
    {
        player = Player.instance;
    }

    public override void MyStart()
    {

    }

    public override void MyUpdate()
    {
        agent.SetDestination(player.transform.position);
        anim.SetFloat("speed", agent.velocity.sqrMagnitude / agent.speed);
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange) {
            myStateManager.ChangeState(Combat);
        }
    }
}
