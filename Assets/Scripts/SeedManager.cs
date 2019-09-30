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
    public bool gameOver = false;
    public Color[] colors = new Color[] {Color.red,Color.blue,Color.yellow};
    public GameObject cuckatielPrefab;
    public CameraFollower cameraFollower;

    public Transform pivot;

    public UIManager uIManager;

    protected List<CarConfiguration> cars;
    public Transform initialPosition;
    // public QUICKFOLOWER follower;


    [System.Serializable]
    public class CarConfiguration
    {
        public List<SeedPlayer> players;
        public PlayerController carController;
        protected Dictionary<SeedPlayer.PlayerActions,SeedPlayer> assigmentPlayers;
        protected Dictionary<SeedPlayer, Cuckatiel> cuckatiels;


        public void AddPlayer(SeedPlayer player)
        {
            players.Add(player);
            player.carController = carController;
            player.SetColor(player.carController.currentColor);            
        }

        public void AddCuckatiel(Cuckatiel c, SeedPlayer sp)
        {
            cuckatiels.Add(sp,c);
        }

        public void RemovePlayer(SeedPlayer player)
        {
            players.Remove(player);
            player.carController = null;

            if(player.gameObject != null)
            {
                player.SetActions(SeedPlayer.PlayerActions.Scream);
            }
        }

        public void RemovePlayers(){
            while (players.Count > 0){
                RemovePlayer(players[0]);
            }
            assigmentPlayers.Clear();
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
            cuckatiels = new Dictionary<SeedPlayer, Cuckatiel>();
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

            //Set cuckatiel positions
            List<int> cuckatielPos = new List<int>(){0,1,2,3};
            for(int i = 0; i < numPlayers; i++)
            {
                int getIndex = Random.Range(0,cuckatielPos.Count);
                int useIndex = cuckatielPos[getIndex];
                cuckatielPos.RemoveAt(getIndex);

                Cuckatiel c = cuckatiels[players[i]];
                Transform transformToAssing = carController.cuckatielPositions[useIndex];
                c.transform.position = transformToAssing.position;
                c.transform.rotation = transformToAssing.rotation;
                c.transform.SetParent(transformToAssing);
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
        cameraFollower.setTarget(initialPosition);
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

        uIManager.AddPlayer(player);
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

        uIManager.RemovePlayer(player);
    }

    public void RemoveCar (int id){
        cars[id].RemovePlayers();
        Destroy(cars[id].carController.gameObject);
        cars[id].carController = null;

        bool hasEnded = true;
        for (int i = 0; i < cars.Count; i++)
        {
            if(cars[i].carController != null)
                hasEnded = false;
        }
        if(hasEnded) 
        {
            gameOver = true;
            StartGameOver();
            Debug.Log("Game over");
        }
        Debug.Log("Removed " + id);
    }

    public void ScrambleCarPlaces(int id){
        if (id > -1 && id < cars.Count) cars[id].RandomizePositions();
    }

    public void StartGame()
    {
        if(isGamePlaying) return;

        int numCars = Mathf.CeilToInt(players.Count/4.0f);

        cars.Clear();
        
        //Instantiate car
        for(int i = 0; i < numCars; i++)
        {
            InstantiateCar(i);
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

                //Instatiate cuckatiels
                GameObject cuckatielObj = GameObject.Instantiate(cuckatielPrefab);
                Cuckatiel cuckatiel = cuckatielObj.GetComponent<Cuckatiel>();
                cuckatiel.SetCuckatiel(players[item]);
                cars[i].AddCuckatiel(cuckatiel,players[item]);

            }
            cars[i].RandomizePositions();
        }



        isGamePlaying = true;

        uIManager.StartGame(cars);
    }

    protected void InstantiateCar(int index)
    {
        GameObject newCar = GameObject.Instantiate(carPrefab, spawnPoint.position + spawnPoint.right * index * 2, spawnPoint.rotation);
        PlayerController playerController = newCar.GetComponent<PlayerController>();
        playerController.id = index;
        playerController.SetCarColor(colors[index]);

        CarConfiguration newConfiguration = new CarConfiguration();
        newConfiguration.carController = playerController;
        

        cars.Add(newConfiguration);
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

    public void debugStartGame(int numCars = 1){
        for(int i = 0; i < numCars; i++)
        {
            InstantiateCar(i);
        }
        cars[0].carController.gamepadInput = true;
    }

    public void Update()
    {
        if(isGamePlaying && !gameOver)
        {
            int winningPlayer = -1;
            int winningAmount = 0;
            for (int i = 0; i < cars.Count; i++)
            {
                if(cars[i].carController != null && cars[i].carController.seedCounter > winningAmount)
                {
                    winningPlayer = i;
                    winningAmount = cars[i].carController.seedCounter;
                }
            }
            cameraFollower.setTarget(cars[winningPlayer].carController.transform);
            if(winningPlayer != -1)
            {
            // follower.target = cars[winningPlayer].carController.transform;
            pivot = cars[winningPlayer].carController.transform;

            }
        }
    }

    public void StartGameOver()
    {
        cameraFollower.setTarget(initialPosition);

        isGamePlaying = false;

        players[0].SetActions(SeedPlayer.PlayerActions.Scream,SeedPlayer.PlayerActions.Start);
        
        uIManager.ResetGame();
    }
}
