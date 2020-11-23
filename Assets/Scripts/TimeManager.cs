using UnityEngine;

public static class TimeManager{
    public static int gameDay;
    public static float gameTime;
    public static float dayLength;
    public static float maxLightIntensity;
    public static float sunlightTime;
    public static float sunlightRate;
    public static float sunrise;
    public static float sunset;
    public static Light Sunlight;

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
            gameTime = 0.0f;
            Debug.Log("It's Day " + gameDay);
        }
    }
}
