using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Link
{
    public int begin;
    public int end;

    public int id
    {
        get { return Mathf.Min(begin, end) + (Mathf.Max(begin, end) << 4); }
    }
}

[System.Serializable]
public struct BoardDefinition
{
    public Link[] links;
    public Color nodeColor;
}
