using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoppingSystem : MonoBehaviour
{
    public GridManager gridManager;

    public void CheckAndPopSegments(int row, int column)
    {
        if (row < 0 || column < 0 || row >= gridManager.logicalGridSize || column >= gridManager.logicalGridSize) return;
        var block = gridManager.logicalGrid[row, column];
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
        if (segmentsToPop.Count > 0)
        {
            CheckAndPopSegments(row,column-1);
            CheckAndPopSegments(row -1, column);
            CheckAndPopSegments(row+1,column);
        }
        
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
        foreach (var adjacentBlock in blocksToCheckForRemoval)
        {
            if (adjacentBlock.Segments.Count > 0) 
            {
                ExpandBlock(adjacentBlock);
            }
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
                if (CanAddFlag(segment, missingFlag, expansionMap))
                {
                    segment.Flags.Add(missingFlag);
                    break;
                }
            }
        }

        if (block.Segments.Count == 1)
        {
            var remainingSegment = block.Segments.First();
            remainingSegment.Flags = allFlags.ToList();
        }

        foreach (var segment in block.Segments)
        {
            UpdateSegmentVisual(segment);
        }
    }

    private bool CanAddFlag(BlockGenerator.Segment segment, BlockGenerator.SegmentFlag newFlag, Dictionary<BlockGenerator.SegmentFlag, BlockGenerator.SegmentFlag[]> expansionMap)
    {
        if (segment.Flags.Count == 2 && !segment.Flags.Contains(newFlag)) return false;

        if (segment.Flags.Contains(BlockGenerator.SegmentFlag.TopLeft) && newFlag == BlockGenerator.SegmentFlag.BottomRight) return false;
        if (segment.Flags.Contains(BlockGenerator.SegmentFlag.BottomRight) && newFlag == BlockGenerator.SegmentFlag.TopLeft) return false;
        if (segment.Flags.Contains(BlockGenerator.SegmentFlag.TopRight) && newFlag == BlockGenerator.SegmentFlag.BottomLeft) return false;
        if (segment.Flags.Contains(BlockGenerator.SegmentFlag.BottomLeft) && newFlag == BlockGenerator.SegmentFlag.TopRight) return false;

        return segment.Flags.Any(flag => expansionMap[newFlag].Contains(flag));
    }

    private void UpdateSegmentVisual(BlockGenerator.Segment segment)
    {
        float minX = 0, maxX = 0;
        float minY = 0, maxY = 0;

        foreach (var flag in segment.Flags)
        {
            switch (flag)
            {
                case BlockGenerator.SegmentFlag.TopLeft:
                    minX = Math.Min(minX, -0.5f);
                    maxY = Math.Max(maxY, 0.5f);
                    break;
                case BlockGenerator.SegmentFlag.TopRight:
                    maxX = Math.Max(maxX, 0.5f);
                    maxY = Math.Max(maxY, 0.5f);
                    break;
                case BlockGenerator.SegmentFlag.BottomLeft:
                    minX = Math.Min(minX, -0.5f);
                    minY = Math.Min(minY, -0.5f);
                    break;
                case BlockGenerator.SegmentFlag.BottomRight:
                    maxX = Math.Max(maxX, 0.5f);
                    minY = Math.Min(minY, -0.5f);
                    break;
            }
        }

        if (minX > maxX) maxX = minX + 0.1f; 
        if (minY > maxY) maxY = minY + 0.1f; 
        
        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;
        float scaleX = Math.Max(maxX - minX, 0.1f); 
        float scaleY = Math.Max(maxY - minY, 0.1f);

        var segmentObject = segment.SegmentObject;
        segmentObject.transform.localPosition = new Vector3(centerX, centerY, 0);
        segmentObject.transform.localScale = new Vector3(scaleX, scaleY, 1);
    }



}
