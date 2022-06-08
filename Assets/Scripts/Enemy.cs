using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Health
{
    public StateManager myStateManager;
    public Hitbox hitBox;

    public int damage;
    public override void Heal(int heal)
    {

    }
        

    public override void TakeDamage(int damage)
    {

    }

    public void Attack()
    {
        hitBox.EnableCollider(damage);
        StartCoroutine(closeCollider());
    }

    public IEnumerator closeCollider()
    {
        yield return new WaitForSeconds(0.1f);
        hitBox.DisableCollider();
        
    }
}
