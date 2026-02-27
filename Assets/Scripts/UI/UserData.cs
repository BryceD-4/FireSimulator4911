using UnityEngine;

public class UserData
{
    //Stores raw user input, not probabilities
    //Wind, temp, humidity
    //slope and fuel load comes from the cell itself
    public int windDirection;
    public float windSpeed;
    public float maxWindSpeed;
    public float temperature;
    public float maxTemperature;
    public float humidity;
}
