using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : State
{
    public Animator anim;
    public NavMeshAgent agent;
    public Transform me;

    public State chase;
    public float patrolDistance;
    public LayerMask ignore;
    public float test;
    Vector3 myPos;
    Vector3 targetPos;

    Player player;
    public void Start()
    {
        player = Player.instance;
        myPos = transform.position;
        targetPos = transform.position;
    }   

    public override void MyStart()
    {
        myPos = transform.position;
        targetPos = transform.position;
    }

    public override void MyUpdate()
    {
        Vector3 dir = me.forward.normalized;
        Vector3 dirToPlayer = (player.transform.position - me.position).normalized;
        test = Vector3.Dot(dir, dirToPlayer);
        if (Vector3.Dot(dir, dirToPlayer) >= 0.2f) //if player is infront
        {
            myStateManager.ChangeState(chase);
        } else {
            if(Vector3.Distance(me.position,targetPos) <= 0.1 + agent.stoppingDistance)
            {
                GoToRandomSpot();
            }
        }
        anim.SetFloat("speed", agent.desiredVelocity.magnitude / agent.speed);
    }

    public void GoToRandomSpot()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolDistance;
        randomDirection += myPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolDistance, 1);
        agent.SetDestination(hit.position);
        targetPos = hit.position;
    }

    public bool CanSeePlayer()
    {
        if(Physics.Raycast(transform.position+new Vector3(0,1,0), player.transform.position + new Vector3(0,1,0),out RaycastHit hit,Mathf.Infinity, ~ignore))
        {
            print(hit.transform);
            if (hit.transform.CompareTag("Player"))
                return true;
        }
        return false;
    }
}
