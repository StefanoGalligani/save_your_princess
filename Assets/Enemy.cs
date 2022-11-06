using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3;
    public float distanceToAttack = 2;
    public float health = 5;
    Transform target;
    bool followTarget = true;

    void Start() {
        target = Camera.main.transform.parent;
    }

    void Update() {
        if (followTarget) {
            Vector3 direction = (target.position - transform.position);
            if (direction.sqrMagnitude > distanceToAttack*distanceToAttack) {
                GetComponent<Rigidbody>().MovePosition(transform.position + direction.normalized * Time.deltaTime * speed);
            } else {

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
