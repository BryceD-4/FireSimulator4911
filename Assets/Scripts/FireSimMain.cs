using UnityEngine;

public class FireSimMain : MonoBehaviour
{
    public GridManager gridManager;
    public GridInteractor gridInteractor;
    public BurningCellManager burningCellManager;
    public UI_Manager uI_Manager;

    private int gridSize = 100;
    
    void Start()
    {
        gridManager.InitializeGrid(gridSize, gridSize);
        gridInteractor.InitializeInteractor(gridSize, gridSize);
        burningCellManager.InitializeManager();
        uI_Manager.InitializeUIManager();
    }

    void Update()
    {
        //WAIT UNTIL UI IS COMPLETE
        if (uI_Manager.UserInputReceived())
        {
            gridInteractor.Update();
            burningCellManager.UpdateBurningCells();
        }
        

    }
}
