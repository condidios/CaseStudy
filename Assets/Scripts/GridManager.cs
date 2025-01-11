using UnityEngine;
using System;

public class GridManager : MonoBehaviour
{
    public int logicalGridSize = 5; // Logical grid size
    public BlockGenerator.Block[,] logicalGrid;    // Holds colors of small cubes
    public float cellSize = 0.25f;  // Each small cube's size in Unity units
    public Transform gridParent;   // Parent for visual grid cells
    public GameObject cellPrefab;  // Prefab for visual cells (optional, for debugging)

    void Start()
    {
        InitializeGrid();
        CreateVisualGrid();
    }

    void InitializeGrid()
    {
        logicalGrid = new BlockGenerator.Block[logicalGridSize, logicalGridSize];
    }

    void CreateVisualGrid()
    {
        for (int row = 0; row < logicalGridSize; row++)
        {
            for (int col = 0; col < logicalGridSize; col++)
            {
                Vector3 position = new Vector3(col, -row, 0); // Adjust based on origin
                Instantiate(cellPrefab, position, Quaternion.identity, gridParent);
            }
        }
    }
    
    
}
