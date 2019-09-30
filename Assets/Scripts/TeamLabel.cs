using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamLabel : MonoBehaviour
{
    
    public Image icon;
    public TMPro.TMP_Text label;

    public Image bg;
    
    public Sprite seed, death;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset(Color c){
        icon.sprite = seed;
        label.text = "0";
        bg.color = c;
    }

    public void Kill(float time){
        icon.sprite = death;
        label.text = Mathf.Round(time).ToString("F2") + "s";
        bg.color = Color.black;
    }


}
