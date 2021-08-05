using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class WallGenerator : MonoBehaviour {
    [Header("Scripts")]
    public MazeManager mazeManager;
    public TileGenerator tileGenerator;

    // variables from the maze manager
    private bool isInit = false;        // checking if the class has all of the required values
    private int borderWidth;
    private Vector2Int mazeSize;
    private Vector2Int topRight;
    private Vector2Int bottomLeft;
    private List<List<Cell>> exitLocations;     // stores a top right and bottom left corner, indicating the location and dimensions of an exit

    // performing pre-use tasks
    public void Init() {
        borderWidth = mazeManager.GetBorderWidth();
        mazeSize = mazeManager.GetMazeSize();
        topRight = mazeManager.GetTopRight();
        bottomLeft = mazeManager.GetBottomLeft();
        exitLocations = mazeManager.GetExitLocations();

        isInit = true;
    }

    // draws walls based on the locations of the paths
    public void GenerateWallMap(bool[,] path, Tilemap groundMap, Tilemap wallMap) {
        if (isInit) {
            // iterating through each of the cell that possibly contain a paths
            for (int col = 0; col < path.GetLength(0); col++) {
                for (int row = 0; row < path.GetLength(1) - 2; row++) {
                    if (path[col, row]) {
                        // checking if there is enough space to place a 2 tile high wall
                        if (path[col, row] && path[col, row+1] && !path[col, row+2]) {
                            // drawing each of the walls
                            GenerateWalls(col, row, path, groundMap, wallMap);                                                    
                        } 
                    }
                }
            }

            // drawing walls on the border of the maze
            // bottom
            for (int col = bottomLeft.x - borderWidth; col < topRight.x + borderWidth + 1; col++) {
                GenerateWalls(col, bottomLeft.y - borderWidth - 2, path, groundMap, wallMap);
            }

            // each of the exits
            foreach (List<Cell> exitCorner in exitLocations) {
                // checking if the segment is to the left or right of the map
                if (exitCorner[0].x < bottomLeft.x || exitCorner[1].x < bottomLeft.x || exitCorner[0].x > topRight.x || exitCorner[1].x > topRight.x) {
                    for (int col = Mathf.Min(exitCorner[0].x, exitCorner[1].x); col < Mathf.Max(exitCorner[0].x, exitCorner[1].x) + 1; col++)
                        // the cell is not within the playable map area
                        if (col > topRight.x || col < bottomLeft.x) GenerateWalls(col, Mathf.Max(exitCorner[0].y, exitCorner[1].y)-1, path, groundMap, wallMap);
                }
            }
        }
    }

    // determing and placing a wall, given the cell location (col and row)
    void GenerateWalls(int col, int row, bool[,] path, Tilemap groundMap, Tilemap wallMap) {
        if (isInit) {
            // checking if there is a maze exit
            if ((row+2 > topRight.y || row+2 < bottomLeft.y) && groundMap.HasTile(new Vector3Int(col, row+2, 0))) return;

            // checking for specific tiles, around the current tile (the tiles must be within the maze area)
            // wall for a 1-wide section
            if (mazeManager.IsPath(col-1, row+2, path) && !mazeManager.IsPath(col, row+2, path) && mazeManager.IsPath(col+1, row+2, path)) {
                tileGenerator.PlaceWallTiles(col, row, tileGenerator.wallSingleBottoms, tileGenerator.wallSingleTops, tileGenerator.grasses, wallMap, groundMap);   
            }

            // front left walls             
            else if (mazeManager.IsPath(col-1, row+2, path) || groundMap.HasTile(new Vector3Int(col-1, row+2, 0)) || col == bottomLeft.x - borderWidth) {
                tileGenerator.PlaceWallTiles(col, row, tileGenerator.wallFrontLeftBottoms, tileGenerator.wallFrontLeftTops, tileGenerator.grasses, wallMap, groundMap);   
            }        

            // front right wall          
            else if (mazeManager.IsPath(col+1, row+2, path) || groundMap.HasTile(new Vector3Int(col+1, row+2, 0)) || col == topRight.x + borderWidth) {
                tileGenerator.PlaceWallTiles(col, row, tileGenerator.wallFrontRightBottoms, tileGenerator.wallFrontRightTops, tileGenerator.grasses, wallMap, groundMap);  
            }  
            
            // standard wall
            else tileGenerator.PlaceWallTiles(col, row, tileGenerator.wallBottoms, tileGenerator.wallTops, tileGenerator.grasses, wallMap, groundMap);  
        }
    }
}
