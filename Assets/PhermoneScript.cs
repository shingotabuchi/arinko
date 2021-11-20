using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhermoneScript : MonoBehaviour
{
    SpriteRenderer PheromoneSprite;
    float PhermoneDissipationTime = SettingsScript.pheromonelife;
    float DissipationRate;
    Color pheromonColor;
    // Start is called before the first frame update
    void Start()
    {
        PheromoneSprite = GetComponent<SpriteRenderer>();
        pheromonColor = PheromoneSprite.color;
        DissipationRate = 1/PhermoneDissipationTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(pheromonColor.a>0)pheromonColor.a -= DissipationRate*Time.deltaTime;
        if(pheromonColor.a<=0)Destroy(gameObject);
        PheromoneSprite.color = pheromonColor;
    }
}
