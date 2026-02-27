using UnityEngine;

public class FireSimMain : MonoBehaviour
{
    public GridManager gridManager;
    // RotatingCamera rotateCamera;
    private int gridSize = 100;
    
    void Start()
    {
        gridManager.InitializeGrid(gridSize, gridSize);
    }

    void Update()
    {
        
    }
}
