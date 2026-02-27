using UnityEngine;

public class GridManager : MonoBehaviour
{
    //100 was chosen to reduce CPU load, as 150 and 200 were very slow
    private float terrainWidth, terrainLength;
    private float cellSize;

    private GridCell [,] mapGrid;

    private GameObject[,] gridVisuals;

    public void InitializeGrid(int gridWidth, int gridLength)
    {
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
        gridVisuals = new GameObject[gridWidth, gridLength];

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
        //This is for the visual above the cell
        gridVisuals[x,z] = new GameObject();

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

//Hold for now until get to this stage of design
        // nextCell.SetCellFuelValue(worldPosition, cellSize);

    }

    

    public GridCell GetMapCell(int x, int z)
    {
        return mapGrid[x,z];
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    void Update()
    {
        
    }
}
