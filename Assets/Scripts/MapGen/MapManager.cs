using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour{

    public static MapManager instance;
    public float offset; //distance between centers
    public int mainBlock = 12; //the index of the block who have the player on

    public string[] bannedSequel1, bannedSequel2;
    int bannedCount1 = 0, bannedCount2 = 0;

    public Block[] map; //the map centered on player
    // - -  -  - -
    // - NW N NE -
    // - W  C  E -
    // - SW S SE -
    // - -  -  - -

    public GameObject[] blockPrefab;

    public Transform car;

    void Awake(){
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    bool Deny (Block b, int newBlock, string prev){
        if (prev == "S"){
            if (b.exitE && map[newBlock + 5] != null && (map[newBlock + 5].exitE ||
                    (map[newBlock + 5].exitS && map[newBlock + 10] != null && map[newBlock + 10].exitE)))
                return true;

            if (b.exitW && map[newBlock + 5] != null && (map[newBlock + 5].exitW ||
                    (map[newBlock + 5].exitS && map[newBlock + 10] != null && map[newBlock + 10].exitW)))
                return true;
        }

      

        return false;
    }

    // Start is called before the first frame update
    void Start(){
        map = new Block[25];
        map[mainBlock] = this.transform.GetChild(0).GetComponent<Block>();
        map[mainBlock].transform.localScale = Vector3.one * offset / map[mainBlock].size;
        Setup();
    }

    public void Setup(){

        Spawn(mainBlock);
    }

    void Spawn(int center){

        if(map[center].exitN){
            SpawnBlock(center, "S", true, 0, 1);
            if (map[center - 5].exitE) SpawnBlock(center, "W", true, 1, 1);
            if (map[center - 5].exitW) SpawnBlock(center, "E", true, -1, 1);
        }

        if(map[center].exitW){
            SpawnBlock(center, "E", map[center].exitW, -1, 0);
            if (map[center - 1].exitN){
                SpawnBlock(center, "S", true, -1, 1);
                if (map[center - 6].exitE){
                    SpawnBlock(center, "W", true, 0, 1);
                    if (map[center - 5].exitE) SpawnBlock(center, "W", true, 1, 1);
                }
            }
        }

        if(map[center].exitE){
            SpawnBlock(center, "W", map[center].exitW, 1, 0);
            if (map[center + 1].exitN){
                SpawnBlock(center, "S", true, 1, 1);
                if (map[center - 4].exitW){
                    SpawnBlock(center, "E", true, 0, 1);
                    if (map[center - 5].exitE) SpawnBlock(center, "E", true, -1, 1);
                }
            }
        }

        SpawnScenario(center);
    }

    void SpawnBlock(int reference, string dir, bool exit, int x, int y){
        Block b;
        List<GameObject> aux = new List<GameObject>();

        if (map[reference + x - 5 * y] == null){
            for (int i = 0; i < blockPrefab.Length; i++){
                b = blockPrefab[i].GetComponent<Block>();
                if (exit == b.VerifyExit(dir) && (dir == "S" || !b.VerifyExit("S")) && !Deny(b, reference + x - 5 * y, dir)){
                    aux.Add(blockPrefab[i]);
                }else if (dir == string.Empty && b.IsScenario()){
                    aux.Add(blockPrefab[i]);
                }

            }
            if (aux.Count == 0) Debug.Log (x + " " + y);

            b = Instantiate(aux[Random.Range(0, aux.Count)], this.transform).GetComponent<Block>();
            b.transform.localScale = Vector3.one * offset / b.size;
            b.transform.position = map[reference].transform.position + (Vector3.right * x + Vector3.forward * y) * offset;
            map[reference + x - 5 * y] = b;
            b.Initialize();
            aux.Clear();
        }
    }

    void SpawnScenario(int reference){
        SpawnBlock(reference, "", true, 0, -1);
        SpawnBlock(reference, "", true, 0, 1);
        SpawnBlock(reference, "", true, 1, -1);
        SpawnBlock(reference, "", true, 1, 1);
        SpawnBlock(reference, "", true, -1, -1);
        SpawnBlock(reference, "", true, -1, 1);
        SpawnBlock(reference, "", true, 1, 0);
        SpawnBlock(reference, "", true, -1, 0);
    }

    // Update is called once per frame
    void Update(){
        Vector3 distance = car.position - map[mainBlock].transform.position;
    
        if(distance.magnitude >= offset / 1.7f){
            float angle = Vector3.Angle(map[mainBlock].transform.forward, distance);

            if (angle < 45.0f){
                ExpandMap(0,1);
            } else if (angle > 135.0f){
                ExpandMap(0,-1);
            } else if(car.position.x < map[mainBlock].transform.position.x){
                ExpandMap(-1,0);
            } else {    
                ExpandMap(1,0);
            }
        }

            //car.position = new Vector3(0.0f, 5.0f, 0.0f);

            if(distance.magnitude >= offset * Mathf.Sqrt(2.0f) / 1.5f){
                RemoveGarbage();
            }
        }
    


    void ExpandMap(int hor, int vert){
        Spawn(mainBlock + hor - 5 * vert);
        map[mainBlock].Close();
        int i, j;
        if (hor == 1 && vert == 0){ //move to E
            for(i = 0; i <= 4; i++){
                for(j = 0; j < 4; j++){
                map[5 * i + j] = map[5 * i + j + 1];
                //Debug.Log(5 * i + j + 1);
                }

                map[5 * i + 4] = null;
            }
        } else if (hor == -1 && vert == 0){ //move to W
            for(i = 0; i <= 4; i++){
                for(j = 4; j > 0; j--){
                map[5 * i + j] = map[5 * i + j - 1];
                //Debug.Log(5 * i + j + 1);
                }

                map[5 * i] = null;
            }
        } else if (hor == 0 && vert == -1){ //move to S
            for(j = 0; j <= 4; j++){
                for(i = 0; i < 4; i++)
                    map[5 * i + j] = map[5 * i + j + 5];

                map[20 + j] = null;
            }
        } else if (hor == 0 && vert == 1){ //move to N
            for(j = 4; j >= 0; j--){
                for(i = 4; i > 0; i--)
                    map[5 * i + j] = map[5 * i + j - 5];

                map[j] = null;
            }
        }

        RemoveGarbage();
    }

    void RemoveGarbage(){
        int i;
        
        for (i = 0; i <=4; i++){    
            if (map[i] != null ) Destroy(map[i].gameObject);
            if (map[20 + i] != null ) Destroy(map[20 + i].gameObject);
        }

        for (i = 1; i <=3; i++){
            if (map[5 * i] != null ) Destroy(map[5 * i].gameObject);
            if (map[5 * i + 4] != null ) Destroy(map[5 * i + 4].gameObject);
        }
    }
}