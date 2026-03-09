/**
Forest Fire Simulator - Unity
Bryce Dixon T00054766 Comp 4911 Capstone March 2026

This file simply holds the data entered by the user for use in the simulator. 

*/

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
