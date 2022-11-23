using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatController : MonoBehaviour, LivingCreature
{
    public float maxHealth = 5;
    private float health;
    private bool dead = false;
    Weapon weapon;
    Princess p;
    SpriteRenderer pSprite;
    private bool holdingPrincess = false;

    void Start()
    {
        health = maxHealth;
        pSprite = Camera.main.GetComponentInChildren<SpriteRenderer>();
    }

    public void SetWeapon(Weapon w) {
        weapon = w;
        weapon.SetOwner(gameObject);
    }

    void Update()
    {
        if (!holdingPrincess && Input.GetMouseButtonDown(0)) {
            weapon.StartAttacking();
        }
        if (Input.GetMouseButtonDown(1)) {
            FindObjectOfType<MissionManager>().SlowTime();
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            if (!p) p = FindObjectOfType<Princess>();
            if (!holdingPrincess && (p.transform.position-transform.position).sqrMagnitude > 5) return;
            SetHoldingPrincess(!holdingPrincess);
        }
    }

    private void SetHoldingPrincess(bool h) {
        pSprite.enabled = h;
        weapon.gameObject.SetActive(!h);
        p.gameObject.SetActive(!h);
        p.transform.position = transform.position + transform.forward*0.2f;
        holdingPrincess = h;
    }

    public void Respawn() {
        health = maxHealth;
        StartCoroutine(InvincibilityTime());
    }

    private IEnumerator InvincibilityTime() {
        yield return new WaitForSeconds(0.5f);
        dead = false;
    }

    public void Damage(float d)
    {
        if (dead) return;

        d -= d/37 * FindObjectOfType<MissionManager>().def;
        health -= d;

        if (holdingPrincess) {
            SetHoldingPrincess(false);
        }

        if (health < 0.01) {
            dead = true;
            FindObjectOfType<MissionManager>().Death();
        }
    }

    public void OnTriggerEnter(Collider coll) {
        if (coll.gameObject.layer == LayerMask.NameToLayer("WinTrigger"))
            if (holdingPrincess) FindObjectOfType<MissionManager>().Win();
    }
}
