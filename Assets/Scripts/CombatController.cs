using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatController : MonoBehaviour, LivingCreature
{
    public float health = 5;
    private bool dead = false;
    Weapon weapon;
    Princess p;
    private bool holdingPrincess = false;

    void Start()
    {
        weapon = GetComponentInChildren<Weapon>();
        weapon.SetOwner(gameObject);
    }

    void Update()
    {
        if (!holdingPrincess && Input.GetMouseButtonDown(0)) {
            weapon.StartAttacking();
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            if (!p) p = FindObjectOfType<Princess>();
            if (!holdingPrincess && (p.transform.position-transform.position).sqrMagnitude > 5) return;
            SetHoldingPrincess(!holdingPrincess);
        }
    }

    private void SetHoldingPrincess(bool h) {
        weapon.gameObject.SetActive(!h);
        p.gameObject.SetActive(!h);
        p.transform.position = transform.position + transform.forward*0.2f;
        holdingPrincess = h;
    }

    public void Damage(float d)
    {
        d -= d/37 * FindObjectOfType<MissionManager>().def;
        health -= d;        
        if (holdingPrincess) {
            SetHoldingPrincess(false);
        }

        if (health < 0.01 && !dead) {
            dead = true;
            FindObjectOfType<MissionManager>().Death();
        }
    }

    public void OnTriggerEnter(Collider coll) {
        if (coll.gameObject.layer == LayerMask.NameToLayer("WinTrigger"))
            if (holdingPrincess) FindObjectOfType<MissionManager>().Win();
    }
}
