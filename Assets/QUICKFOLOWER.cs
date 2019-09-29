using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QUICKFOLOWER : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if(target == null) return;
        transform.position = target.position;
    }
}
