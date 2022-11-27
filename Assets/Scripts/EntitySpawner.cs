using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour {
    public bool isActive;
    public abstract void Spawn(int diff);
}

public class EntitySpawner : Spawner
{
    public int minDiff;
    public GameObject entity;
    public Vector2Int quantityRange;
    public Vector2 spawnSize;
    public bool randomizeRotation;

    public override void Spawn(int diff) {
        if (diff >= minDiff) {
            int n = Random.Range(quantityRange.x, quantityRange.y+1);

            bool hasCollider = entity.GetComponent<Collider>();

            for (int i=0; i<n; i++) {
                Vector3 posOffset = new Vector3(
                    Random.Range(-spawnSize.x/2, spawnSize.x/2),
                    0,
                    Random.Range(-spawnSize.y/2, spawnSize.y/2)
                );
                Vector3 pos = transform.position + posOffset;
                
                RaycastHit hit;
                Physics.Raycast(pos + Vector3.up * 10, Vector3.down, out hit, 100, ~LayerMask.NameToLayer("Floor"));
                pos = hit.point;

                GameObject instance = Instantiate(entity, pos, transform.rotation, transform);
                if (hasCollider) instance.GetComponent<Collider>().enabled = false;
                if (randomizeRotation) {
                    instance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                }
            }

            if (hasCollider) {
                foreach(Transform instance in transform) {
                    instance.GetComponent<Collider>().enabled = true;
                }
            }
        }

        Destroy(this);
    }
}
