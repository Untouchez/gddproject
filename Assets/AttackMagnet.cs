using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMagnet : MonoBehaviour
{
    public List<Health> Close; //gets called by player from attack function
    public static AttackMagnet instance;

    public void Awake()
    {
        instance = this;
    }

    public void OnTriggerEnter(Collider other)
    {
        Health newThing = other.transform.GetComponent<Health>();
        if (newThing)
            Close.Add(newThing);
    }

    public void OnTriggerExit(Collider other)
    {
        Health newThing = other.transform.GetComponent<Health>();
        if(newThing)
            Close.Remove(newThing);
    }
}
