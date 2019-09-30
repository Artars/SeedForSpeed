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

    public Dictionary<int,TeamLabel> teamCounter;
    public List<SeedManager.CarConfiguration> team;

    public GameObject teamPrefab;
    public Transform teamParent;
    public GameObject startScreenPanel;

    [Range(0,1)]
    public float alphaTeam = 0.8f;

    public void Start()
    {
        playerImages = new Dictionary<SeedPlayer, GameObject>();
        team = new List<SeedManager.CarConfiguration>();
        teamCounter = new Dictionary<int, TeamLabel>();
        startScreenPanel.SetActive(true);
    }

    public void AddPlayer(SeedPlayer player)
    {
        GameObject newImage = GameObject.Instantiate(prefabImage);
        newImage.transform.SetParent(presentationParent);
        Image imageRef = newImage.GetComponentInChildren<Image>();
        if(imageRef == null) imageRef = newImage.GetComponentInChildren<Image>();
        imageRef.sprite = sprites[player.cuckatID];
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
        startScreenPanel.SetActive(false);
        teamParent.gameObject.SetActive(true);

        teamCounter.Clear();

        for (int i = 0; i < controllers.Count; i++)
        {
            GameObject toAssingn = GameObject.Instantiate(teamPrefab);
            //Image image = toAssingn.GetComponent<Image>();
            Color teamColor = team[i].carController.currentColor;
            teamColor.a = alphaTeam;
            
            TeamLabel label = toAssingn.GetComponent<TeamLabel>();
            label.Reset(teamColor);
            //TMPro.TMP_Text text = toAssingn.GetComponentInChildren<TMPro.TMP_Text>();

            teamCounter.Add(i,label);
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
                if(team[i].carController != null)
                {
                    teamCounter[i].label.text = team[i].carController.seedCounter.ToString();
                }
                else
                {
                    //teamCounter[i].transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    public void ResetGame()
    {
        isGamePlaying = false;
        startScreenPanel.SetActive(true);
        teamParent.gameObject.SetActive(false);

        foreach(var element in teamCounter)
        {
            Destroy(element.Value.transform.parent.gameObject);
        }
    }
}
