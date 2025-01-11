using UnityEngine;

public class BlockPlacementManager : MonoBehaviour
{
    public GridManager gridManager;        // Reference to the grid manager
    public BlockGenerator blockGenerator; // Reference to the block generator
    public float hoverHeight = 2f;        // Height at which the block hovers above the grid
    public PoppingSystem poppingSystem;
    private BlockGenerator.Block currentBlock; // The currently active block
    private GameObject currentBlockObject;     // GameObject of the current block

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

    // ReSharper disable Unity.PerformanceAnalysis
    void SpawnNewBlock()
    {
        // Generate a new block and its visual representation
        currentBlock = blockGenerator.GenerateBlock(gridManager);
        currentBlockObject = blockGenerator.InstantiateBlock(currentBlock);
        currentBlockObject.transform.position = new Vector3(0, hoverHeight, 0); // Start above the grid
    }

    void HandleBlockHovering()
    {
        if (currentBlockObject == null) return;

        // Convert mouse X position to world position
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        // Clamp mouse position to grid's X range
        float clampedX = Mathf.Clamp(mouseWorldPosition.x, 0, gridManager.logicalGridSize - 1);
        currentBlockObject.transform.position = new Vector3(clampedX, hoverHeight, 0);
    }

    void HandleBlockPlacement()
    {
        if (Input.GetMouseButtonDown(0)) // Detect left-click
        {
            // Determine the column based on mouse X position
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            int column = Mathf.RoundToInt(mouseWorldPosition.x);

            // Ensure column is within grid bounds
            column = Mathf.Clamp(column, 0, gridManager.logicalGridSize - 1);

            // Find the lowest empty cell in the column
            int row = FindLowestEmptyRowInColumn(column);

            if (row >= 0) // If there's space in the column
            {
                // Place the block in the grid
                gridManager.logicalGrid[row, column] = currentBlock;
                Vector3 dropPosition = new Vector3(column, -row, 0);
                currentBlockObject.transform.position = dropPosition;
                
                poppingSystem.CheckAndPopSegments(currentBlock,row, column);

                // Clear reference to the current block
                currentBlockObject = null;

                // Spawn a new block for the next round
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
        return -1; // Column is full
    }
}
