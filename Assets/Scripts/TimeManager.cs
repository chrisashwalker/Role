using UnityEngine;

public static class TimeManager{
    public static int gameDay;
    public static float gameTime;
    public static float dayLength = 600.0f;
    public static float maxLightIntensity = 4.0f;
    public static float sunlightTime;
    public static float sunrise = dayLength / 4;
    public static float sunset = dayLength / 4 * 3;
    public static float sunlightRate = maxLightIntensity / sunrise;
    public static Light Sunlight = GameObject.FindWithTag("Sunlight").GetComponent<Light>();

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
