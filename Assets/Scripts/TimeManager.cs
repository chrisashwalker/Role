using UnityEngine;

public static class TimeManager{
    public static int gameDay{get;set;}
    public static float gameTime{get;set;}
    public static float dayLength{get;} = 600.0f;
    public static float maxLightIntensity{get;} = 4.0f;
    public static float sunlightTime{get;set;}
    public static float sunrise{get;} = dayLength / 4;
    public static float sunset{get;} = dayLength / 4 * 3;
    public static float sunlightRate{get;} = maxLightIntensity / sunrise;
    public static Light Sunlight{get;set;}

    public static void ClockTick(){
        gameTime += Time.deltaTime;
        sunlightTime = gameTime - sunrise;
        if (gameTime > sunrise && gameTime < sunset){
            Sunlight.intensity = 0.2f + maxLightIntensity - System.Math.Abs((sunrise - sunlightTime) * sunlightRate);
        } else {
            Sunlight.intensity = 0.2f;
        }
        if (gameTime >= dayLength){
            gameDay += 1;
            foreach (AlteredObject ao in Saves.GameData.AlteredObjects){
                ao.DaysAltered += 1;
            }
            gameTime = 0.0f;
            Debug.Log("It's Day " + gameDay);
        }
    }
}
