using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockGenerator : MonoBehaviour
{
    public GameObject segmentPrefab; // Assign a basic prefab for segments in the inspector
    public Transform blockParent;   // Assign a parent transform for better hierarchy organization
    public GridManager gridManager;

    public Color[] allColors = { Color.red, Color.blue, Color.yellow, Color.green };

    private GameObject currentBlockObject; // The block currently being moved
    private Block currentBlockData;        // Data representation of the current block

    public enum SegmentFlag
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class Segment
    {
        public SegmentFlag[] Flags;
        public Color Color;
        public GameObject SegmentObject;

        public Segment(SegmentFlag[] flags, Color color)
        {
            Flags = flags;
            Color = color;
        }
    }

    public class Block
    {
        public List<Segment> Segments = new List<Segment>();
        public GameObject BlockObject;
    }

    public void SpawnNewBlock()
    {
        // Generate a new block
        currentBlockData = GenerateBlock(gridManager);

        // Instantiate the block visually
        currentBlockObject = InstantiateBlock(currentBlockData);

        // Position the block above the grid and allow it to follow the mouse
        currentBlockObject.transform.position = new Vector3(0, gridManager.GetGridTopY() + 1, 0);
    }

    public Block GenerateBlock(GridManager gridManager)
    {
        Block block = new Block();

        int segmentCount = Random.Range(1, 5); // Randomize 1 to 4 segments
        List<Color> allowedColors = new List<Color>(allColors);

        switch (segmentCount)
        {
            case 1:
                CreateSingleSegment(block, allowedColors);
                break;
            case 2:
                CreateTwoSegments(block, allowedColors);
                break;
            case 3:
                CreateThreeSegments(block, allowedColors);
                break;
            case 4:
                CreateFourSegments(block, allowedColors);
                break;
        }

        return block;
    }

    void CreateSingleSegment(Block block, List<Color> allowedColors)
    {
        block.Segments.Add(new Segment(new[] { SegmentFlag.TopLeft, SegmentFlag.TopRight, SegmentFlag.BottomLeft, SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
    }

    void CreateTwoSegments(Block block, List<Color> allowedColors)
    {
        bool isHorizontal = Random.value > 0.5f; // Horizontal or vertical split
        if (isHorizontal)
        {
            block.Segments.Add(new Segment(new[] { SegmentFlag.TopLeft, SegmentFlag.TopRight }, GetRandomColor(allowedColors)));
            block.Segments.Add(new Segment(new[] { SegmentFlag.BottomLeft, SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
        }
        else
        {
            block.Segments.Add(new Segment(new[] { SegmentFlag.TopLeft, SegmentFlag.BottomLeft }, GetRandomColor(allowedColors)));
            block.Segments.Add(new Segment(new[] { SegmentFlag.TopRight, SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
        }
    }

    void CreateThreeSegments(Block block, List<Color> allowedColors)
    {
        bool isHorizontal = Random.value > 0.5f; // Horizontal or vertical split
        if (isHorizontal)
        {
            block.Segments.Add(new Segment(new[] { SegmentFlag.TopLeft, SegmentFlag.TopRight }, GetRandomColor(allowedColors)));

            bool splitBottom = Random.value > 0.5f;
            if (splitBottom)
            {
                block.Segments.Add(new Segment(new[] { SegmentFlag.BottomLeft }, GetRandomColor(allowedColors)));
                block.Segments.Add(new Segment(new[] { SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
            }
            else
            {
                block.Segments.Add(new Segment(new[] { SegmentFlag.BottomLeft, SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
            }
        }
        else
        {
            block.Segments.Add(new Segment(new[] { SegmentFlag.TopLeft, SegmentFlag.BottomLeft }, GetRandomColor(allowedColors)));

            bool splitRight = Random.value > 0.5f;
            if (splitRight)
            {
                block.Segments.Add(new Segment(new[] { SegmentFlag.TopRight }, GetRandomColor(allowedColors)));
                block.Segments.Add(new Segment(new[] { SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
            }
            else
            {
                block.Segments.Add(new Segment(new[] { SegmentFlag.TopRight, SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
            }
        }
    }

    void CreateFourSegments(Block block, List<Color> allowedColors)
    {
        block.Segments.Add(new Segment(new[] { SegmentFlag.TopLeft }, GetRandomColor(allowedColors)));
        block.Segments.Add(new Segment(new[] { SegmentFlag.TopRight }, GetRandomColor(allowedColors)));
        block.Segments.Add(new Segment(new[] { SegmentFlag.BottomLeft }, GetRandomColor(allowedColors)));
        block.Segments.Add(new Segment(new[] { SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
    }

    Color GetRandomColor(List<Color> allowedColors)
    {
        int index = Random.Range(0, allowedColors.Count);
        Color chosenColor = allowedColors[index];
        allowedColors.RemoveAt(index);
        return chosenColor;
    }

    public GameObject InstantiateBlock(Block block)
    {
        GameObject blockObject = new GameObject("Block");
        block.BlockObject = blockObject;
        if (blockParent != null)
        {
            blockObject.transform.SetParent(blockParent);
        }

        foreach (var segment in block.Segments)
        {
            GameObject segmentObject = Instantiate(segmentPrefab, blockObject.transform);
            segment.SegmentObject = segmentObject;
            segmentObject.name = "Segment";

            // Position the segment based on its flags
            Vector3 positionOffset = GetPositionOffset(segment.Flags);
            segmentObject.transform.localPosition = positionOffset;

            // Scale the segment based on the number of flags it occupies
            Vector3 segmentScale = GetScaleForFlags(segment.Flags);
            segmentObject.transform.localScale = segmentScale;

            // Apply the segment's color
            segmentObject.GetComponent<SpriteRenderer>().color = segment.Color;
        }

        return blockObject;
    }

    Vector3 GetPositionOffset(SegmentFlag[] flags)
    {
        Vector3 offset = Vector3.zero;
        foreach (var flag in flags)
        {
            switch (flag)
            {
                case SegmentFlag.TopLeft:
                    offset += new Vector3(-0.25f, 0.25f, 0f);
                    break;
                case SegmentFlag.TopRight:
                    offset += new Vector3(0.25f, 0.25f, 0f);
                    break;
                case SegmentFlag.BottomLeft:
                    offset += new Vector3(-0.25f, -0.25f, 0f);
                    break;
                case SegmentFlag.BottomRight:
                    offset += new Vector3(0.25f, -0.25f, 0f);
                    break;
            }
        }

        return offset / flags.Length; // Average the positions for the center
    }

    Vector3 GetScaleForFlags(SegmentFlag[] flags)
    {
        float scaleX = 1f;
        float scaleY = 1f;

        // Adjust scale based on flags
        if (flags.Length == 1)
        {
            scaleX = 0.5f;
            scaleY = 0.5f;
        }
        else if (flags.Length == 2)
        {
            if (flags.Contains(SegmentFlag.TopLeft) && flags.Contains(SegmentFlag.TopRight) ||
                flags.Contains(SegmentFlag.BottomLeft) && flags.Contains(SegmentFlag.BottomRight))
            {
                scaleX = 1f; // Horizontal stretch
                scaleY = 0.5f; // Vertical narrow
            }
            else if (flags.Contains(SegmentFlag.TopLeft) && flags.Contains(SegmentFlag.BottomLeft) ||
                     flags.Contains(SegmentFlag.TopRight) && flags.Contains(SegmentFlag.BottomRight))
            {
                scaleX = 0.5f; // Horizontal narrow
                scaleY = 1f; // Vertical stretch
            }
        }
        else if (flags.Length == 4)
        {
            scaleX = 1f;
            scaleY = 1f; // Full block size
        }

        return new Vector3(scaleX, scaleY, 1f); // Keep Y scale at 1 for now
    }
    

}
