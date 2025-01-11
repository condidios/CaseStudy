using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int logicalGridSize = 5; // 5x5 grid
    public int cellSize = 1;        // Size of each cell
    public Transform gridParent;   // Parent object for visual grid
    public GameObject gridCellPrefab; // Prefab for visual grid cells

    public BlockGenerator.Block[,] logicalGrid; // Tracks the state of each cell (null if empty)

    void Start()
    {
        InitializeGrid();
        DrawGrid();
    }

    void InitializeGrid()
    {
        // Initialize logical grid with empty cells
        logicalGrid = new BlockGenerator.Block[logicalGridSize, logicalGridSize];
    }

    void DrawGrid()
    {
        // Draw a visual representation of the grid
        for (int x = 0; x < logicalGridSize; x++)
        {
            for (int y = 0; y < logicalGridSize; y++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, -y * cellSize, 0);
                Instantiate(gridCellPrefab, cellPosition, Quaternion.identity, gridParent);
            }
        }
    }

    public bool IsCellEmpty(int row, int column)
    {
        return logicalGrid[row, column] == null;
    }
    public float GetGridTopY()
    {
        // Calculate the top Y position of the grid
        return gridParent.position.y; // Grid's top Y is the parent transform's Y position
    }

}