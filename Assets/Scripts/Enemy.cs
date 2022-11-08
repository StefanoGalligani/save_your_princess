using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, LivingCreature
{
    public float speed = 3;
    public float distanceToAttack = 2;
    public float health = 5;
    public float attackTime = 1;
    public float firstAttackTime = .2f;

    float counter = 0;
    Weapon weapon;
    Transform target;
    Rigidbody rb;
    bool followTarget = true;

    void Start() {
        target = Camera.main.transform.parent;
        rb = GetComponent<Rigidbody>();
        weapon = GetComponentInChildren<Weapon>();
        weapon.SetOwner(gameObject);
    }

    void FixedUpdate() {
        if (followTarget) {
            Vector3 direction = (target.position - transform.position);
            direction.y = 0;
            if (direction.sqrMagnitude > distanceToAttack*distanceToAttack) {
                rb.MovePosition(rb.position + direction.normalized * Time.fixedDeltaTime * speed);
                counter = attackTime - firstAttackTime;
            } else {
                counter += Time.fixedDeltaTime;
                if (counter >= attackTime) {
                    counter = 0;
                    weapon.StartAttacking();
                }
            }
        }
    }

    public void Damage(float d) {
        health -= d;
        if (health < 0.001f) {
            Destroy(gameObject);
        }
    } 

    void LateUpdate()
    {
//        transform.forward = Camera.main.transform.forward;
        transform.LookAt(Camera.main.transform);

        transform.forward = new Vector3(transform.forward.x, 0, transform.forward.z);
    }
}
