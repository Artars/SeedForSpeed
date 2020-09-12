using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SeedManager : MonoBehaviour
{
    public static SeedManager instance = null;
    public List<SeedPlayer> players;
    public bool isGamePlaying = false;
    public GameObject carPrefab;
    public Transform spawnPoint;
    public bool gameOver = false;
    public Color[] colors = new Color[] {Color.red,Color.blue,Color.yellow};
    public string[] colorsName = new string[] {"Red","Blue","Yellow"};
    public GameObject cuckatielPrefab;
    public CameraFollower cameraFollowerPrefab;

    public Transform pivot;

    public UIManager uIManager;

    public AudioSource playerDeath;

    public float maxDistance;
    public float maxTimeOutside;

    protected List<CarConfiguration> cars;
    public Transform initialPosition;
    // public QUICKFOLOWER follower;

    float startTime = 0.0f;

    [System.Serializable]
    public class CarConfiguration
    {
        public int id = -1;
        public Color color;
        public string colorName;
        public List<SeedPlayer> players;
        public PlayerController carController;
        public CameraFollower cameraFollower;
        public float dangerCounter;
        public float distancePct;
        public bool winning = false;
        protected Dictionary<SeedPlayer.PlayerActions,SeedPlayer> assigmentPlayers;
        protected Dictionary<SeedPlayer, Cuckatiel> cuckatiels;


        public void AddPlayer(SeedPlayer player)
        {
            players.Add(player);
            player.carController = carController;
            player.SetColor(player.carController.currentColor);      
            player.carId = id;
        }

        public void AddCuckatiel(Cuckatiel c, SeedPlayer sp)
        {
            cuckatiels.Add(sp,c);
        }

        public void RemovePlayer(SeedPlayer player)
        {
            players.Remove(player);
            player.carController = null;
            player.carId = -1;

            if(player.gameObject != null)
            {
                player.SetColor(Color.grey);
                player.DisplayMessage("Your " + colorName + " car was arrested!");
            }
            if(cuckatiels.ContainsKey(player))
            {
                Destroy(cuckatiels[player]);
                cuckatiels.Remove(player);
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
        // cameraFollower.setTarget(initialPosition.transform);
    }

    public void AddPlayer(SeedPlayer player)
    {
        players.Add(player);
        player.SetColor(Color.grey);
        if(!isGamePlaying)
        {
            if(players.Count == 1)
            {
                player.DisplayMessage("You can't play alone!");
                // player.SetActions(SeedPlayer.PlayerActions.Scream ,SeedPlayer.PlayerActions.Start);
            }
            else if(players.Count == 2)
            {
                players[0].SetActions(SeedPlayer.PlayerActions.Scream ,SeedPlayer.PlayerActions.Start);
                player.SetActions(SeedPlayer.PlayerActions.Scream);            
            }
            else
            {
                player.SetActions(SeedPlayer.PlayerActions.Scream);
            }
        }
        else
        {
            player.DisplayMessage("Wait for match to end!");
        }

        uIManager.AddPlayer(player);
    }

    public void RemovePlayer(SeedPlayer player)
    {
        players.Remove(player);
        if(!isGamePlaying)
        {
            if(players.Count == 1)
            {
                players[0].DisplayMessage("You can't play alone!");
            }
            else if(player.actions[1].action == SeedPlayer.PlayerActions.Start)
            {
                if(players.Count > 1)
                {
                    players[0].SetActions(SeedPlayer.PlayerActions.Scream ,SeedPlayer.PlayerActions.Start);
                }
            }
        }
        else
        {
            int carId = player.carId;
            if(carId != -1)
            {
                cars[carId].RemovePlayer(player);
                if(cars[carId].players.Count < 2)
                {
                    cars[carId].carController.seedCounter = 0;
                }
                else
                {
                    ScrambleCarPlaces(carId);
                }
            }
        }

        uIManager.RemovePlayer(player);
    }

    public void RemoveCar (int id){
        cars[id].RemovePlayers();
        Destroy(cars[id].carController.gameObject);
        cars[id].carController = null;
        uIManager.teamCounter[id].Kill(Time.time - startTime);
        playerDeath.Play();

        bool hasEnded = true;
        for (int i = 0; i < cars.Count; i++)
        {
            if(cars[i].carController != null)
                hasEnded = false;
        }
        if(hasEnded) 
        {
            gameOver = true;
            Invoke("StartGameOver", 2.0f);
            Debug.Log("Game over");
        }
        Debug.Log("Removed " + id);
    }

    public void ScrambleCarPlaces(int id){
        if (id > -1 && id < cars.Count) cars[id].RandomizePositions();
        uIManager.teamCounter[id].ControlChange();
        cars[id].carController.ReleaseFeathers();
    }

    public void ScrambleCarPlacesBump(int id){
        if (id > -1 && id < cars.Count) cars[id].RandomizePositions();
        uIManager.teamCounter[id].ControlChange();
    }

    public void StartGame()
    {
        if(isGamePlaying) return;
        startTime = Time.time;
        int numCars = 1;
        if (players.Count > 1){
            numCars = Mathf.FloorToInt(players.Count/2.0f);
            if (numCars > 4) numCars = 4;
        }

        cars.Clear();
        
        //Instantiate car
        for(int i = 0; i < numCars; i++)
        {
            InstantiateCar(i);
        }

        InstantiateCameras(numCars);

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
        GameObject newCar = GameObject.Instantiate(carPrefab, spawnPoint.position + spawnPoint.right * index * 5, spawnPoint.rotation);
        PlayerController playerController = newCar.GetComponent<PlayerController>();
        playerController.id = index;
        playerController.SetCarColor(colors[index]);

        CarConfiguration newConfiguration = new CarConfiguration();
        newConfiguration.carController = playerController;
        newConfiguration.id = index;
        newConfiguration.color = colors[index];
        newConfiguration.colorName = colorsName[index];
        newConfiguration.dangerCounter = maxTimeOutside;

        cars.Add(newConfiguration);
    }

    protected void InstantiateCameras(int carsCount)
    {
        float widtDelta = (carsCount > 1) ? 0.5f : 1f;
        int verticalCount = Mathf.CeilToInt(carsCount / 2f);
        float heightDelta = 1.0f / verticalCount;

        for (int i = 0; i < carsCount; i++)
        {
            CameraFollower currentCameraFollower = GameObject.Instantiate(cameraFollowerPrefab).GetComponent<CameraFollower>();
            Camera currentCamera = currentCameraFollower.GetComponentInChildren<Camera>();
            int horizontalIndex = (i % 2);
            int verticalIndex = Mathf.FloorToInt( (carsCount-1-i) / 2f);

            //Rect is defined by 4 points(corners) or by starting points and rect. The second one will be easier
            float startingX = horizontalIndex * widtDelta;
            float startingY = verticalIndex * heightDelta;
            currentCamera.rect = new Rect(startingX ,startingY ,widtDelta, heightDelta);

            currentCameraFollower.setTarget(cars[i].carController.transform);
            cars[i].cameraFollower = currentCameraFollower;
        }
        
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
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (isGamePlaying){
                SceneManager.LoadScene(0);
            }else{
                Application.Quit();
            }
        }

        if(isGamePlaying && !gameOver)
        {
            int winningPlayer = -1;
            int winningAmount = 0;
            for (int i = 0; i < cars.Count; i++)
            {
                if(cars[i].carController != null && cars[i].carController.seedCounter >= winningAmount)
                {
                    if (winningPlayer == -1 || cars[i].carController.seedCounter != winningAmount ||
                        cars[i].carController.transform.position.z > cars[winningPlayer].carController.transform.position.z){
                        winningPlayer = i;
                        winningAmount = cars[i].carController.seedCounter;
                    }
                }
            }
            if(winningPlayer != -1)
            {
                // cameraFollower.setTarget(cars[winningPlayer].carController.transform);
                // follower.target = cars[winningPlayer].carController.transform;
                pivot.position = cars[winningPlayer].carController.transform.position;

                Vector3 winningPosition = cars[winningPlayer].carController.transform.position;

                for (int i = 0; i < cars.Count; i++)
                {
                    if(cars[i].carController == null) continue;
                    //Update winning
                    cars[i].winning = (i == winningPlayer);

                    //Update distance and lose conditions
                    float distance = (cars[i].carController.transform.position - winningPosition).magnitude;
                    cars[i].distancePct = distance / maxDistance;
                    if(cars[i].distancePct >= 1.0f)
                    {
                        cars[i].dangerCounter -= Time.deltaTime;
                    }
                    else
                    {
                        cars[i].dangerCounter += Time.deltaTime;
                    }
                    cars[i].dangerCounter = Mathf.Clamp(cars[i].dangerCounter,0, maxTimeOutside);
                    if(cars[i].dangerCounter <= 0)
                        RemoveCar(i);
                }

            }
        }
    }

    public void StartGameOver()
    {
        //cameraFollower.setTarget(initialPosition);
        //pivot.position = initialPosition.position;

        //isGamePlaying = false;

        //players[0].SetActions(SeedPlayer.PlayerActions.Scream,SeedPlayer.PlayerActions.Start);
        
        //uIManager.ResetGame();

        SceneManager.LoadScene(0);
    }
}
