using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Gear
{
    public float pitInicial = 0.56f;  
    public float pitFinal = 1.1f;  
    public float minVel, maxVel;
}

public class EngineSound : MonoBehaviour
{

    [SerializeField] private List<Gear> gears = new List<Gear>();

    public CarMain CarMain;

    public AudioSource clip;
    void Start(){
        
    }

    public int gearIndex;

    public float estageEngine = 0;

    private float velKm = 0;
    void FixedUpdate(){
        velKm = (CarMain.speed * 3.6f);
        calcularMarcha();

        estageEngine = R3(gears[gearIndex].minVel, gears[gearIndex].maxVel, velKm);
        clip.pitch = Mathf.Lerp(
            clip.pitch,
            R3Inverso(gears[gearIndex].pitInicial, gears[gearIndex].pitFinal, estageEngine),
            1
        );

    }


    void calcularMarcha(){
        if(velKm >= gears[gearIndex].minVel && velKm < gears[gearIndex].maxVel)
        {
            return;
        }

        Debug.Log("MudarMarcha");
        for (int i = 0; i < gears.Count; i++)
        {
            if (velKm >= gears[i].minVel && velKm < gears[i].maxVel)
            {
                gearIndex = i;
                break;
            }
        }



    }



    private float R3(float min, float max, float valor)
    {
        float porcentagem = ((valor - min) * 100f) / (max - min);
        return porcentagem;
    }
    private float R3Inverso(float min, float max, float porcentagem)
    {
        float valor = min + ((porcentagem / 100f) * (max - min));
        return valor;
    }

}
