using UnityEngine;

public class GridCell
{
    private int x, z;
    public float elevation;
    public Fuel fuelLibrary;
    public float fuelLoad;

    public bool isBurning;
    public bool isBurned;

     //This is to make the cells burn out after a period of time
    //timer = how long it has burned, duration = total burn time
    public float maxBurnDuration = 5.0f;
    public float burnTimer = 0f;

    public GridCell(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    // public void SetCellFuelValue(Vector3 worldPosition, float cellSize)
    // {
    //     this.fuelLoad = fuelLibrary.regularFuel;

    //     Collider[] cellFuelHits = Physics.OverlapBox(
    //         worldPosition, 
    //         new Vector3(cellSize, 12.0f, cellSize)*0.5f,
    //         Quaternion.identity,
    //         LayerMask.GetMask("FuelObject")
    //     );

    //     //This is used to ensure the same object is not counted multiple times per cell
    //     HashSet<GameObject> countedFuelObjects = new HashSet<GameObject>();
    //     foreach(Collider collided in cellFuelHits)
    //     {
    //         GameObject rootObject = collided.transform.root.gameObject;
    //         if (!countedFuelObjects.Contains(rootObject))
    //         {
    //             countedFuelObjects.Add(rootObject);
    //             if (rootObject.CompareTag("Grass"))
    //             {
    //                 //Grass is very combustable
    //                 this.fuelLoad += fuelLibrary.grassFuel;

    //             } else if (rootObject.CompareTag("LowTree"))
    //             {
    //                 //Low tree has ladder fuel and is more combustable
    //                 this.fuelLoad += fuelLibrary.lowTreeFuel;

    //             }else if (rootObject.CompareTag("TallTree"))
    //             {
    //                 //If it is a tall tree, we want the fire to move slower
    //                 //So make = and break loop here
    //                 this.fuelLoad = fuelLibrary.tallTreeFuel;
    //                 break;
    //             }
    //         }
    //     }
    // }
    public int GetCellX()
    {
        return x;
    }
    public int GetCellZ()
    {
        return z;
    }
}
