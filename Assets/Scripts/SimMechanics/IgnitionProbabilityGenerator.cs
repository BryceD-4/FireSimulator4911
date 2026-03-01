using UnityEngine;

public class IgnitionProbabilityGenerator
{
    private float cellSize;
    private float distBetweenAdjacentCells;
    private float distBetweenDiagonalCells;

    private UserData rawUserValues;

    private float windWeight = 0.15f; 
    private float temperatureWeight = 0.35f; 
    private float slopeWeight = 0.1f; 
    private float fuelWeight = 0.35f;
    private float humidityWeight = 0.4f;

    // private float baseProb = 0.05;

    //These factors are the same for all cells
    private float temperatureFactor, humidityFactor;

    //As the probability value is between 0 and the max fuel amount, using inverseLerp to get this between 0 and 1
    private static float maxFuelAmount = 2.0f;

    public IgnitionProbabilityGenerator(float cellSize)
    {
        this.cellSize = cellSize;
        //Use the cell size to calculate the distance between cells to use for
        //the slope calculations
        distBetweenAdjacentCells = cellSize;
        //Diagonal of a square cell is a multiple of sqrt(2) (pythagorean theorem)
        distBetweenDiagonalCells = cellSize*Mathf.Sqrt(2f);
    }
    public void SetUserInput(UserData input)
    {
        rawUserValues = input;
        //Once we get the user values, we can calculate the temperature and humidity
        //values, as these will be the same for all cells.
        temperatureFactor = CalculateTemperatureFactor();
        humidityFactor = CalculateHumidityFactor();
    }
    
    public float GetIgnitionProbability(GridCell mainCell, GridCell neighbour)
    {
        float weightedProbabilities;
        float cellIgnitionProbability;
        float windFactor;
        float slopeFactor; 
        float fuelFactor;

        windFactor = CalculateWindFactor(mainCell.GetCellX(), mainCell.GetCellZ(), neighbour.GetCellX(), neighbour.GetCellZ());
        slopeFactor = CalculateSlopeFactor(mainCell, neighbour);
        fuelFactor = CalculateFuelFactor(neighbour);

        weightedProbabilities = 
        temperatureWeight*temperatureFactor +
        slopeWeight*slopeFactor +
        windWeight*windFactor + 
        fuelWeight*fuelFactor +
        humidityWeight * humidityFactor;

        
        // cellIgnitionProbability = weightedProbabilities * humidityFactor;
        cellIgnitionProbability = weightedProbabilities * 0.006f;
        
        Debug.Log("Temp Factor = " + temperatureFactor + " HumidFact = " + humidityFactor + " slopeFactor = " 
        + slopeFactor +" windFactor = " + windFactor + " FuelFact = " + fuelFactor +" Prob = " + cellIgnitionProbability);

        return cellIgnitionProbability;
    }

    //Fuel --> linear
    private float CalculateFuelFactor(GridCell neighbour)
    {
        //Get the fuel factor from this cell
        float fuelValue = neighbour.fuelLoad;

        //Clamp the fuel value between 0 and 1. 
        float normalizedFuel = Mathf.InverseLerp(0f, maxFuelAmount, fuelValue);

        return normalizedFuel;
    }

    //Temp --> exponential
    private float CalculateTemperatureFactor()
    {
        float temperatureVal = rawUserValues.temperature/rawUserValues.maxTemperature;
        return temperatureVal*temperatureVal;
    }

    //Humidity --> exponential 
    private float CalculateHumidityFactor()
    {
        //humidity is always out of 100%
        //Subtract to get how "dry" it is.
        float humidity = 1f-(rawUserValues.humidity/100f);  
        //Squared curve (exponential)
        float dryLevel = humidity * humidity;
        //Drylevel is how dry it is and how dry it is affects all other variables
        return dryLevel; 
    }

