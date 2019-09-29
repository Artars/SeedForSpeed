using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Sprite[] sprites;
    public GameObject prefabImage;
    public Transform presentationParent;
    protected Dictionary<SeedPlayer,GameObject> playerImages;
    public bool isGamePlaying;

    public Dictionary<int,TMPro.TMP_Text> teamCounter;
    public List<SeedManager.CarConfiguration> team;

    public GameObject teamPrefab;
    public Transform teamParent;

    public void Start()
    {
        playerImages = new Dictionary<SeedPlayer, GameObject>();
        team = new List<SeedManager.CarConfiguration>();
        teamCounter = new Dictionary<int, TMPro.TMP_Text>();
    }

    public void AddPlayer(SeedPlayer player)
    {
        GameObject newImage = GameObject.Instantiate(prefabImage);
        newImage.transform.parent = (presentationParent);
        newImage.GetComponent<Image>().sprite = sprites[player.cuckatID];
        newImage.SetActive(true);

        playerImages.Add(player, newImage);
    }

    public void RemovePlayer(SeedPlayer player)
    {
        if(playerImages.ContainsKey(player))
        {
            GameObject toDestroy = playerImages[player];

            Destroy(toDestroy);

            playerImages.Remove(player);
        }
    }

    public void StartGame(List<SeedManager.CarConfiguration> controllers)
    {
        team = controllers;
        presentationParent.gameObject.SetActive(false);
        teamParent.gameObject.SetActive(true);

        for (int i = 0; i < controllers.Count; i++)
        {
            GameObject toAssingn = GameObject.Instantiate(teamPrefab);
            Image image = toAssingn.GetComponentInChildren<Image>();
            image.color = team[i].carController.currentColor;
            
            TMPro.TMP_Text text = toAssingn.GetComponentInChildren<TMPro.TMP_Text>();

            teamCounter.Add(i,text);
            toAssingn.SetActive(true);
            toAssingn.transform.SetParent(teamParent);
        }
        isGamePlaying = true;
    }

    void Update()
    {
        if(isGamePlaying)
        {
            for (int i = 0; i < team.Count; i++)
            {
                teamCounter[i].text = team[i].carController.seedCounter.ToString();
            }
        }
    }

    public void ResetGame()
    {
        isGamePlaying = false;
        presentationParent.gameObject.SetActive(true);
        teamParent.gameObject.SetActive(false);

        foreach(var element in teamCounter)
        {
            Destroy(element.Value.transform.parent.gameObject);
        }
    }
}
