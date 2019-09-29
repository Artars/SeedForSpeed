using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleActivator : MonoBehaviour
{
    public List<GameObject> obstacles = new List<GameObject>();
    public GameObject seedCar;
    public float seedChance = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        int obsNumber = (int)Random.Range(0,2);
        int i = 0;
        int aux;
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
        if (Random.Range(0,1)<seedChance) seedCar.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
