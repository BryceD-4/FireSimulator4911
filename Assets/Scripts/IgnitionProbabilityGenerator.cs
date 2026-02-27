using UnityEngine;

public class IgnitionProbabilityGenerator
{
    private float cellSize;
    private float distBetweenAdjacentCells;
    private float distBetweenDiagonalCells;

    public IgnitionProbabilityGenerator(float cellSize)
    {
        this.cellSize = cellSize;
        //Use the cell size to calculate the distance between cells to use for
        //the slope calculations
        distBetweenAdjacentCells = cellSize;
        //Diagonal of a square cell is a multiple of sqrt(2) (pythagorean theorem)
        distBetweenDiagonalCells = cellSize*Mathf.Sqrt(2f);
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
}
