using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  

public class FoodMakerScript : MonoBehaviour
{
    public GameObject food;
    public GameObject antman;
    public GameObject Canvas;
    bool isMakingFood = false;
    // Start is called before the first frame update
    void Start()
    {
        int numberofantmen = (int)SettingsScript.antcount;
        for (int i = 0; i < numberofantmen; i++)
        {
            Instantiate(antman, new Vector3(0,0,0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Canvas.SetActive(false);
            isMakingFood = true;
            Vector3 screenPoint = Input.mousePosition;
            screenPoint.z = 10.0f;
            Instantiate(food, Camera.main.ScreenToWorldPoint(screenPoint), Quaternion.identity);
        }
        if(isMakingFood){
            Vector3 screenPoint = Input.mousePosition;
            screenPoint.z = 10.0f;
            Instantiate(food, Camera.main.ScreenToWorldPoint(screenPoint), Quaternion.identity);
        }
        if(Input.GetMouseButtonUp(0)){
            isMakingFood = false;
        }
    }

    public void Modoru(){
        SceneManager.LoadScene("MenuScene");
    }
}
