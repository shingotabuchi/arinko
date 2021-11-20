using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.UI;  
using UnityEngine.SceneManagement;  
using System;

public class SettingsScript : MonoBehaviour
{
    public static float antcount = 15;
    public Text AntCount;
    public static float speed = 2;
    public Text Speed;
    public static float pheromonePerSec = 5;
    public Text PheroPerSec;
    public static float viewradius = 2;
    public Text Viewradius;
    public static float fov = 210;
    public Text Fov;
    public static float bunkainou = 30;
    public Text Bunkainou;
    public static float kyodori = 30;
    public Text Kyodori;
    public static bool mukimuki;
    public static float pheromonelife = 60;
    public Text Pheromonelife;
    public static float gensuirate = 30;
    public Text Gensuirate;
    public static float majimesa = 80;
    public Text Majimesa;
    bool canStart = true;
    public Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ToggleMukiMuki();
    }
    public void LoadScene() {
        if(AntCount.text.Length>0){
            try
            {
                antcount = float.Parse(AntCount.text);
                if(antcount<0)canStart = false;
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }

        if(Speed.text.Length>0){
        try
        {
            speed = float.Parse(Speed.text);
            if(speed<0) canStart = false;
        }
        catch (FormatException)
        {
            canStart = false;
        }}

        if(PheroPerSec.text.Length>0){
        try
        {
            pheromonePerSec = float.Parse(PheroPerSec.text);
            if(pheromonePerSec<0) canStart = false;
        }
        catch (FormatException)
        {
            canStart = false;
        }}

        if(Viewradius.text.Length>0){
        try
        {
            viewradius = float.Parse(Viewradius.text);
            if(viewradius<0) canStart = false;
        }
        catch (FormatException)
        {
            canStart = false;
        }}
        
        if(Fov.text.Length>0){
        try
        {
            fov = float.Parse(Fov.text);
            if(fov>360)fov = 360;
            if(fov<0) canStart = false;
        }
        catch (FormatException)
        {
            canStart = false;
        }}

        if(Bunkainou.text.Length>0){
        try
        {
            bunkainou = float.Parse(Bunkainou.text);
            if(bunkainou<0) canStart = false;
        }
        catch (FormatException)
        {
            canStart = false;
        }}

        if(Kyodori.text.Length>0){
        try
        {
            kyodori = float.Parse(Kyodori.text);
            if(kyodori>100) kyodori = 100;
            if(kyodori<0) canStart = false;
        }
        catch (FormatException)
        {
            canStart = false;
        }}

        if(Pheromonelife.text.Length>0){
        try
        {
            pheromonelife = float.Parse(Pheromonelife.text);
            if(pheromonelife<0) canStart = false;
        }
        catch (FormatException)
        {
            canStart = false;
        }}

        if(Gensuirate.text.Length>0){
        try
        {
            gensuirate = float.Parse(Gensuirate.text);
            if(gensuirate<=0) canStart = false;
        }
        catch (FormatException)
        {
            canStart = false;
        }}

        if(Majimesa.text.Length>0){
        try
        {
            majimesa = float.Parse(Majimesa.text);
            if(majimesa>100) majimesa = 100;
            if(majimesa<0)canStart = false;
        }
        catch (FormatException)
        {
            canStart = false;
        }}
        if(canStart){
            SceneManager.LoadScene("Arinko");
        }
    }
    void ToggleMukiMuki(){
        mukimuki = toggle.isOn;
        print(mukimuki);
    }
}
