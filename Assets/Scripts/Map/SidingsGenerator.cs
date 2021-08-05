using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class SidingsGenerator : MonoBehaviour {
    [Header("Scripts")]
    public MazeManager mazeManager;
    public TileGenerator tileGenerator;

    // variables from the maze manager
    private bool isInit = false;        // checking if the class has all of the required values
    private int borderWidth;
    private Vector2Int topRight;
    private Vector2Int bottomLeft;
    private Tilemap topSidingMap;
    private Tilemap leftSidingMap;
    private Tilemap rightSidingMap;

    // performing pre-use tasks
    public void Init() {
        borderWidth = mazeManager.GetBorderWidth();
        topRight = mazeManager.GetTopRight();
        bottomLeft = mazeManager.GetBottomLeft();

        isInit = true;
    }

    // this procedure will create the sidings for the walls
    public void GenerateSidingsMap(bool[,] path, Tilemap map) {
        if (isInit) {
            // iterating through each of the cell that possibly contain a paths
            for (int col = bottomLeft.x - borderWidth; col < topRight.x + borderWidth + 1; col++) {
                for (int row = bottomLeft.y - borderWidth; row < topRight.y + borderWidth + 1; row++) {
                    // there are no tiles at this location
                    if (!tileGenerator.HasTile(col, row, map)) {
                        GenerateSidings(col, row, map);
                    }
                }
            }
        }
    }

    // determining and placing a siding at a given location
    void GenerateSidings(int col, int row, Tilemap map) {
        if (isInit) {
            // determining which siding must used
            ///////   top siding   //////
            if (!tileGenerator.HasTile(col-1, row, map) && (tileGenerator.HasTile(col, row+1, map) || row+1>topRight.y+borderWidth) && !tileGenerator.HasTile(col+1, row, map) && col-1>=bottomLeft.x-borderWidth && col+1<=topRight.x+borderWidth) {
                mazeManager.topSidingMap.SetTile(new Vector3Int(col, row, 0), tileGenerator.ChooseWeightedTile(tileGenerator.topSidings));
            // checking for single-wide corners
            } else if (tileGenerator.HasTile(col-1, row, map) && tileGenerator.HasTile(col, row+1, map) && tileGenerator.HasTile(col+1, row, map)) {
                mazeManager.topSidingMap.SetTile(new Vector3Int(col, row, 0), tileGenerator.ChooseWeightedTile(tileGenerator.topSidingCorners));
            // other sidings
            } else {
                ///////   left siding    //////
                GenerateLeftSidings(col, row, map);

                ///////  right siding  //////
                GenerateRightSidings(col, row, map);
            }
        }
    }

    // determining and placing the sidings that hug the left side of a cell
    void GenerateLeftSidings(int col, int row, Tilemap map) {
        // sharp corners
        if ((tileGenerator.HasTile(col-1, row, map) || col-1<bottomLeft.x-borderWidth) && (tileGenerator.HasTile(col, row+1, map) || row+1>topRight.y+borderWidth)) {
            mazeManager.leftSidingMap.SetTile(new Vector3Int(col, row, 0), tileGenerator.ChooseWeightedTile(tileGenerator.leftSidingsSharp));
        // standard
        } else if ((tileGenerator.HasTile(col-1, row, map) || col-1<bottomLeft.x-borderWidth) && !tileGenerator.HasTile(col, row+1, map) && (tileGenerator.HasTile(col-1, row+1, map) || col-1<bottomLeft.x-borderWidth)) {
            mazeManager.leftSidingMap.SetTile(new Vector3Int(col, row, 0), tileGenerator.ChooseWeightedTile(tileGenerator.leftSidings));
        // soft corners
        } else if (tileGenerator.HasTile(col-1, row, map) && !tileGenerator.HasTile(col-1, row+1, map)) {
            mazeManager.leftSidingMap.SetTile(new Vector3Int(col, row, 0), tileGenerator.ChooseWeightedTile(tileGenerator.leftSidingsSoft));
        // single pixel corners
        } else if (!tileGenerator.HasTile(col-1, row, map) && tileGenerator.HasTile(col-1, row+1, map) && !tileGenerator.HasTile(col, row+1, map)) {
            mazeManager.leftSidingMap.SetTile(new Vector3Int(col, row, 0), tileGenerator.ChooseWeightedTile(tileGenerator.leftSidingCorners));
        } 
    }

    // determining and placing the sidings that hug the right side of a cell
    void GenerateRightSidings(int col, int row, Tilemap map) {
        // sharp corners
        if ((tileGenerator.HasTile(col+1, row, map) || col+1>topRight.x+borderWidth) && (tileGenerator.HasTile(col, row+1, map) || row+1>topRight.y+borderWidth)) {
            mazeManager.rightSidingMap.SetTile(new Vector3Int(col, row, 0), tileGenerator.ChooseWeightedTile(tileGenerator.rightSidingsSharp));
        } else if ((tileGenerator.HasTile(col+1, row, map) || col+1>topRight.x+borderWidth) && !tileGenerator.HasTile(col, row+1, map) && (tileGenerator.HasTile(col+1, row+1, map) || col+1>topRight.x+borderWidth)) {
            mazeManager.rightSidingMap.SetTile(new Vector3Int(col, row, 0), tileGenerator.ChooseWeightedTile(tileGenerator.rightSidings));
        // soft corners
        } else if (tileGenerator.HasTile(col+1, row, map) && !tileGenerator.HasTile(col+1, row+1, map)) {
            mazeManager.rightSidingMap.SetTile(new Vector3Int(col, row, 0), tileGenerator.ChooseWeightedTile(tileGenerator.rightSidingsSoft));
        // single pixel corners
        } else if (!tileGenerator.HasTile(col+1, row, map) && tileGenerator.HasTile(col+1, row+1, map) && !tileGenerator.HasTile(col, row+1, map)) {
            mazeManager.rightSidingMap.SetTile(new Vector3Int(col, row, 0), tileGenerator.ChooseWeightedTile(tileGenerator.rightSidingCorners));
        }
    }
}
