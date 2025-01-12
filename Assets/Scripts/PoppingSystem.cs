using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoppingSystem : MonoBehaviour
{
    public GridManager gridManager;

    public void CheckAndPopSegments(BlockGenerator.Block block, int row, int column)
    {
        if (block == null) return;
        List<BlockGenerator.Segment> segmentsToPop = new List<BlockGenerator.Segment>();
        List<BlockGenerator.Block> blocksToCheckForRemoval = new List<BlockGenerator.Block>();
        
        foreach (var segment in block.Segments)
        {
            foreach (var flag in segment.Flags)
            {
                switch (flag)
                {
                    case BlockGenerator.SegmentFlag.TopLeft:
                        CheckAdjacentSegment(row, column - 1, BlockGenerator.SegmentFlag.TopRight, segment, segmentsToPop,blocksToCheckForRemoval);
                        break;

                    case BlockGenerator.SegmentFlag.BottomLeft:
                        CheckAdjacentSegment(row, column - 1, BlockGenerator.SegmentFlag.BottomRight, segment, segmentsToPop,blocksToCheckForRemoval); 
                        CheckAdjacentSegment(row + 1, column, BlockGenerator.SegmentFlag.TopLeft, segment, segmentsToPop,blocksToCheckForRemoval); 
                        break;

                    case BlockGenerator.SegmentFlag.TopRight:
                        CheckAdjacentSegment(row, column + 1, BlockGenerator.SegmentFlag.TopLeft, segment, segmentsToPop,blocksToCheckForRemoval); 
                        break;

                    case BlockGenerator.SegmentFlag.BottomRight:
                        CheckAdjacentSegment(row, column + 1, BlockGenerator.SegmentFlag.BottomLeft, segment, segmentsToPop,blocksToCheckForRemoval); 
                        CheckAdjacentSegment(row + 1, column, BlockGenerator.SegmentFlag.TopRight, segment, segmentsToPop,blocksToCheckForRemoval); 
                        break;
                }
            }
        }
        PopSegments(block, segmentsToPop,blocksToCheckForRemoval);
    }

    private void CheckAdjacentSegment(int row, int column, BlockGenerator.SegmentFlag requiredFlag, BlockGenerator.Segment currentSegment, List<BlockGenerator.Segment> segmentsToPop, List<BlockGenerator.Block> blocksToCheck)
    {
        if (row < 0 || column < 0 || row >= gridManager.logicalGridSize || column >= gridManager.logicalGridSize) return;
        var adjacentBlock = gridManager.logicalGrid[row, column];
        if (adjacentBlock == null) return;

        foreach (var adjacentSegment in adjacentBlock.Segments)
        {
            if (adjacentSegment.Flags.Contains(requiredFlag) && adjacentSegment.Color == currentSegment.Color)
            {
                if (!blocksToCheck.Contains(adjacentBlock))
                {
                    blocksToCheck.Add(adjacentBlock);
                }
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

    private void PopSegments(BlockGenerator.Block block, List<BlockGenerator.Segment> segmentsToPop, List<BlockGenerator.Block> blocksToCheckForRemoval)
    {
        foreach (var segment in segmentsToPop)
        {
            Destroy(segment.SegmentObject);
            foreach (var blockToCheck in blocksToCheckForRemoval)
            {
                blockToCheck.Segments.Remove(segment);
            }
            block.Segments.Remove(segment);
        }

        foreach (var blockToCheck in blocksToCheckForRemoval)
        {
            if (blockToCheck.Segments.Count == 0)
            {
                RemoveBlockFromGrid(blockToCheck);
            }
        }

        if (block.Segments.Count == 0)
        {
            RemoveBlockFromGrid(block);
        }
        else
        {
            ExpandBlock(block); 
        }
    }

    private void RemoveBlockFromGrid(BlockGenerator.Block block)
    {
        int removedRow = -1;
        int removedColumn = -1;

        for (int row = 0; row < gridManager.logicalGridSize; row++)
        {
            for (int column = 0; column < gridManager.logicalGridSize; column++)
            {
                if (gridManager.logicalGrid[row, column] == block)
                {
                    gridManager.logicalGrid[row, column] = null;
                    removedRow = row;
                    removedColumn = column;
                    break;
                }
            }
        }

        Destroy(block.BlockObject);

        if (removedRow != -1 && removedColumn != -1)
        {
            gridManager.UpdateGridAfterRemoval(removedColumn);
        }
    }
    private void ExpandBlock(BlockGenerator.Block block)
    {
        var expansionMap = new Dictionary<BlockGenerator.SegmentFlag, BlockGenerator.SegmentFlag[]>
        {
            { BlockGenerator.SegmentFlag.TopLeft, new[] { BlockGenerator.SegmentFlag.BottomLeft, BlockGenerator.SegmentFlag.TopRight } },
            { BlockGenerator.SegmentFlag.TopRight, new[] { BlockGenerator.SegmentFlag.BottomRight, BlockGenerator.SegmentFlag.TopLeft } },
            { BlockGenerator.SegmentFlag.BottomLeft, new[] { BlockGenerator.SegmentFlag.TopLeft, BlockGenerator.SegmentFlag.BottomRight } },
            { BlockGenerator.SegmentFlag.BottomRight, new[] { BlockGenerator.SegmentFlag.TopRight, BlockGenerator.SegmentFlag.BottomLeft } }
        };

        // Determine missing flags in the block
        var allFlags = new HashSet<BlockGenerator.SegmentFlag> 
        { 
            BlockGenerator.SegmentFlag.TopLeft, BlockGenerator.SegmentFlag.TopRight, 
            BlockGenerator.SegmentFlag.BottomLeft, BlockGenerator.SegmentFlag.BottomRight 
        };
        var existingFlags = block.Segments.SelectMany(segment => segment.Flags).ToHashSet();
        var missingFlags = allFlags.Except(existingFlags).ToList();

        foreach (var missingFlag in missingFlags)
        {
            foreach (var segment in block.Segments)
            {
                if (segment.Flags.Any(flag => expansionMap[missingFlag].Contains(flag)))
                {
                    segment.Flags.Add(missingFlag);

                    UpdateSegmentVisual(segment, missingFlag);
                    break;
                }
            }
        }
    }
    private void UpdateSegmentVisual(BlockGenerator.Segment segment, BlockGenerator.SegmentFlag newFlag)
    {
        var segmentObject = segment.SegmentObject;
        var renderer = segmentObject.GetComponent<Renderer>();
        var originalSize = renderer.bounds.size;

        switch (newFlag)
        {
            case BlockGenerator.SegmentFlag.TopLeft:
                segmentObject.transform.localPosition += new Vector3(-originalSize.x / 2, originalSize.y / 2, 0);
                break;
            case BlockGenerator.SegmentFlag.TopRight:
                segmentObject.transform.localPosition += new Vector3(originalSize.x / 2, originalSize.y / 2, 0);
                break;
            case BlockGenerator.SegmentFlag.BottomLeft:
                segmentObject.transform.localPosition += new Vector3(-originalSize.x / 2, -originalSize.y / 2, 0);
                break;
            case BlockGenerator.SegmentFlag.BottomRight:
                segmentObject.transform.localPosition += new Vector3(originalSize.x / 2, -originalSize.y / 2, 0);
                break;
        }
        
        renderer.transform.localScale *= 1.1f; 
    }


}
