using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform toFollow;
    public Vector3 offset = new Vector3(0,100,0);

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = toFollow.position+offset;
    }
}
