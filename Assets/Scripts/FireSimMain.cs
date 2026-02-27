using UnityEngine;

public class FireSimMain : MonoBehaviour
{
    public GridManager gridManager;
    public GridInteractor gridInteractor;
    // RotatingCamera rotateCamera;
    private int gridSize = 100;
    
    void Start()
    {
        gridManager.InitializeGrid(gridSize, gridSize);
        gridInteractor.Start();
    }

    void Update()
    {
        gridInteractor.Update();
    }
}
