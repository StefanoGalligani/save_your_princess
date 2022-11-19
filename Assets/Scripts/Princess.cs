using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Princess : MonoBehaviour
{
    
    void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.forward = new Vector3(transform.forward.x, 0, transform.forward.z);
    }
}
