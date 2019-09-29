using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Block : MonoBehaviour{

    public float size;
    public bool exitN, exitS, exitE, exitW;
    
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

    public bool VerifyExit(string dir){
        switch(dir){
            case "N":
                return exitN;
            
            case "S":
                return exitS;

            case "E":
                return exitE;

            case "W":
                return exitW;

            default:
                return false;
        }
    }

    public bool IsScenario(){
        return !exitN && !exitS && !exitE && !exitW;
    }

    public void Close(){
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.black;
        exitE = false;
        exitW = false;
        exitN = false;
        exitS = false;
    }
}
