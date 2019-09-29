using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour{

    public static MapManager instance;

    public float offset; //distance between centers
    public int mainBlock = 12; //the index of the block who have the player on

    public Block[] map; //the map centered on player
    // - -  -  - -
    // - NW N NE -
    // - W  C  E -
    // - SW S SE -
    // - -  -  - -

    public GameObject blockPrefab;

    public Transform car;

    void Awake(){
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start(){
        map = new Block[25];
        map[mainBlock] = this.transform.GetChild(0).GetComponent<Block>();
        Setup();
    }

    public void Setup(){
        Spawn(mainBlock);
    }

    void Spawn(int center){
        Block b;
        for (int i = 0; i < 8; i++){
            int x = 0, y = 0; //positions by the center
            //N=(0,1); NE=(1,1); E=(1,0); SE=(1,-1); S=(0,-1); SW=(-1,-1); W(-1,0); NW=(-1,1)
            if (i == 1 || i == 2 || i == 3) x += 1;
            if (i == 5 || i == 6 || i == 7) x -= 1;
            if (i == 7 || i == 1 || i == 0) y += 1;
            if (i == 3 || i == 4 || i == 5) y -= 1;

            if (map[center + x - 5*y] == null) {
                b = Instantiate(blockPrefab, this.transform).GetComponent<Block>();
                b.transform.position = map[center].transform.position + (Vector3.right * x + Vector3.forward * y) * offset;
                map[center + x - 5*y] = b;
                b.Initialize();
            }
        }
    }

    // Update is called once per frame
    void Update(){
        Vector3 distance = car.position - map[mainBlock].transform.position;
    
        if(distance.magnitude >= offset / 1.5f){
            float angle = Vector3.Angle(map[mainBlock].transform.forward, distance);

            if (angle < 45.0f){
                ExpandMap(0,1);
                Debug.Log("N");
            } else if (angle > 135.0f){
                ExpandMap(0,-1);
                 Debug.Log("S");
            } else if(car.position.x < map[mainBlock].transform.position.x){
                ExpandMap(-1,0);
                Debug.Log("W");
            } else {    
                ExpandMap(1,0);
                Debug.Log("E");
            }
        }

            //car.position = new Vector3(0.0f, 5.0f, 0.0f);

            if(distance.magnitude >= offset * Mathf.Sqrt(2.0f) / 1.5f){
                RemoveGarbage();
            }
        }
    


    void ExpandMap(int hor, int vert){
        Spawn(mainBlock + hor - 5 * vert);
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