using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoppingSystem : MonoBehaviour
{
    public GridManager gridManager;

    public void CheckAndPopSegments(BlockGenerator.Block block, int row, int column)
    {
        if (block == null) return;

        // List to collect segments for popping
        List<BlockGenerator.Segment> segmentsToPop = new List<BlockGenerator.Segment>();

        // Iterate through each segment of the block
        foreach (var segment in block.Segments)
        {
            foreach (var flag in segment.Flags)
            {
                switch (flag)
                {
                    case BlockGenerator.SegmentFlag.TopLeft:
                        CheckAdjacentSegment(row, column - 1, BlockGenerator.SegmentFlag.TopRight, segment, segmentsToPop); // Check left block's top-right
                        break;

                    case BlockGenerator.SegmentFlag.BottomLeft:
                        CheckAdjacentSegment(row, column - 1, BlockGenerator.SegmentFlag.BottomRight, segment, segmentsToPop); // Check left block's bottom-right
                        CheckAdjacentSegment(row + 1, column, BlockGenerator.SegmentFlag.TopLeft, segment, segmentsToPop); // Check bottom block's top-left
                        break;

                    case BlockGenerator.SegmentFlag.TopRight:
                        CheckAdjacentSegment(row, column + 1, BlockGenerator.SegmentFlag.TopLeft, segment, segmentsToPop); // Check right block's top-left
                        break;

                    case BlockGenerator.SegmentFlag.BottomRight:
                        CheckAdjacentSegment(row, column + 1, BlockGenerator.SegmentFlag.BottomLeft, segment, segmentsToPop); // Check right block's bottom-left
                        CheckAdjacentSegment(row + 1, column, BlockGenerator.SegmentFlag.TopRight, segment, segmentsToPop); // Check bottom block's top-right
                        break;
                }
            }
        }

        // Pop the collected segments
        PopSegments(block, segmentsToPop);
    }

    private void CheckAdjacentSegment(int row, int column, BlockGenerator.SegmentFlag requiredFlag, BlockGenerator.Segment currentSegment, List<BlockGenerator.Segment> segmentsToPop)
    {
        // Validate grid bounds
        if (row < 0 || column < 0 || row >= gridManager.logicalGridSize || column >= gridManager.logicalGridSize) return;

        // Get the adjacent block
        var adjacentBlock = gridManager.logicalGrid[row, column];
        if (adjacentBlock == null) return;

        // Check segments of the adjacent block
        foreach (var adjacentSegment in adjacentBlock.Segments)
        {
            if (adjacentSegment.Flags.Contains(requiredFlag) && adjacentSegment.Color == currentSegment.Color)
            {
                // Add to pop list if not already present
                if (!segmentsToPop.Contains(adjacentSegment))
                {
                    segmentsToPop.Add(adjacentSegment);
                }
                if (!segmentsToPop.Contains(currentSegment))
                {
                    segmentsToPop.Add(currentSegment);
                }
            }
        }
    }

    private void PopSegments(BlockGenerator.Block block, List<BlockGenerator.Segment> segmentsToPop)
    {
        foreach (var segment in segmentsToPop)
        {
            // Optionally, visually destroy or deactivate the segment
            Destroy(segment.SegmentObject);

            // Remove segment from the block's segment list
            block.Segments.Remove(segment);
        }

        // If the block has no remaining segments, clear it from the grid
        if (block.Segments.Count == 0)
        {
            RemoveBlockFromGrid(block);
        }
    }

    private void RemoveBlockFromGrid(BlockGenerator.Block block)
    {
        // Find the block's position in the grid
        for (int row = 0; row < gridManager.logicalGridSize; row++)
        {
            for (int column = 0; column < gridManager.logicalGridSize; column++)
            {
                if (gridManager.logicalGrid[row, column] == block)
                {
                    // Clear the block from the grid
                    gridManager.logicalGrid[row, column] = null;
                    break;
                }
            }
        }
        // Optionally, destroy the block GameObject
        Destroy(block.BlockObject);
    }
}
