using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleActivator : MonoBehaviour
{
    public static float seedChance = 0.7f;
    public static int countBlocks = 0;
    public static float chanceDrop = 0.01f;
    public List<GameObject> obstacles = new List<GameObject>();
    public GameObject seedCar;
    public bool isOrigin = false;

    // Start is called before the first frame update
    void Start()
    {
        int obsNumber = (int)Random.Range(0,2);
        int i = 0;
        int aux;
        countBlocks++;
        while(i < obsNumber){
            aux = (int)Random.Range(0,obstacles.Count-1);
            if (!obstacles[aux].activeSelf) obstacles[aux].SetActive(true);
            else{
                while (obstacles[aux].activeSelf){
                    aux = (int)Random.Range(0,obstacles.Count-1);
                    if (!obstacles[aux].activeSelf) obstacles[aux].SetActive(true);
                }
            }
            i++;
        }
        if (!isOrigin && Random.Range(0,1)<seedChance-countBlocks*chanceDrop && seedCar != null) 
            seedCar.SetActive(true);
        else if (isOrigin && seedCar != null)
            seedCar.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
