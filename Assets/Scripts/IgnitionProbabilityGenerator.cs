using UnityEngine;

public class IgnitionProbabilityGenerator
{
    private float cellSize;
    private float distBetweenAdjacentCells;
    private float distBetweenDiagonalCells;

    private UserData rawUserValues;

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
        Debug.Log("PROB USER. VAL TEMP = " + rawUserValues.temperature);
    }
    public float GetIgnitionProbability(GridCell mainCell, GridCell neighbour)
    {
        float weightedProbabilities;
        float cellIgnitionProbability;
        float windFactor;
        float slopeFactor; 
        float fuelFactor;

        return 1;
    }

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
    public Vector2 GetWindDirection()
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

    // private float CalculateFuelFactor(Cell neighbour)
    // {
    //     //Get the fuel factor from this cell
    //     float fuelValue = neighbour.fuelLoad;

    //     //Clamp the fuel value between 0 and 1. 
    //     float normalizedFuel = Mathf.InverseLerp(0f, maxFuelAmount, fuelValue);

    //     return normalizedFuel;
    // }
}
