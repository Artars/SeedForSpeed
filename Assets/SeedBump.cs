using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedBump : MonoBehaviour
{
    public int seeds = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Car"){
            PlayerController player = other.GetComponent<PlayerController>();
            player.seedGain(seeds);
            Destroy(gameObject);
        }
    }
}
