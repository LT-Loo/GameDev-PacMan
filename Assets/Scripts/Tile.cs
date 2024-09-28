using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int Type {get; private set;}
    public Vector3 Position {get; private set;}
    public Quaternion Rotation {get; private set;}
    public int Row {get; private set;}
    public int Col {get; private set;}
    
    public Tile(int type, Vector3 pos, Quaternion rot, int r, int c) {
        Type = type;
        Position = pos;
        Rotation = rot;
        Row = r;
        Col = c;
    }
}
