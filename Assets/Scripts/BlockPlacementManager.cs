using UnityEngine;

public class BlockPlacementManager : MonoBehaviour
{
    public GridManager gridManager;
    public BlockGenerator blockGenerator;
    public float hoverHeight = 2f;
    public PoppingSystem poppingSystem;
    private BlockGenerator.Block currentBlock;
    private GameObject currentBlockObject;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        SpawnNewBlock();
    }

    void Update()
    {
        HandleBlockHovering();
        HandleBlockPlacement();
    }

    void SpawnNewBlock()
    {
        currentBlock = blockGenerator.GenerateBlock();
        currentBlockObject = blockGenerator.InstantiateBlock(currentBlock);
        currentBlockObject.transform.position = new Vector3(0, hoverHeight, 0);
    }

    void HandleBlockHovering()
    {
        if (currentBlockObject == null) return;

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        float clampedX = Mathf.Clamp(mouseWorldPosition.x, 0, gridManager.logicalGridSize - 1);
        currentBlockObject.transform.position = new Vector3(clampedX, hoverHeight, 0);
    }

    void HandleBlockPlacement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int column = Mathf.RoundToInt(mouseWorldPosition.x);

            column = Mathf.Clamp(column, 0, gridManager.logicalGridSize - 1);

            int row = FindLowestEmptyRowInColumn(column);

            if (row >= 0) 
            {
                gridManager.logicalGrid[row, column] = currentBlock;
                Vector3 dropPosition = new Vector3(column, -row, 0);
                currentBlockObject.transform.position = dropPosition;
                
                poppingSystem.CheckAndPopSegments(row, column);
                
                currentBlockObject = null;
                
                SpawnNewBlock();
            }
            else
            {
                Debug.Log("Column is full!");
            }
        }
    }
    int FindLowestEmptyRowInColumn(int column)
    {
        for (int row = gridManager.logicalGridSize - 1; row >= 0; row--)
        {
            if (gridManager.logicalGrid[row, column] == null)
            {
                return row;
            }
        }
        return -1; 
    }
}
