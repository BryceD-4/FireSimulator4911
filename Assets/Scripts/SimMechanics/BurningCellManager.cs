using UnityEngine;
//For List<>()
using System.Collections.Generic;
public class BurningCellManager : MonoBehaviour
{
    public List<GridCell> burningCells = new();

    //Used for probability of ignition
    private System.Random myRandom = new System.Random();
    public GridManager gridManager;
    public GameObject burningCellPrefab;

    public IgnitionProbabilityGenerator igniteProbGen;
    public void InitializeManager()
    {
        igniteProbGen = new IgnitionProbabilityGenerator(gridManager.GetCellSize());
    }

    public void UpdateBurningCells()
    {
        //Need to keep a list of cells to ignite, or else creates a rippling effect
        //where the cell you just ignited causes this to run again.
        List<GridCell> cellsToIgnite = new();
        foreach (GridCell cell in burningCells)
        {
            //Get its neighbours probability of igniting
            cellsToIgnite = IgniteNeighboursIfAble(cellsToIgnite, cell.GetCellX(), cell.GetCellZ());
            //Check if this cell has burned out
            cell.burnTimer += Time.deltaTime;
            if(cell.burnTimer >= cell.maxBurnDuration)
            {
                //BURN OUT THE CELL
            }
        }

        foreach(GridCell cell in cellsToIgnite)
        {
            //Cells may be repeated in list, so check the cell has not been tended to yet
            if (!cell.isBurning)
            {
                //IGNITE CELL HERE
                IgniteCellVisually(cell.GetCellX(), cell.GetCellZ());

                cell.isBurning = true;
            }
        }
    }

    List<GridCell> IgniteNeighboursIfAble(List<GridCell> neighbours, int xMainCell, int zMainCell)
    {
        float ignitionProbability;
        GridCell mainCell = gridManager.GetMapCell(xMainCell, zMainCell);
        //Loop through all neighbours (8 total), check their probability of igniting
        for(int x = xMainCell-1; x <= xMainCell+1; x++)
        {
            for(int z=zMainCell-1; z<=zMainCell+1; z++)
            {
                int gridLength = gridManager.GetGridLength();
                int gridWidth = gridManager.GetGridWidth();

                if(x<0 || z<0 || x>gridWidth-1 || z > gridLength - 1)
                {
                    //If edge case (OOB), continue to next iteration
                    continue;
                }
                //If not out of bounds, get the cell of interest
                GridCell nextNeigh = gridManager.GetMapCell(x,z);

                if (nextNeigh.isBurning || nextNeigh.isBurned)
                {
                    //If this cell is already burnt or is burning, continue
                    continue;
                }

                //Get ignition Probability for this cell
                ignitionProbability = igniteProbGen.GetIgnitionProbability(mainCell, nextNeigh);
                float nextRand = (float)myRandom.NextDouble();

                if(nextRand < ignitionProbability)
                {
                    //If our probability is higher than the random value
                    //Ignite this cell
                    neighbours.Add(nextNeigh);
                }
            }
        }
        return neighbours;
    }//END METHOD

    public void IgniteCellVisually(int x, int z)
    {
        GridCell ignitingCell = gridManager.GetMapCell(x,z);
        GameObject cellVisual = gridManager.GetGridVisualCell(x,z);
        float cellSize = gridManager.GetCellSize();

        //Get 3D position of the cell
        Vector3 worldPos = new Vector3(
            x * cellSize + cellSize / 2,
            0,
            z * cellSize + cellSize / 2
        );
        worldPos.y = ignitingCell.elevation;

        //For debugging to check what cells are detecting the fuel
        // if(ignitingCell.fuelLoad == 0.4f){

        //Place the prefab object for burning cell, at the world position
        //Instantiate(Object original, Vector3 position, Quaternion rotation)
        //The quaternion (a means of communicating 3D coordinates), is just how to orient the object
        //REF:https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Object.Instantiate.html
        //Quaternion.identity --> This just lines the object up with the world axes, so that it sits square
        //REF: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Quaternion-identity.html
        
        cellVisual = Instantiate(burningCellPrefab, worldPos, Quaternion.identity);
        
        // } //Part of debugging "if" above

        burningCells.Add(ignitingCell);
    }

    //  void BurnOutCell(int x, int z)
    // {
    //     grid[x,z].isBurning = false;
    //     grid[x,z].isBurned = true;
        
    //     SetTerrainLayer(x,z,1);

    //     GameObject cellVisual = gridVisuals[x,z];
    //     if(cellVisual != null)
    //     {
    //         // Renderer renderer = cellVisual.GetComponent<Renderer>();
    //         // renderer.material = burnedMaterial;

    //         Destroy(gridVisuals[x,z]);
    //         gridVisuals[x,z] = null;
    //     }
    // }
    
}//END CLASS
