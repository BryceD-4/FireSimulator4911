/**
Forest Fire Simulator - Unity
Bryce Dixon T00054766 Comp 4911 Capstone March 2026

This program handles the creation of the 2D backend grid which is populated with
GridCell objects. 
It also sets the elevation value for each gridcell based on the Terrain object
and sets the fuel value for the cells. 
*/

using UnityEngine;

public class GridManager : MonoBehaviour
{
    //Holds the actual terrain width and length from Unity Terrain obj
    private float terrainWidth, terrainLength;
    //The L and W of 2D grid
    private int gridWidth, gridLength;
    //Calculated from terrain and grid dimensions
    private float cellSize;

    //This is the 2D backend grid
    private GridCell [,] mapGrid;

    //For debugging
    //This was used for early iterations
    // private GameObject[,] gridVisuals;

    public void InitializeGrid(int gridW, int gridL)
    {
        gridWidth = gridW;
        gridLength = gridL;
        //Calculate the size of the grid based off of the terrain size.
        //This gets our main terrain object, we only have one, so no issues with unpredictable behaviour
        Terrain mainTerrain = Terrain.activeTerrain;
        //Get the actual dimensions of the game terrain
        terrainWidth = mainTerrain.terrainData.size.x;
        terrainLength = mainTerrain.terrainData.size.z;
        //Get the cell size from the grid and terrain dimensions
        //Our grid is always square, so can use width of length.
        cellSize = terrainWidth/ gridWidth;


        //initialize the grids
        mapGrid = new GridCell[gridWidth, gridLength];

        //used for debugging
        // gridVisuals = new GameObject[gridWidth, gridLength];

        //Populate our game grid with gridcells
        for(int x=0; x<gridWidth; x++)
        {
            for(int z=0; z < gridLength; z++)
            {
                PopulateGridCell(x,z);
            }
        }
    }

    private void PopulateGridCell(int x, int z)
    {
        GridCell nextCell = new GridCell(x,z);
        //Initialize the grid cell itself
        mapGrid[x,z] = nextCell;

        //Used for debugging
        //This is for the visual above the cell
        // gridVisuals[x,z] = new GameObject();

        //creates a 3D vector
        //Vector3(float x, float y, float z)
        //Keep it in 2D, we simply set the y dimension to 0
        //REF: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Vector3-ctor.html
        Vector3 worldPosition = new Vector3(
            x*cellSize + cellSize/2, 
            0, 
            z*cellSize + cellSize/2
        );

        //Set the elevation to the elvation on the map
        //This requires the use of a Vector3 object, so that is why we used it above. 
        //SampleHeight(Vector3 worldPosition)
        //REF: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Terrain.SampleHeight.html
        float cellHeight = Terrain.activeTerrain.SampleHeight(worldPosition);
        //Set the cell elevation
        nextCell.elevation = cellHeight;
        //Set the world position y value to cell height for it to be at the surface of the cell
        //Used for collider down below
        worldPosition.y = cellHeight;

        //Uses colliders to detect wihch fuel is in this cell.
        nextCell.SetCellFuelValue(worldPosition, cellSize);

    }

    public GridCell GetMapCell(int x, int z)
    {
        return mapGrid[x,z];
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public int GetGridWidth()
    {
        return gridWidth;
    }
    public int GetGridLength()
    {
        return gridLength;
    }

    // For debugging
    // public GameObject GetGridVisualCell(int x, int z)
    // {
    //     return gridVisuals[x,z];
    // }
}
