using System.Collections.Generic;
using UnityEngine;

public class SegmentDebug : MonoBehaviour
{
    [Header("Segment Properties")]
    public List<BlockGenerator.SegmentFlag> Flags;
    public Color SegmentColor;

    private void OnValidate()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = SegmentColor;
        }
    }

    public void Initialize(List<BlockGenerator.SegmentFlag> flags, Color color)
    {
        Flags = flags;
        SegmentColor = color;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = SegmentColor;
        }
    }
}