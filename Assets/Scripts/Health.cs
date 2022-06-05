using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public abstract void TakeDamage(int damage);

    public abstract void Heal(int heal);
}
