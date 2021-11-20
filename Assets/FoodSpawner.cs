using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject food;
    public static int phase;
    public static bool spawnFood;
    public float FoodSpawnAreaMinWidthRatio;
    public float FoodSpawnAreaMinHeightRatio;
    float ScreenHalfWidth;
    float ScreenHalfHeight;
    // Start is called before the first frame update
    void Start()
    {
        phase = 0;
        spawnFood = true;
        ScreenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        ScreenHalfHeight = ScreenHalfWidth / Camera.main.aspect;
    }

    // Update is called once per frame
    void Update()
    {
        SpawnFood();
    }

    void SpawnFood(){
        switch(phase){
            case 0:
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPosition = Random.insideUnitCircle.normalized*Random.Range(2f,4f);
                    float phaseZeroFoodSpawnRadius = Random.Range(0.1f,0.5f);
                    DrawFoodCircle(spawnPosition,phaseZeroFoodSpawnRadius);
                }
                spawnFood = false;
                phase++;
                break;
            case 1:
                break;
        }
    }

    void DrawFoodCircle(Vector2 spawnPos,float radius){
        Vector2 currSpawnPos;
        Vector2 prevSpawnPos = new Vector2(-9999,-9999);
        for (float radiusIndex = 0; radiusIndex < radius; radiusIndex+=0.05f)
        {
            for (float angleIndex = 0; angleIndex < 360; angleIndex+=1)
            {
                currSpawnPos = new Vector2(Mathf.Cos(angleIndex*Mathf.PI/180),Mathf.Sin(angleIndex*Mathf.PI/180));
                currSpawnPos = (Vector2)(currSpawnPos*radiusIndex + spawnPos);
                if(Vector2.Distance(prevSpawnPos,currSpawnPos)>0.05f||prevSpawnPos.x==-9999){
                    Instantiate(food,currSpawnPos,Quaternion.identity);
                    prevSpawnPos = currSpawnPos;
                }
            }
        }
    }
}
