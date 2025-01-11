using System;
using System.Linq;
using UnityEngine;

public class SegmentDebug : MonoBehaviour
{
    [Header("Segment Properties")]
    public BlockGenerator.SegmentFlag[] Flags;
    public Color SegmentColor;

    private void OnValidate()
    {
        // Automatically update the renderer's color in the editor
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = SegmentColor;
        }
    }

    public void Initialize(BlockGenerator.SegmentFlag[] flags, Color color)
    {
        Flags = flags;
        SegmentColor = color;

        // Update renderer's color when initialized
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = SegmentColor;
        }
    }
}