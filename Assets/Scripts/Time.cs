using UnityEngine;

public static class Time{
    public static float dayLength{get;} = 600.0f;
    public static float maxLightIntensity{get;} = 2.0f;
    public static float sunlightTime{get;set;}
    public static float sunrise{get;} = dayLength / 4;
    public static float sunset{get;} = dayLength / 4 * 3;
    public static float sunlightRate{get;} = maxLightIntensity / sunrise;
    public static Light Sunlight{get;set;}

    public static void ClockTick(){
        Saves.GameData.GameTime += Time.deltaTime;
        sunlightTime = Saves.GameData.GameTime - sunrise;
        if (Saves.GameData.GameTime > sunrise && Saves.GameData.GameTime < sunset){
            Sunlight.intensity = 0.2f + maxLightIntensity - System.Math.Abs((sunrise - sunlightTime) * sunlightRate);
        } else {
            Sunlight.intensity = 0.2f;
        }
        if (Saves.GameData.GameTime >= dayLength){
            Saves.GameData.GameDay += 1;
            foreach (AlteredObject ao in Saves.GameData.AlteredObjects){
                ao.DaysAltered += 1;
            }
            Saves.GameData.GameTime = 0.0f;
        }
    }
}
