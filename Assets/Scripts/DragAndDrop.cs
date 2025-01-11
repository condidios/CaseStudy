using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 offset; // Offset between mouse position and block center
    private bool isDragging = false;

    public float snapSize = 1f; // Size of each visual grid cell (e.g., 1 unit for a 5x5 visual grid)

    void OnMouseDown()
    {
        // Capture the offset between mouse position and block position
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        offset = transform.position - mouseWorldPos;
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Follow the mouse position while maintaining offset
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            transform.position = mouseWorldPos + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Snap the block to the nearest grid cell
        Vector3 snappedPosition = SnapToGrid(transform.position);
        transform.position = snappedPosition;

        // Convert position to logical grid coordinates and update the grid
        Vector2Int logicalGridPos = ConvertToLogicalGrid(snappedPosition);
        UpdateLogicalGrid(logicalGridPos);
    }

    Vector2Int ConvertToLogicalGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / snapSize * 4); // Multiply by 4 to account for smaller cubes
        int y = Mathf.RoundToInt(worldPosition.y / snapSize * 4);
        return new Vector2Int(x, y);
    }

    void UpdateLogicalGrid(Vector2Int gridPosition)
    {
        // Update the 20x20 logical grid with the block's color data
        Debug.Log($"Block placed at logical grid position: {gridPosition}");
        // Add your logical grid update logic here
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.nearClipPlane; // Distance from the camera
        return Camera.main.ScreenToWorldPoint(mouseScreenPos);
    }

    Vector3 SnapToGrid(Vector3 worldPosition)
    {
        float snappedX = Mathf.Round(worldPosition.x / snapSize) * snapSize;
        float snappedY = Mathf.Round(worldPosition.y / snapSize) * snapSize;
        return new Vector3(snappedX, snappedY, worldPosition.z);
    }
}

