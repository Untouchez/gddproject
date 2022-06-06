using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform[] models;
    public AnimationClip[] walkingClips;
    public Vector3 destination;

    Animator anim;
    AnimatorOverrideController animatorOverrideController;

    public bool isImposter;

    void Start()
    {
        anim = GetComponent<Animator>();

        animatorOverrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animatorOverrideController;
        animatorOverrideController["walk"] = walkingClips[Random.Range(0, walkingClips.Length)];

        Transform randomModel = models[Random.Range(0, models.Length)];
        randomModel.gameObject.SetActive(true);

        isImposter = Random.Range(0, 1f) <= 0.1f;

        agent.speed = Random.Range(2, 5);

        DisableRagdoll();
    }

    void Update()
    {
        if (isImposter)
        {
            if (Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance + 15f)
            {
                float walkRadius = Random.Range(400, 800);
                Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
                randomDirection += transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
                Vector3 finalPosition = hit.position;

                agent.SetDestination(finalPosition);
            }
            return;
        }

        if (Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance + 5f)
        {
            this.gameObject.SetActive(false);
        }
    }
    public bool isTripped;
    public IEnumerator trip()
    {
        if (isTripped)
            yield break;
        anim.ApplyBuiltinRootMotion();
        anim.SetTrigger("trip");
        isTripped = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(20f);
        agent.isStopped = false;
        anim.applyRootMotion = false;
        isTripped = false;
    }
    void OnEnable()
    {
        anim = GetComponent<Animator>();
        anim.Play("walk", 0, Random.Range(0, 0.4f));
        anim.SetBool("isWalking", true);
    }

    void DisableRagdoll()
    {
        //avoid using getcomponentsinchildren only on startup
        Rigidbody[] rb = GetComponentsInChildren<Rigidbody>();
        Collider[] cb = GetComponentsInChildren<Collider>();
        foreach (Rigidbody ri in rb)
        {
            ri.isKinematic = true;
        }
        foreach (Collider col in cb)
        {
            col.enabled = false;
        }

        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;
    }
}
