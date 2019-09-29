using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour{

    public float size;
    int index;

    public Color[] colors;

    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        
    }

    public void Initialize(){
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colors[Random.Range(0, colors.Length)];
    }
}
