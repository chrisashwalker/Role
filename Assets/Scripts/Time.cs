using UnityEngine;

public static class Time
{
    public static Light Sunlight {get;set;}  
    public static float MaxSunlightIntensity {get;} = 2.0f;
    public static float DayLength {get;} = 600.0f;
    public static float Sunrise {get;} = DayLength / 4;
    public static float Sunset {get;} = DayLength / 4 * 3;
    public static float SunlightRate {get;} = MaxSunlightIntensity / Sunrise;
    public static float SunlightTime {get;set;}

    public static void GetSunlight()
    {
        Sunlight = GameObject.FindWithTag("Sunlight").GetComponent<Light>();
    }

    public static void Tick()
    {
        Saves.Data.GameTime += Time.deltaTime;
        SunlightTime = Saves.GameData.GameTime - Sunrise;
        if (Saves.GameData.GameTime > Sunrise && Saves.GameData.GameTime < Sunset){
            Sunlight.intensity = 0.2f + MaxSunlightIntensity - System.Math.Abs((Sunrise - SunlightTime) * SunlightRate);
        } else {
            Sunlight.intensity = 0.2f;
        }
        if (Saves.GameData.GameTime >= DayLength){
            Saves.GameData.GameDay += 1;
            foreach (AlteredObject ao in Saves.GameData.AlteredObjects){
                ao.DaysAltered += 1;
            }
            Saves.GameData.GameTime = 0.0f;
        }
    }
}
