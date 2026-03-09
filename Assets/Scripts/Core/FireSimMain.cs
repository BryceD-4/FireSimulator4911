/**
Forest Fire Simulator - Unity
Bryce Dixon T00054766 Comp 4911 Capstone March 2026

This file is the central coordinator for all code in the fire simulator. 
It initializes all items which need to be initialized and coordinates the update loop. 
*/
using UnityEngine;

public class FireSimMain : MonoBehaviour
{
   //Get a reference of all objects which have a monobehaviour that 
   //needs to be iniitalized and/or called every frame. 
    public GridManager gridManager;
    public GridInteractor gridInteractor;
    public BurningCellManager burningCellManager;
    public UI_Manager uI_Manager;
    public GridMeshOverlay gridMeshOverlay;

    //This determines the size of the game grid. All other files are 
    //referencing the grid from this central value. This is the only spot it
    //needs to be changed. 
    private int gridSize = 100;
    
    //Initialize everything that needs to be - called once
    void Start()
    {
        //Build the 2D backend grid, populate cells with GRidCell objects
        gridManager.InitializeGrid(gridSize, gridSize);
        //Get the grid size values for accurate user-grid selection
        gridInteractor.InitializeInteractor(gridSize, gridSize);
        //Set up the manager to create ignitionProbability manager
        burningCellManager.InitializeManager();
        //create event listeners for the user input
        uI_Manager.InitializeUIManager();
        //Create the grid overlay for visuals, ensuring it is the same size as the grid created. 
        gridMeshOverlay.Initialize(gridSize, gridSize, gridManager.GetCellSize());
    }

    //Updarte loop is called once per frame
    void Update()
    {
        //This ensures the simulator does not start until the user enters their values
        if (uI_Manager.UserInputReceived())
        {
            //Grid interactor detects when the user has clicked on the map and ignites the fire there
            gridInteractor.UpdateInteractor();
            //This iterates through the burning cells and extinguishes them/ ignites their neighbours as needed.
            burningCellManager.UpdateBurningCells();
        }
        

    }
}
