using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockGenerator : MonoBehaviour
{
    public GameObject segmentPrefab;
    public Transform blockParent;
    public GridManager gridManager;

    public Color[] allColors;

    private GameObject _currentBlockObject;
    private Block _currentBlockData;

    public enum SegmentFlag
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    public class Segment
    {
        public List<SegmentFlag> flags;
        public Color color;
        public GameObject segmentObject;

        public Segment(List<SegmentFlag> flags, Color color)
        {
            this.flags = flags;
            this.color = color;
        }
    }

    public class Block
    {
        public List<Segment> segments = new List<Segment>();
        public GameObject blockObject;
    }

    public void SpawnNewBlock()
    {
        _currentBlockData = GenerateBlock();

        _currentBlockObject = InstantiateBlock(_currentBlockData);

        _currentBlockObject.transform.position = new Vector3(0, gridManager.GetGridTopY() + 1, 0);
    }
    
    public Block GenerateBlock()
    {
        Block block = new Block();
        int segmentCount = Random.Range(1, 5);
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
        block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.TopLeft, SegmentFlag.TopRight, SegmentFlag.BottomLeft, SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
    }

    void CreateTwoSegments(Block block, List<Color> allowedColors)
    {
        bool isHorizontal = Random.value > 0.5f;
        if (isHorizontal)
        {
            block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.TopLeft, SegmentFlag.TopRight }, GetRandomColor(allowedColors)));
            block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.BottomLeft, SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
        }
        else
        {
            block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.TopLeft, SegmentFlag.BottomLeft }, GetRandomColor(allowedColors)));
            block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.TopRight, SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
        }
    }

    void CreateThreeSegments(Block block, List<Color> allowedColors)
    {
        bool isHorizontal = Random.value > 0.5f; 
        if (isHorizontal)
        {
            block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.TopLeft, SegmentFlag.TopRight }, GetRandomColor(allowedColors)));

            bool splitBottom = Random.value > 0.5f;
            if (splitBottom)
            {
                block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.BottomLeft }, GetRandomColor(allowedColors)));
                block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
            }
            else
            {
                block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.BottomLeft, SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
            }
        }
        else
        {
            block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.TopLeft, SegmentFlag.BottomLeft }, GetRandomColor(allowedColors)));

            bool splitRight = Random.value > 0.5f;
            if (splitRight)
            {
                block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.TopRight }, GetRandomColor(allowedColors)));
                block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
            }
            else
            {
                block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.TopRight, SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
            }
        }
    }

    void CreateFourSegments(Block block, List<Color> allowedColors)
    {
        block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.TopLeft }, GetRandomColor(allowedColors)));
        block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.TopRight }, GetRandomColor(allowedColors)));
        block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.BottomLeft }, GetRandomColor(allowedColors)));
        block.segments.Add(new Segment(new List<SegmentFlag>() { SegmentFlag.BottomRight }, GetRandomColor(allowedColors)));
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
        block.blockObject = blockObject;
        if (blockParent != null)
        {
            blockObject.transform.SetParent(blockParent);
        }

        foreach (var segment in block.segments)
        {
            GameObject segmentObject = Instantiate(segmentPrefab, blockObject.transform);
            segment.segmentObject = segmentObject;
            segmentObject.name = "Segment";

            Vector3 positionOffset = GetPositionOffset(segment.flags);
            segmentObject.transform.localPosition = positionOffset;

            Vector3 segmentScale = GetScaleForFlags(segment.flags);
            segmentObject.transform.localScale = segmentScale;

            segmentObject.GetComponent<SpriteRenderer>().color = segment.color;
            
            SegmentDebug debugComponent = segmentObject.AddComponent<SegmentDebug>();
            debugComponent.Initialize(segment.flags, segment.color);
        }

        return blockObject;
    }

    Vector3 GetPositionOffset(List<SegmentFlag> flags)
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
        return offset / flags.Count;
    }

    Vector3 GetScaleForFlags(List<SegmentFlag> flags)
    {
        float scaleX = 1f;
        float scaleY = 1f;

        if (flags.Count == 1)
        {
            scaleX = 0.5f;
            scaleY = 0.5f;
        }
        else if (flags.Count == 2)
        {
            if (flags.Contains(SegmentFlag.TopLeft) && flags.Contains(SegmentFlag.TopRight) ||
                flags.Contains(SegmentFlag.BottomLeft) && flags.Contains(SegmentFlag.BottomRight))
            {
                scaleX = 1f; 
                scaleY = 0.5f; 
            }
            else if (flags.Contains(SegmentFlag.TopLeft) && flags.Contains(SegmentFlag.BottomLeft) ||
                     flags.Contains(SegmentFlag.TopRight) && flags.Contains(SegmentFlag.BottomRight))
            {
                scaleX = 0.5f; 
                scaleY = 1f; 
            }
        }
        else if (flags.Count == 4)
        {
            scaleX = 1f;
            scaleY = 1f; 
        }
        return new Vector3(scaleX, scaleY, 1f); 
    }
}
