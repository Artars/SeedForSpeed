using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public static SeedManager instance = null;
    public List<SeedPlayer> players;
    public bool isGamePlaying = false;
    public GameObject carPrefab;
    public Transform spawnPoint;

    protected List<CarConfiguration> cars;

    [System.Serializable]
    public class CarConfiguration
    {
        public List<SeedPlayer> players;
        public PlayerController carController;
        protected Dictionary<SeedPlayer.PlayerActions,SeedPlayer> assigmentPlayers;

        public void AddPlayer(SeedPlayer player)
        {
            players.Add(player);
            player.carController = carController;
        }

        public void RemovePlayer(SeedPlayer player)
        {
            players.Remove(player);
            player.carController = null;
        }

        public CarConfiguration()
        {
            //Initialize dictionary
            assigmentPlayers = new Dictionary<SeedPlayer.PlayerActions, SeedPlayer>();
            assigmentPlayers.Add(SeedPlayer.PlayerActions.Left,null);
            assigmentPlayers.Add(SeedPlayer.PlayerActions.Right, null);
            assigmentPlayers.Add(SeedPlayer.PlayerActions.Accelerator, null);
            assigmentPlayers.Add(SeedPlayer.PlayerActions.Brake, null);

            players = new List<SeedPlayer>();
        }

        public int Count
        {
            get{return players.Count;}
        }

        public void RandomizePositions()
        {
            int numPlayers = Count;
            SeedPlayer.PlayerActions[,] selected = new SeedPlayer.PlayerActions[numPlayers,2];
            List<SeedPlayer.PlayerActions> rolesRemaining = new List<SeedPlayer.PlayerActions>(){
                SeedPlayer.PlayerActions.Left,
                SeedPlayer.PlayerActions.Right, 
                SeedPlayer.PlayerActions.Accelerator,
                SeedPlayer.PlayerActions.Brake};
            
            // while(rolesRemaining.Count < numPlayers * 2)
            // {
            //     rolesRemaining.Add(SeedPlayer.PlayerActions.Scream);
            // }

            //Select first role
            for(int i = 0; i < numPlayers; i++)
            {
                int index1;
                index1 = Random.Range(0, rolesRemaining.Count);

                SeedPlayer.PlayerActions role1 = rolesRemaining[index1];
                rolesRemaining.RemoveAt(index1);

                selected[i,0] = role1;
                // players[i].SetActions(role1, role2);
                // assigmentPlayers[role1] = players[i];
            }

            // Add scream roles
            while(rolesRemaining.Count < numPlayers)
            {
                rolesRemaining.Add(SeedPlayer.PlayerActions.Scream);
            }

            // Select second role
            for(int i = 0; i < numPlayers; i++)
            {
                int index1;
                index1 = Random.Range(0, rolesRemaining.Count);

                SeedPlayer.PlayerActions role1 = rolesRemaining[index1];
                rolesRemaining.RemoveAt(index1);

                selected[i,1] = role1;
                // players[i].SetActions(role1, role2);
                // assigmentPlayers[role1] = players[i];
            }

            // Set roles
            for(int i = 0; i < numPlayers; i++)
            {
                players[i].SetActions(selected[i,0], selected[i,1]);
                assigmentPlayers[selected[i,0]] = players[i];
                if(selected[i,1] != SeedPlayer.PlayerActions.Scream)
                {
                    assigmentPlayers[selected[i,1]] = players[i];
                }
            }
            
        }
    }

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        players = new List<SeedPlayer>();
        cars = new List<CarConfiguration>();
    }

    public void AddPlayer(SeedPlayer player)
    {
        players.Add(player);
        if(players.Count == 1)
        {
            player.SetActions(SeedPlayer.PlayerActions.Scream ,SeedPlayer.PlayerActions.Start);
        }
        else
        {
            player.SetActions(SeedPlayer.PlayerActions.Scream);
        }
    }

    public void RemovePlayer(SeedPlayer player)
    {
        players.Remove(player);
        if(!isGamePlaying)
        {
            if(player.actions[1].action == SeedPlayer.PlayerActions.Start)
            {
                if(players.Count > 0)
                {
                    players[0].SetActions(SeedPlayer.PlayerActions.Scream ,SeedPlayer.PlayerActions.Start);
                }
            }
        }
    }

    public void StartGame()
    {
        if(isGamePlaying) return;

        int numCars = Mathf.CeilToInt(players.Count/4.0f);

        cars.Clear();
        
        //Instantiate car
        for(int i = 0; i < numCars; i++)
        {
            GameObject newCar = GameObject.Instantiate(carPrefab, spawnPoint.position + spawnPoint.right * i * 2, spawnPoint.rotation);
            PlayerController playerController = newCar.GetComponent<PlayerController>();

            CarConfiguration newConfiguration = new CarConfiguration();
            newConfiguration.carController = playerController;

            cars.Add(newConfiguration);
        }

        List<int>[] assigment = new List<int>[numCars];
        List<int> playersRemaining = new List<int>();

        for (int i = 0; i < numCars; i++)
            assigment[i] = new List<int>();            

        for(int i = 0; i < players.Count; i++)
            playersRemaining.Add(i);

        playersRemaining = RandomizeArray(playersRemaining);

        isGamePlaying = true;
        int currentCar = 0;
        while(playersRemaining.Count > 0)
        {
            assigment[currentCar].Add(playersRemaining[0]);
            playersRemaining.RemoveAt(0);
            currentCar++;
            if(currentCar >= numCars)
                currentCar = 0;
        }

        for (int i = 0; i < numCars; i++)
        {
            foreach (var item in assigment[i])
            {
                cars[i].AddPlayer(players[item]);
            }
            cars[i].RandomizePositions();
        }

        isGamePlaying = true;
    }

    protected List<int> RandomizeArray(List<int> vector)
    {
        List<int> randomized = new List<int>();
        while(vector.Count > 0)
        {
            int index = Random.Range(0,vector.Count);
            int value = vector[index];
            vector.RemoveAt(index);

            randomized.Add(value);
        }

        return randomized;
    }


}
