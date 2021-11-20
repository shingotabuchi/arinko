using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenAnt : MonoBehaviour
{
    public GameObject AntMan;
    public GameObject AntParent;
    public static int FoodCount;
    public static bool spawnAnt;
    // Start is called before the first frame update
    void Start()
    {
        FoodCount = 0;
        spawnAnt = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnAnt){
            AntScript.isInit = false;
            GameObject newAntMan;
            newAntMan = Instantiate(AntMan, transform.position, Quaternion.identity);
            newAntMan.transform.parent = AntParent.transform;
            spawnAnt = false;
        }
    }
}
