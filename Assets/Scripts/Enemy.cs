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
    public Vector2 thresholdDistances = new Vector2(15, 25);

    float counter = 0;
    Weapon weapon;
    Transform target;
    Rigidbody rb;
    bool followTarget = false;
    bool stunned = false;

    void Start() {
        target = Camera.main.transform.parent;
        rb = GetComponent<Rigidbody>();
        weapon = GetComponentInChildren<Weapon>();
        weapon.SetOwner(gameObject);
    }

    void Update() {
        float sqrDist = (target.position - transform.position).sqrMagnitude;
        if (sqrDist <= thresholdDistances.x*thresholdDistances.x) followTarget = true;
        if (sqrDist >= thresholdDistances.y*thresholdDistances.y) followTarget = false;
    }

    void FixedUpdate() {
        if (followTarget && !stunned) {
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

    public void Heal(float h) {
        health += h;
    }

    public void Stun() {
        stunned = true;
        StartCoroutine(RemoveStun());
    }

    private IEnumerator RemoveStun() {
        yield return new WaitForSeconds(3);
        stunned = false;
    }

    void LateUpdate()
    {
//        transform.forward = Camera.main.transform.forward;
        transform.LookAt(Camera.main.transform);

        transform.forward = new Vector3(transform.forward.x, 0, transform.forward.z);
    }
}
