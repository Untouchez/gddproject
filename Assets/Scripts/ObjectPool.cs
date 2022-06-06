using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public Queue<NPC> pooledObjects;
    public NPC objectToPool;
    public int poolSize;

    private void Start()
    {
        pooledObjects = new Queue<NPC>();
        NPC newGameObject;

        for (int i = 0; i < poolSize; i++)
        {
            newGameObject = Instantiate(objectToPool);
            newGameObject.gameObject.SetActive(false);
            pooledObjects.Enqueue(newGameObject);
        }
    }

    public NPC GetPooledObject()
    {
        return pooledObjects.Dequeue();
    }

    public void InstantiateNewObject(Vector3 newPos)
    {
        NPC newPooledObject = GetPooledObject();
        newPooledObject.gameObject.SetActive(true);
        newPooledObject.transform.position = newPos;
    }

}
