using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpReaction : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Car"){
            Debug.Log("Scramble!!!");
            if (SeedManager.instance != null) SeedManager.instance.ScrambleCarPlaces(other.GetComponent<PlayerController>().id);
        }
    }
}
