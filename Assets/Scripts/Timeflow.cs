using UnityEngine;

public static class Timeflow
{
    public static Light Sunlight {get; set;}  
    public static float MaxSunlightIntensity {get;} = 2.0f;
    public static float DayLength {get;} = 600.0f;
    public static float Sunrise {get;} = DayLength / 4;
    public static float Sunset {get;} = DayLength / 4 * 3;
    public static float SunlightRate {get;} = MaxSunlightIntensity / Sunrise;
    public static float SunlightTime {get; set;}

    public static void GetSunlight()
    {
        Sunlight = GameObject.FindWithTag(Tags.Sunlight).GetComponent<Light>();
    }

    public static void Tick()
    {
        Saves.GameState.GameTime += Time.deltaTime;
        SunlightTime = Saves.GameState.GameTime - Sunrise;
        if (Saves.GameState.GameTime > Sunrise && Saves.GameState.GameTime < Sunset)
        {
            Sunlight.intensity = 0.2f + MaxSunlightIntensity - System.Math.Abs((Sunrise - SunlightTime) * SunlightRate);
        } 
        else 
        {
            Sunlight.intensity = 0.2f;
        }
        if (Saves.GameState.GameTime >= DayLength)
        {
            Saves.GameState.GameDay += 1;
            foreach (AlteredObject ao in Saves.GameState.AlteredObjects)
            {
                ao.DaysAltered += 1;
            }
            Saves.GameState.GameTime = 0.0f;
        }
    }
}
