using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamLabel : MonoBehaviour
{
    public Image icon;
    public TMPro.TMP_Text label, msg;

    public Image bg;
    
    public Slider slider;
    public Image sliderFillerImg;
    public GameObject warning;
    public TMPro.TMP_Text warningCounter;

    public Color normalSliderColor = Color.white;
    public Color dangerSliderColor = Color.red;
    public Sprite seed, death, goldenSeed;

    public Image fadeImg;
    public float fadeTime = 1;
    public float endFadeAlpha = 0.6f;

    protected bool isDead = false;
    protected bool isWinning = false;

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

    public void SetWinning(bool winning)
    {
        if(!isDead)
        {
            if(isWinning != winning)
            {
                icon.sprite = (winning) ? goldenSeed : seed;
                isWinning = winning;
            }
        }
    }

    public void Kill(float time){
        EraseMsg();
        icon.color = bg.color;
        icon.sprite = death;
        label.text = Mathf.Round(time).ToString("F0") + "s";
        bg.color = Color.black;
        isDead = true;
        StartCoroutine(backToBlack());
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

    public void UpdateDanger(float distancePct, float dangerCounter) 
    {
        slider.value = distancePct;
        if(distancePct >= 1f)
        {
            warning.SetActive(true);
            warningCounter.text = Mathf.Round(dangerCounter).ToString("F0") + "s";
            sliderFillerImg.color = dangerSliderColor;
        }
        else
        {
            warning.SetActive(false);
            sliderFillerImg.color = normalSliderColor;
        }
    }

    void EraseMsg(){
        icon.gameObject.SetActive(true);
        label.gameObject.SetActive(true);
        msg.gameObject.SetActive(false);
    }

    private IEnumerator backToBlack()
    {
        float counter = 0;
        float t = 0;
        Color currentColor = fadeImg.color;

        while(counter < fadeTime)
        {
            currentColor.a = Mathf.Lerp(0, endFadeAlpha, t);
            fadeImg.color = currentColor;
            counter += Time.deltaTime;
            t = counter / fadeTime;
            yield return null;
        }

        currentColor.a = 1;
        fadeImg.color = currentColor;
    }


}
