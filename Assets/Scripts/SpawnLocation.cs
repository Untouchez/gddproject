using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnLocation : MonoBehaviour
{
    public List<SpawnLocation> allSpawns;

    public ObjectPool objectPool;
    public NPC npc;
    public float spawnRadius;
    public float fireRate;
    private float lastFired;

    private float newFireRate;

    // Start is called before the first frame update
    void Start()
    {
        newFireRate = fireRate;
        allSpawns = new List<SpawnLocation>(FindObjectsOfType<SpawnLocation>());
        allSpawns.Remove(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastFired > 1 / newFireRate)
        {
            print("spawnLocation");
            lastFired = Time.time;
            SpawnNPC();
        }
    }

    void SpawnNPC()
    {
        newFireRate = Random.Range(fireRate / 2, fireRate);

        NPC currentnpc = objectPool.GetPooledObject();

        currentnpc.transform.position = GetRandomSpotNear();
        currentnpc.gameObject.SetActive(true);

        SpawnLocation destination = allSpawns[Random.Range(0, allSpawns.Count)];
        Vector3 destinationPoint = destination.GetRandomSpotNear();
        currentnpc.destination = destinationPoint;
        currentnpc.agent.SetDestination(destinationPoint);
    }

    public Vector3 GetRandomSpotNear()
    {
        Vector3 randomSpot = transform.position + Random.insideUnitSphere * 5f;
        randomSpot.y = 0;

        if (NavMesh.SamplePosition(randomSpot, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            return hit.position;
        return randomSpot;
    }

    public void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
    }
}
