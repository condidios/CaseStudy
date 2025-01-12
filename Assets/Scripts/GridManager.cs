using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int logicalGridSize = 5;
    public int cellSize = 1;
    public Transform gridParent;
    public GameObject gridCellPrefab;

    public BlockGenerator.Block[,] logicalGrid;

    void Start()
    {
        InitializeGrid();
        DrawGrid();
    }

    void InitializeGrid()
    {
        logicalGrid = new BlockGenerator.Block[logicalGridSize, logicalGridSize];
    }

    void DrawGrid()
    {
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
        return gridParent.position.y;
    }
    public void UpdateGridAfterRemoval(int column)
    {
        for (int row = logicalGridSize - 2; row >= 0; row--)
        {
            if (logicalGrid[row, column] != null)
            {
                int newRow = FindLowestEmptyRow(column, row);
                if (newRow != row)
                {
                    var block = logicalGrid[row, column];
                    logicalGrid[newRow, column] = block;
                    logicalGrid[row, column] = null;

                    Vector3 newPosition = new Vector3(column * cellSize, -newRow * cellSize, 0);
                    block.BlockObject.transform.position = newPosition;
                }
            }
        }
    }

    private int FindLowestEmptyRow(int column, int startRow)
    {
        for (int row = logicalGridSize - 1; row > startRow; row--)
        {
            if (logicalGrid[row, column] == null)
            {
                return row;
            }
        }
        return startRow;
    }

}