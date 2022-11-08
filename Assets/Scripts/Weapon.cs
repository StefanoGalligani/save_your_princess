using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 2;
    private bool isAttacking = false;
    private Animator anim;
    private GameObject owner;

    void Start() {
        anim = GetComponent<Animator>();
    }

    public void SetOwner(GameObject g) {
        owner = g;
    }

    public void StartAttacking() {
        if (isAttacking) return;
        isAttacking = true;
        anim.Play("Attack");
        GetComponent<Collider>().enabled = true;
    }

    public void FinishAttacking() {
        isAttacking = false;
        GetComponent<Collider>().enabled = false;
    }

    public void OnTriggerEnter(Collider coll) {
        if (isAttacking && coll.GetComponent<LivingCreature>() != null) {
            if (!coll.gameObject.Equals(owner))
                coll.gameObject.GetComponent<LivingCreature>().Damage(damage);
        }
    }
}
