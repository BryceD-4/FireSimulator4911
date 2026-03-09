/**
Forest Fire Simulator - Unity
Bryce Dixon T00054766 Comp 4911 Capstone March 2026

This class creates a GridCell object which populates each cell of the 
2D grid. This class:
-  calculates the fuel load of the cell when called by GridManager. 
- Holds the burn time and timer for each cell's extinguishing
- holds the location and elevation for each cell

*/

using UnityEngine;
//Needed for HashSet<>
using System.Collections.Generic;

public class GridCell
{
    private int x, z;
    public float elevation;
    //This simply holds the costs of the fuel objects.
    //All cells use the same library
    public static Fuel fuelLibrary = new Fuel();
    //This is how much fuel is on this cell
    public float fuelLoad;

    //Booleans are used as a simple state machine for the cell
    public bool isBurning;
    public bool isExtinguished;

    //This is to make the cells burn out after a period of time
    //timer = how long it has burned, duration = total burn time
    public float maxBurnDuration = 2.0f;
    public float burnTimer = 0f;

    public GridCell(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    //Called by GridManager on iniitalization of the gridCell
    public void SetCellFuelValue(Vector3 worldPosition, float cellSize)
    {
        //all cells begin with regular fuel
        fuelLoad = fuelLibrary.regularFuel;

        //REF: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Physics.OverlapBox.html
        //Increased y value to detect when objects are just below or above the slope surface
        //As trees were sunken in and not being detected. 
        //Adjusting the y position of this vector 3 affects how large the box is that we are detecting, 12 seemed to be the sweet spot. 
        Collider[] cellFuelHits = Physics.OverlapBox(
            worldPosition, 
            new Vector3(cellSize, 12.0f, cellSize)*0.5f,
            Quaternion.identity,
            LayerMask.GetMask("FuelObject")
        );

        //This is used to ensure the same object is not counted multiple times per cell
        HashSet<GameObject> countedFuelObjects = new HashSet<GameObject>();
        foreach(Collider collided in cellFuelHits)
        {
            //Get the root object of the collided object
            //This is to stop the same object from being detected multiple times
            //REF: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Transform-root.html
            Transform currentObj = collided.transform;
            //This will hold the root object when found below
            GameObject rootObject = null;
            
            //Need to start at the object found and iterate up to the main parent of that plant. 
            //This avoids counting each of the objects of a given plant as a different plant, 
            //i.e. each plant counted only once
            //REF: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Transform-parent.html
            while(currentObj != null)
            {
                
                if(currentObj.CompareTag("Grass") || currentObj.CompareTag("LowTree") || currentObj.CompareTag("TallTree"))
                {
                    //If we are at a plant object level
                    //Stop here and end the loop
                    rootObject = currentObj.gameObject;
                    break;
                }
                //Move up one level to get parent and continue
                currentObj = currentObj.parent;
            }

            //null was added as a guard against false detection
            //This if was added so that objects are only counted once
            if (rootObject != null && !countedFuelObjects.Contains(rootObject))
            {
                //If the root is not already counted, we count it and 
                //update the cell for this object.
                countedFuelObjects.Add(rootObject);
                if (rootObject.CompareTag("Grass"))
                {
                    //Grass is very combustable
                    this.fuelLoad += fuelLibrary.grassFuel;

                } else if (rootObject.CompareTag("LowTree"))
                {
                    //Low tree has ladder fuel and is more combustable
                    this.fuelLoad += fuelLibrary.lowTreeFuel;

                }else if (rootObject.CompareTag("TallTree"))
                {
                    //If it is a tall tree, we want the fire to move slower
                    //So make = and break loop here
                    this.fuelLoad = fuelLibrary.tallTreeFuel;
                    break;
                }
            }
        }

        //DEBUG only
        // if(fuelLoad == fuelLibrary.tallTreeFuel || fuelLoad > fuelLibrary.regularFuel){
        //     Debug.Log("Cell Fuel Val = " + fuelLoad);
        // }

    }
    public int GetCellX()
    {
        return x;
    }
    public int GetCellZ()
    {
        return z;
    }
}
