using UnityEngine;

public class FireSimMain : MonoBehaviour
{
    public GridManager gridManager;
    public GridInteractor gridInteractor;
    public BurningCellManager burningCellManager;

    private int gridSize = 100;
    
    void Start()
    {
        gridManager.InitializeGrid(gridSize, gridSize);
        gridInteractor.InitializeInteractor(gridSize, gridSize);
        burningCellManager.InitializeManager();
    }

    void Update()
    {
        gridInteractor.Update();
        burningCellManager.UpdateBurningCells();

    }
}
