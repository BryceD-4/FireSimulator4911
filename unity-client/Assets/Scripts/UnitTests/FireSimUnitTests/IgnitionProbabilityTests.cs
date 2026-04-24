/**
Forest Fire Simulator - Unity
Bryce Dixon T00054766 Comp 4911 Capstone March 2026

This program is the unit tests for testing the "IgnitionProbabilityGenerator" class
to ensure the probabilities are being calculated correctly. 

References used:
1. using test runner:https://docs.unity3d.com/6000.0/Documentation/Manual/testing-editortestsrunner.html
2. Structuring unit tests in unity: https://docs.unity.com/en-us/cloud-code/modules/how-to-guides/unit-testing-create

*/
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class IgnitionProbabilityTests
{
    //These are known variables created for the unit tests below. 
    //Layout of the neighbours
    //0,0 | 0,1 
    //1,0 | 1,1 --> north is this way -->    
    GridCell neighbour00 = new GridCell(0,0);
    GridCell neighbour01 = new GridCell(0,1);
    GridCell main10 = new GridCell(1,0);
    GridCell neighbour11 = new GridCell(1,1);


    //1.5 as 2D grid is 100x100 and unityterrain is 150x150
    static float cellSize = 1.5f;
    static float temperature = 10f;
    static float maxTemperature = 50f;
    //0 = north
    static int windDirection = 0;
    static float windSpeed = 20f;
    static float maxWindSpeed = 60f;
    static float humidity = 50f;

    //Used to access the functions within the probability generator
    IgnitionProbabilityGenerator probabilityGenerator = new IgnitionProbabilityGenerator(cellSize);
    UserData userData = new UserData();
    //This runs once before each test is run
    [SetUp]
    public void InitialSetUp()
    {
        //To get a 20 degree angle for slope change, calculate the rise value for 20 degrees
        neighbour00.elevation = 1+(Mathf.Tan(20f * Mathf.Deg2Rad) * cellSize);
        main10.elevation = 1;
        //To get a 45 degree angle change, make the rise and run equal
        //the run to a diagonal cell will be cellSize*sqrt(2)
        neighbour01.elevation = 1+(cellSize*Mathf.Sqrt(2)); 

        //Populate the user data object with known values
        userData.windDirection = windDirection;
        userData.windSpeed = windSpeed;
        userData.maxWindSpeed = maxWindSpeed;
        userData.temperature = temperature;
        userData.maxTemperature = maxTemperature;
        userData.humidity = humidity;

        //Pass this data to the probability ggenerator so we know the user input for
        //the tests below. 
        probabilityGenerator.SetUserInput(userData);
    }
    
    // A Test behaves as an ordinary method
    [Test]
    public void SimpleTestCheck()
    {
        // Use the Assert class to test conditions
        Assert.AreEqual(2, 1 + 1);
    }

    [Test]
    public void TestCalculateFuelFactor()
    {
        //get the position of the cell of interest
        Vector3 worldPosition = new Vector3(
          neighbour00.GetCellX()*cellSize+cellSize/2,
          0,
          neighbour00.GetCellZ()*cellSize+cellSize/2
        );
        //Set the fuel cell value 
        neighbour00.SetCellFuelValue(worldPosition, cellSize);
        //Get the fuel factor from the calculation
        float fuelFactor = probabilityGenerator.CalculateFuelFactor(neighbour00);
        //This value will be 0.025 due to how FuelFactor is normalized, so /2
        float regularFuel = GridCell.fuelLibrary.regularFuel/2;

        Assert.AreEqual(regularFuel, fuelFactor);
    }
    
    [Test]
    public void TestCalculateTemperatureFactor()
    {
        //When set user input is called, it calculates the temperature factor
        float calculatedTemperatureFactor = probabilityGenerator.temperatureFactor;
        float temperatureValue = temperature/maxTemperature;
        //It should be equal to the exponentially squared value. 
        Assert.AreEqual((temperatureValue*temperatureValue), calculatedTemperatureFactor);
    }

    [Test]
    public void TestCalculateHumidityFactor()
    {
        //When set user input is called, it calculates the humidity factor
        float calculatedHumidityFactor = probabilityGenerator.humidityFactor;
        float humidityValue = humidity/100f;
        //Thsi should be equal to the exponentially squared cvalue. 
        Assert.AreEqual((humidityValue*humidityValue), calculatedHumidityFactor);
    }

    [Test]
    public void TestCalculateWindFactorDirectlyDownwind()
    {
        //Wind will be heading north 
        //Get the neighbour and main cell information for entry into the method
        int mainX = main10.GetCellX();
        int mainZ = main10.GetCellZ();
        int neighbourX = neighbour11.GetCellX();
        int neighbourZ = neighbour11.GetCellZ();
        float windFactor = probabilityGenerator.CalculateWindFactor(mainX, mainZ, neighbourX, neighbourZ);

        float windSpeedRatio = windSpeed/maxWindSpeed;
        //It should be a maximal application of exponential wind speed
        float expectedWindValue = 1f * windSpeedRatio*windSpeedRatio;
        //items which are directly downwind will have a direct exponential multiple
        Assert.AreEqual(expectedWindValue, windFactor);
    }

    [Test]
    public void TestCalculateWindFactorPartiallyDownwind()
    {
        //Wind will be heading north 
        //Get the cell information for the method below
        int mainX = main10.GetCellX();
        int mainZ = main10.GetCellZ();
        int neighbourX = neighbour01.GetCellX();
        int neighbourZ = neighbour01.GetCellZ();
        float windFactor = probabilityGenerator.CalculateWindFactor(mainX, mainZ, neighbourX, neighbourZ);

        float windSpeedRatio = windSpeed/maxWindSpeed;
        //Since it is a diagonal dot product, we need to normalize, meaning 1/sqrt2 for the indice
        float expectedWindValue = (1/Mathf.Sqrt(2)) * windSpeedRatio*windSpeedRatio;
       //Cells which are diagonally downwind have a wind factor of a multiple of 0.71 (1/sqrt(2))
       //due to the relationship of the dot products
        Assert.AreEqual(expectedWindValue, windFactor);
    }

    [Test]
    public void TestCalculateWindFactorUpwind()
    {
        //Wind will be heading north 
        int mainX = main10.GetCellX();
        int mainZ = main10.GetCellZ();
        int neighbourX = neighbour00.GetCellX();
        int neighbourZ = neighbour00.GetCellZ();
        float windFactor = probabilityGenerator.CalculateWindFactor(mainX, mainZ, neighbourX, neighbourZ);

        //Any cell upwind should not have a wind factor
        float expectedWindValue = 0f;

        //Any cells that are upwind should have a windfactor of 0
        Assert.AreEqual(expectedWindValue, windFactor);
    }

    [Test]
    public void TestDiagonalCalculateSlopeFactor()
    {
        //Get the probability factor
        float slopeFactor = probabilityGenerator.CalculateSlopeFactor(main10, neighbour01);
        //With these items, the slope factor should be 1 (maximal)
        float expectedSlopeFactor = 1.0f;

        Assert.AreEqual(expectedSlopeFactor, slopeFactor);
    }

    [Test]
    public void TestAdjacentCalculateSlopeFactor()
    {
        float slopeFactor = probabilityGenerator.CalculateSlopeFactor(main10, neighbour00);

        //0.25 received as expected angle is 20 degrees
        //20/40 = 0.5, then 0.5*0.5 (exponential) = 0.25
        float elevationAngle = 20;
        float maxSlopeChange = 40;
        float slopeChange = elevationAngle/maxSlopeChange;
        float expectedSlopeFactor = slopeChange*slopeChange;

        //Due to floating point error, needed to test within a given range. 
        //as expected was 0.25 and received was 0.2499994
        Assert.That(slopeFactor, Is.EqualTo(expectedSlopeFactor).Within(0.001f));
    }
}
