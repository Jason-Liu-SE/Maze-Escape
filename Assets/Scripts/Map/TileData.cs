using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData {
    public int x;
    public int y;
    public bool isPath;

    // constructor
    public TileData() {
        this.x = 0;
        this.y = 0;
        this.isPath = false;
    }

    // constructor with optionary parameters
    public TileData(int X, int Y, bool IsPath=false) {
        this.x = X;
        this.y = Y;
        this.isPath = IsPath;
    }
}
