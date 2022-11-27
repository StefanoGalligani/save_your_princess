using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 2;
    public float maxTime = 5;
    public float speed;
    private GameObject owner;

    public void SetOwner(GameObject g) {
        owner = g;
    }

    public void Shoot(Vector3 dir) {
        GetComponent<Rigidbody>().velocity = dir*speed;
    }

    void Update() {
        maxTime -= Time.deltaTime;
        if (maxTime <= 0) Destroy(gameObject);
    }

    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.forward = new Vector3(transform.forward.x, 0, transform.forward.z);
    }

    public void OnTriggerEnter(Collider coll) {
        if (coll.GetComponent<LivingCreature>() != null) {
            if (!coll.gameObject.Equals(owner)) {
                coll.gameObject.GetComponent<LivingCreature>().Damage(damage);
                StartCoroutine(PlayAndDestroy());
            }
        }
    }

    private IEnumerator PlayAndDestroy() {
        GetComponent<AudioSource>().volume = GlobalSfxVolume.sfxVolume;        
        GetComponent<AudioSource>().Play();

        GetComponent<Collider>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(0.5f);
        
        Destroy(gameObject);
    }
}
