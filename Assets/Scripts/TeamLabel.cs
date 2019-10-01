using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamLabel : MonoBehaviour
{
    
    public Image icon;
    public TMPro.TMP_Text label, msg;

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
        icon.color = Color.white;
        icon.sprite = seed;
        label.text = "0";
        bg.color = c;
    }

    public void Kill(float time){
        EraseMsg();
        icon.color = bg.color;
        icon.sprite = death;
        label.text = Mathf.Round(time).ToString("F0") + "s";
        bg.color = Color.black;
    }

    public void ControlChange(){
        if (bg.color != Color.black){
            CancelInvoke();
            icon.gameObject.SetActive(false);
            label.gameObject.SetActive(false);
            msg.gameObject.SetActive(true);
            Invoke("EraseMsg", 2.0f);
        }
    }

    void EraseMsg(){
        icon.gameObject.SetActive(true);
        label.gameObject.SetActive(true);
        msg.gameObject.SetActive(false);
    }


}
