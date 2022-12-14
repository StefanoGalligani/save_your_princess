using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 2;
    public GameObject projectile;
    public bool vampire = false;
    public bool air = false;
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

    public void ShootProjectile() {
        GameObject p = Instantiate(projectile);
        p.transform.position = transform.position + owner.transform.forward*0.5f;
        p.GetComponent<Projectile>().SetOwner(owner);
        p.GetComponent<Projectile>().Shoot(owner.transform.forward);
    }

    public void OnTriggerEnter(Collider coll) {
        if (isAttacking && coll.GetComponent<LivingCreature>() != null) {
            if (!coll.gameObject.Equals(owner)) {
                coll.gameObject.GetComponent<LivingCreature>().Damage(damage);
                if (!air) GetComponent<Collider>().enabled = false;

                if (GetComponent<AudioSource>()) {
                    GetComponent<AudioSource>().volume = GlobalSfxVolume.sfxVolume;  
                    GetComponent<AudioSource>().Play();
                }
                
                if (vampire)
                    owner.GetComponent<LivingCreature>().Heal(damage/3);
            }
        }
    }
}
