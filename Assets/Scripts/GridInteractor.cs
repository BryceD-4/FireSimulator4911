using UnityEngine;
using UnityEngine.InputSystem;

//This stores the mouse clicks and hover functions
public class GridInteractor : MonoBehaviour
{
    public GridManager gridManager;
    //Needed so that if a cell is clicked, we can ignite it
    public BurningCellManager burnCellManager;
    float cellSize;
    public GameObject cursorHighlighter;

    private int gridLength, gridWidth;
    public void InitializeInteractor(int gridL, int gridW)
    {
        gridLength = gridL;
        gridWidth = gridW;

        cellSize = gridManager.GetCellSize();
        
        //Get the cursor highlighter the same size as the cells in the grid
        cursorHighlighter.transform.localScale = new Vector3(cellSize,cellSize,cellSize);
    }

    public void Update()
    {
        DetectMouseHoverCell();

        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            IgniteCellUnderMouse();
        }
    }

    void IgniteCellUnderMouse()
    {
        //same as above, we get the ray to the point on the screen where the mouse is
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        //In unity, we had to take the higlighter and in inspector, turn it from Layer: default to
        //Layer: ignore raycast, so that when we click, it is not just hitting the highlighter object, 
        // but rather the terrain below it (the next item down). 
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.GetComponent<Terrain>())
            {
                //As we did above, get the point of contact
                Vector3 worldPos = hit.point;

                //Get the x and z positions of this point
                int xIndex = Mathf.FloorToInt(worldPos.x / cellSize);
                int zIndex = Mathf.FloorToInt(worldPos.z / cellSize);

                GridCell currentCell = gridManager.GetMapCell(xIndex, zIndex);
                //If the cell is within the game grid
                if (IsValidCell(xIndex, zIndex))
                {
                    //Only do this if the cell is not already burning
                    if (!currentCell.isBurning)
                    {
                        //Set the cell to burning in the game grid
                        currentCell.isBurning = true;

                        Debug.Log("Cell CLicked = " + xIndex +", " + zIndex);
                        //Make the visual representation of burning
                        // SpawnBurningVisual(xIndex, zIndex);
                        burnCellManager.IgniteCellVisually(xIndex, zIndex);
                    }
                }
            }
        }
    }

     bool IsValidCell(int x, int z)
    {
       //Make sure the cell is within the game grid
       //Make sure is it within the parameters of the game grid
        return x >= 0 && x < gridWidth &&
            z >= 0 && z < gridLength;
    }

    void DetectMouseHoverCell()
    {
          //Ray Object = an infinite line from an origin pointing in a direction
        //Constructor = Ray (Vector3 origin, Vector3 direction)
        //REF: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Ray-ctor.html
        
        //ScreenPointToRay(Vector3 position); --> https://docs.unity3d.com/ScriptReference/Camera.ScreenPointToRay.html?ampDeviceId=34478755-7e9d-483f-8793-713f18b2aa5f&ampSessionId=1771715737117&ampTimestamp=1771803587978
        //This returns a ray going from the camera through the specific point on the screen
        //Input.mousePoisiton (is deprecated, but still works in this instance): https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Input-mousePosition.html
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());


        //RaycastHit is used to get information from a RayCast object
        //RayCast object --> casts a ray from origin, in direction, to maxdistance, against all scene colliders
        //REF: https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Physics.Raycast.html
        //Ray = a structural line moving through 3D space, RayCast = a line that performs a physics collision detection test
        RaycastHit hit;

        //Use the ray (the 3D line) to find the line of sight, while hit holds information of the first item that was hit on this line
        if (Physics.Raycast(ray, out hit))
        {
            //If the item that was collided with on this ray was terrain, enter here
            if (hit.collider.GetComponent<Terrain>())
            {
                //This gets the point where the ray hit the terrain
                Vector3 worldPos = hit.point;
                //Get the x and z coordinates of the intersection point
                int xIndex = Mathf.FloorToInt(worldPos.x / cellSize);
                int zIndex = Mathf.FloorToInt(worldPos.z / cellSize);

                //Turn the point found into a vector3 object so that we can get the y posiiton down below to place the obejct
                Vector3 cellWorldPosition = new Vector3(
                    xIndex * cellSize + cellSize / 2,
                    0,
                    zIndex * cellSize + cellSize / 2
                );

                //Get the height of the cell as we did above
                //Other alternative would be to get the elevation of this cell in the grid
                float height = Terrain.activeTerrain.SampleHeight(cellWorldPosition);
                //Place the object position jsut above this height
                cellWorldPosition.y = height + 0.1f;
                //Place our cursor object overtop of this placement
                cursorHighlighter.transform.position = cellWorldPosition;
            }
        }
    }
}
