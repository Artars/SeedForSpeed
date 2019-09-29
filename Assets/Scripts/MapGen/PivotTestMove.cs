using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotTestMove : MonoBehaviour{

    public float speed;

    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        if(Input.GetAxis("Horizontal") > 0.0f){
            transform.position += Vector3.right * speed;
        }else if(Input.GetAxis("Horizontal") < 0.0f){
            transform.position += Vector3.left * speed;
        }

        if(Input.GetAxis("Vertical") > 0.0f){
            transform.position += Vector3.forward * speed;
        }else if(Input.GetAxis("Vertical") < 0.0f){
            transform.position += Vector3.back * speed;
        }


    }
}
