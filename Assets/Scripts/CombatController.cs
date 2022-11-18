using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatController : MonoBehaviour, LivingCreature
{
    public float health = 5;
    private bool dead = false;
    Weapon weapon;

    void Start()
    {
        weapon = GetComponentInChildren<Weapon>();
        weapon.SetOwner(gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            weapon.StartAttacking();
        }
    }

    public void Damage(float d)
    {
        d -= d/37 * FindObjectOfType<MissionManager>().def;
        health -= d;
        if (health < 0.01 && !dead) {
            dead = true;
            FindObjectOfType<MissionManager>().Death();
        }
    }
}
