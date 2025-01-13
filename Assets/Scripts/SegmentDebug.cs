using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SegmentDebug : MonoBehaviour
{
    [FormerlySerializedAs("Flags")] [Header("Segment Properties")]
    public List<BlockGenerator.SegmentFlag> flags;
    [FormerlySerializedAs("SegmentColor")] public Color segmentColor;

    private void OnValidate()
    {
        Renderer component = GetComponent<Renderer>();
        if (component != null)
        {
            component.material.color = segmentColor;
        }
    }

    public void Initialize(List<BlockGenerator.SegmentFlag> colorFlags, Color color)
    {
        this.flags = colorFlags;
        segmentColor = color;

        Renderer component = GetComponent<Renderer>();
        if (component != null)
        {
            component.material.color = segmentColor;
        }
    }
}