using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    Weapon weapon;
    void Start()
    {
        weapon = GetComponentInChildren<Weapon>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            weapon.StartAttacking();
        }
    }
}
