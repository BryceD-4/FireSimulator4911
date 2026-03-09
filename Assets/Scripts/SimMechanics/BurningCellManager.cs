/**
Forest Fire Simulator - Unity
Bryce Dixon T00054766 Comp 4911 Capstone March 2026

This program tracks and iterates through all cells which are burning
and calls the ignitionprobability generator on any cells to look at igniting.

*/

using UnityEngine;
//Needed For List<>() and HashSet<>()
using System.Collections.Generic;

public class BurningCellManager : MonoBehaviour
{
    //Used hashset as removal performance is better than list
    public HashSet<GridCell> burningCells = new();

    //Used for probability of ignition
    private System.Random myRandom = new System.Random();
    //Grid manager needed to get the cell to modify
    public GridManager gridManager;
    
    //Used for debugging to display the cell being selected
    // public GameObject burningCellPrefab;

    //Used to get the probability of ignition
    public IgnitionProbabilityGenerator igniteProbGen;

    //The game object which houses the grid mesh.
    public GridMeshOverlay gridMeshOverlay;

    //Added as all levels of ignition were travelling to fast, so this limits
    //how often neighbouring cells are checked for ignition
    private float neighbourIgnitionTimer;

    //Called by FireSimMain
    public void InitializeManager()
    {
        //Create a singular probability generator for use
        igniteProbGen = new IgnitionProbabilityGenerator(gridManager.GetCellSize());

        neighbourIgnitionTimer = 0;
    }

    //Loops through all burning cells, checks their neighbours for ingition probability. 
    //also checks if the cell is to be extinguished. 
    public void UpdateBurningCells()
    {
        //Increment the time once per frame
        neighbourIgnitionTimer++;
        
        //Need to keep a list of cells to ignite, or else creates a rippling effect
        //where the cell you just ignited causes this to run again.
        List<GridCell> cellsToIgnite = new();

        //PErformance was tanking when cells began to extinguish, so separating ignition and
        //extinguishing to mitigate/ separate overhead
        List<GridCell> cellsToExtinguish = new();

        foreach (GridCell cell in burningCells)
        {
            //Added to limit how often neighbouring cells are checked
            //as rate of spread was simply too fast at all levels.
            if(neighbourIgnitionTimer %8 == 0){
                //Get its neighbours probability of igniting
                //This returns an appended list of neighbours to ignite
                cellsToIgnite = IgniteNeighboursIfAble(cellsToIgnite, cell.GetCellX(), cell.GetCellZ());
            }
            //Check if this cell has burned out
            cell.burnTimer += Time.deltaTime;
            if(cell.burnTimer >= cell.maxBurnDuration)
            {
                //EXTINGUISH THE CELL if it has been bruning longer than the burn duration
                cellsToExtinguish.Add(cell);
            }
        }

        //Handle all ignition cells after iteration
        //Added as doing this in line was dratically impacting performance
        //one multiple cells were in burn list
        foreach(GridCell cell in cellsToIgnite)
        {
            //Cells may be repeated in list, so check the cell has not been tended to yet
            if (!cell.isBurning)
            {
                //IGNITE CELL
                IgniteCellVisually(cell.GetCellX(), cell.GetCellZ());
            }
        }

        //Handle all extinguish cells after the igniting
        //added for same performance consideration, calling this in line 
        // was a performance sink.
        foreach(GridCell cell in cellsToExtinguish)
        {
            //Cells may be repeated in the list, so check it has not been
            //already tended to
            if (!cell.isExtinguished)
            {
                //Extinguish the cell
                BurnOutCell(cell.GetCellX(), cell.GetCellZ());
            }
        }
    }

    //Called from aboe when want to check if neighbouring cell is reaching
    //ignition probability
    List<GridCell> IgniteNeighboursIfAble(List<GridCell> neighbours, int xMainCell, int zMainCell)
    {
        float ignitionProbability;
        //Get the cell of interest
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

                if (nextNeigh.isBurning || nextNeigh.isExtinguished)
                {
                    //If this cell is already burnt or is burning, continue
                    //This will also take care of re-visiting the cell of interest (i.e. maincell)
                    continue;
                }

                //Get ignition Probability for this cell
                ignitionProbability = igniteProbGen.GetIgnitionProbability(mainCell, nextNeigh);
                float nextRand = (float)myRandom.NextDouble();

                //a random value produces the cutoff for the ignition of cells
                //using a threshhold worked but produced very structured burns, this adds more 
                //variety and looks more natural. 
                //Added x2 to slow rate of spread
                if((nextRand * 2) < ignitionProbability)
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

        //Add and mark the cell in the lists
        burningCells.Add(ignitingCell);
        ignitingCell.isBurning = true;

        //Colour is an orange fire colour
        gridMeshOverlay.SetCellColour(x,z, new Color(2.5f, 0.2f, 0.0f, 1.0f));
        

        
    }

    //This makes the cell turn black on the grid overlay
     void BurnOutCell(int x, int z)
    {
        //Get the current cell
        GridCell currentCell = gridManager.GetMapCell(x,z);
        //Remove the cell from the list so it is no longer iterated in future.
        burningCells.Remove(currentCell);

        //Set booleans for communication/ state change monitoring
        currentCell.isBurning = false;
        currentCell.isExtinguished = true;
        
        //Set the colour to a dark burnt colour 
        gridMeshOverlay.SetCellColour(x,z, new Color(0.02f, 0.02f, 0.02f, 1.0f));

    }
    
}//END CLASS
