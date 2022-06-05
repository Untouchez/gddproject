using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Health
{
    Animator anim;

    public void Start()
    {
        anim = GetComponent<Animator>(); 
    }
    public override void Heal(int heal)
    {
        
    }

    public override void TakeDamage(int damage)
    {
        print(transform + " take damage: " + damage);
        anim.SetTrigger("Take Damage");
    }
}