    //Wind --> Linear, Make exponential?
    private float CalculateWindFactor(int mainX, int mainZ, int neighbourX, int neighbourZ)
    {
        //WIND SPEED
        //Need to map the wind speed on a range of 0-200
        float windSpeedValue = rawUserValues.windSpeed/rawUserValues.maxWindSpeed;

        //WIND DIRECTION
        Vector2 windDirection = GetWindDirection();
        int changeX = neighbourX-mainX;
        int changeZ = neighbourZ-mainZ;
        Vector2 directionToNeighbour = new Vector2(changeX, changeZ).normalized;
        //This will be >0, 0, <0
        //Once normalized, is now 1, 0, -1
        float windDirectionValue = Vector2.Dot(windDirection, directionToNeighbour);

        float windFactor = Mathf.Max(0,windDirectionValue)*windSpeedValue;

        // Debug.Log("WIND --> direction: "+windDirection.ToString() + " DirToNeigh: "+directionToNeighbour.ToString() 
        // + " WindDirVal: "+windDirectionValue.ToString());
        //Now just get the wind direction and compound this with the wind speed. 
        //If the value is 0 and less than 0, there is no wind factor at play
        return windFactor;
    }
    private Vector2 GetWindDirection()
    {
        //7:NW  0:N  1:NE → (-1,1)  (0,1)  (1,1)
        //6:W         2:E → (-1,0)  (0,0)  (1,0)
        //5:SW  4:S  3:SE → (-1,-1) (0,-1) (1, -1)

        switch (rawUserValues.windDirection)
        {
            case 0: return new Vector2(0,1);
            //Returns vector == 1 and not 1.41
            case 1: return new Vector2(1,1).normalized;
            case 2: return new Vector2(1,0);
            case 3: return new Vector2(1,-1).normalized;
            case 4: return new Vector2(0,-1);
            case 5: return new Vector2(-1,-1).normalized;
            case 6: return new Vector2(-1,0);
            case 7: return new Vector2(-1,1).normalized;
            default: return new Vector2(0,0);
        }
    }

    //SLOPE --> exponential
    private float CalculateSlopeFactor(GridCell mainCell, GridCell neighbour)
    {
        //Look at elevation between neighbour and main
        //If increases, then higher slope factor
        float elevChange = neighbour.elevation - mainCell.elevation;
        //Now determine if the cells are diagonal from one another or adjacent
        float changeX = Mathf.Abs(neighbour.GetCellX() - mainCell.GetCellX());
        float changeZ = Mathf.Abs(neighbour.GetCellZ() - mainCell.GetCellZ());
        float distanceBetweenCells = distBetweenAdjacentCells;
        //Diagonal will have x and z both >0
        if(changeX > 0 && changeZ > 0)
        {
            //If the cells are diagonal
            distanceBetweenCells = distBetweenDiagonalCells;
        }

        //Now we have opposite and adjacent to get angle = Tan (TOA)
        //tan-1(O/A), arctan, = arctan(elevChange/distancebtw) = angle
        //Mathf = unity, MathF = C# system setting
        float slopeAngleRad = Mathf.Atan(elevChange/distanceBetweenCells);
        //arc tan is in radians, not degrees: https://www.geeksforgeeks.org/maths/arctan/
        //Convert to degrees --> https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Mathf.Rad2Deg.html
        float slopeAngleDeg = slopeAngleRad*Mathf.Rad2Deg;


        float slopeFactor = 0f;
        if(slopeAngleDeg > 0f)
        {
            //If it is an uphill slope, then it modifies the outcome
            //gradual increase in fire behaviour, exponential, with 40 degree slope being an apex
            // https://www.researchgate.net/figure/The-effect-of-slope-steepness-on-the-uphill-rate-of-spread-of-free-burning-wildland-fires_fig2_313350325
            //Use 40 degree as cutoff, clamp the slope at 40
            //Clamp01 clamps between 0 and 1: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Mathf.Clamp01.html 
            float normalizedSlope = Mathf.Clamp01(slopeAngleDeg/40f);
            //Now square the values to create an exponential curve
            slopeFactor = normalizedSlope * normalizedSlope;
        }
        // Debug.Log("Change x, z: "+changeX+", "+changeZ+" Distance between: " + distanceBetweenCells + " ElevChange: "+elevChange + " SlopeDegrees: " + slopeAngleDeg + " Slope factor: " + slopeFactor);

        return slopeFactor;

    }
}
