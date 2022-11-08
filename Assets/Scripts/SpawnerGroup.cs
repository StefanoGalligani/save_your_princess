using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerGroup : Spawner
{
    public Spawner[] spawnerArray;
    public bool randomOrder;
    public override void Spawn(int diff) {
        for (int i=0; i<spawnerArray.Length; i++) {
            Transform child = transform.GetChild(!randomOrder ? 0 : Random.Range(0, transform.childCount));

            spawnerArray[i].transform.position = child.position;
            spawnerArray[i].transform.rotation = child.rotation;

            child.SetParent(null);
            Destroy(child.gameObject);
            spawnerArray[i].Spawn(diff);
        }

        Destroy(this.gameObject);
    }
}
