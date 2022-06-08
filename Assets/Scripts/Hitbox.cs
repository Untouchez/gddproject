using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public ParticleSystem hitEffect;
    Collider collider;
    int damage;

    public float atackForce;
    public void Start()
    {
        collider = GetComponent<Collider>();
        DisableCollider();
    }

    public void EnableCollider(int newDamage)
    {
        damage = newDamage;
        collider.enabled = true;
    }

    public void DisableCollider()
    {
        collider.enabled = false;
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.GetComponent<Health>() != null && !collision.CompareTag(transform.root.tag)) {
            print("hit: " + collision.transform);
            hitEffect.transform.position = collision.ClosestPoint(collision.transform.position) + new Vector3(0,0.1f,0);
            hitEffect.Play(true);
            collision.transform.GetComponent<Health>().TakeDamage(damage);
            collision.transform.GetComponent<Animator>().SetTrigger("Take Damage");
            collision.GetComponent<Rigidbody>().AddForce(transform.forward * atackForce);
        }
    }
}
