using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class ColliderGenerator : MonoBehaviour {
    // inspector fields
    [Header("Scripts")]
    public MazeManager mazeManager;
    public TileGenerator tileGenerator;

    // variables from the maze manager
    private bool isInit = false;        // checking if the class has all of the required values
    private int borderWidth;
    private Vector2Int mazeSize;
    private Vector2Int pathWidth;
    private Vector2Int wallWidth;
    private Vector2Int topRight;
    private Vector2Int bottomLeft;
    private int[] exitsOnEachSide = new int[4];

    // objects
    new private BoxCollider2D collider;

    // storing the variables needed for each of the functions to work correctly
    public void Init() {
        borderWidth = mazeManager.GetBorderWidth();
        mazeSize = mazeManager.GetMazeSize();
        pathWidth = mazeManager.GetPathWidth();
        wallWidth = mazeManager.GetWallWidth();
        topRight = mazeManager.GetTopRight();
        bottomLeft = mazeManager.GetBottomLeft();
        exitsOnEachSide = mazeManager.GetExitsOnEachSide();

        isInit = true;
    }

    // Generating the box colliders
    public void GenerateBoxColliders(bool[,] path, List<Node> nodes, GameObject colliders, Tilemap groundMap) {
        if (isInit) {
            int col = 0;
            int row = 0;
            bool[,] vScanned = new bool[path.GetLength(0),path.GetLength(1)];   // vertically scanned cells        
            bool[,] hScanned = new bool[path.GetLength(0),path.GetLength(1)];   // horizontally scanned cells

            // iterating through each of the cells in the maze
            foreach (Node node in nodes) {
                col = (int)node.x;
                row = (int)node.y;

                // checking if the cell has previously been scanned
                if (mazeManager.IsInBounds(col-1, row-1, maxx:mazeSize.x+pathWidth.x, maxy:mazeSize.y+pathWidth.y)) {
                    CreateVerticalBoxCollider(col-1, row-1, vScanned, path, colliders);
                    CreateHorizontalBoxCollider(col-1, row-1, hScanned, path, colliders);
                }
            }

            // creating the box colliders around the maze
            CreateBorderBoxColliders(colliders, groundMap);

            // creating a box collider, indicating the end of the maze
            CreateFinishCollider(colliders);
        }
    }

    // determining the offset and size of box colliders
    // this function and the following one assumes that the colliders are within the bounds of the maze
    void CreateVerticalBoxCollider(int col, int row, bool[,] scanned, bool[,] path, GameObject colliders) {
        int miny = row;
        int maxy = row;
        Vector2 offset = new Vector2(0f, 0f);

        if (!mazeManager.IsPath(col, row, path) && !scanned[col, row] && col > bottomLeft.x && col < topRight.x) {
            // determining the max y and min y values
            while (!mazeManager.IsPath(col, maxy+1, path) && mazeManager.IsInBounds(col, maxy+1, maxx:mazeSize.x+pathWidth.x, maxy:mazeSize.y+pathWidth.y)) maxy++;     // north
            while (!mazeManager.IsPath(col, miny-1, path) && mazeManager.IsInBounds(col, miny-1, maxx:mazeSize.x+pathWidth.x, maxy:mazeSize.y+pathWidth.y)) miny--;     // south

            // creating the vertical collider
            // the max or min y value must be outside of maze bounds or; the wall must have a path on the NSEW sides and must be in bounds
            if ((maxy >= row+pathWidth.y-1 || miny <= row-(pathWidth.y-1)) || (mazeManager.IsPath(col, maxy+1, path) && mazeManager.IsPath(col, miny-1, path) && mazeManager.IsPath(col+1, row, path) && mazeManager.IsPath(col-pathWidth.x, row, path)) && (col-wallWidth.x+1 > bottomLeft.x && col < topRight.x)) {
                if (maxy > topRight.y) maxy = topRight.y;
                if (miny < bottomLeft.y) miny = bottomLeft.y;

                // checking if the size of the collider is large enough
                if (wallWidth.x < 0.0002 || maxy-miny+1 < 0.0002) return;

                // creating box collider with initial values
                collider = colliders.AddComponent<BoxCollider2D>();
                offset.x = col+2-wallWidth.x+Mathf.Floor(wallWidth.x/2);
                offset.y = Mathf.Floor((maxy+miny)/2)+1f;
                collider.size = new Vector2(wallWidth.x, maxy-miny+1);

                // accounting for floating point centres
                if (wallWidth.x % 2 == 0) offset.x -= 0.5f;
                if ((maxy+miny) % 2 != 0) offset.y += 0.5f;

                collider.offset = new Vector2(offset.x, offset.y);

                // indicating which cells have been scanned
                tileGenerator.UpdateVisited(scanned, new Cell(col-wallWidth.x+1, miny), new Cell(col, maxy));
            }
        }
    }

    // this procedure is used to create horizontal box colliders for the walls in the maze
    void CreateHorizontalBoxCollider(int col, int row, bool[,] scanned, bool[,] path, GameObject colliders) {
        int minx = col;
        int maxx = col;
        Vector2 offset = new Vector2(0f, 0f);

        if (!mazeManager.IsPath(col, row, path) && !scanned[col, row]) {
            // determining the max x and min x values
            while (!mazeManager.IsPath(maxx+1, row, path) && mazeManager.IsInBounds(maxx+1, row, maxx:mazeSize.x+pathWidth.x, maxy:mazeSize.y+pathWidth.y)) maxx++;     // east
            while (!mazeManager.IsPath(minx-1, row, path) && mazeManager.IsInBounds(minx-1, row, maxx:mazeSize.x+pathWidth.x, maxy:mazeSize.y+pathWidth.y)) minx--;     // west
            
            // creating the horizontal collider
            // the max or min x must be outside the playable maze area or; the starting row must within the playable map area
            if ((maxx >= col+pathWidth.x-1 || minx <= col-(pathWidth.x-1)) && (row-wallWidth.y > bottomLeft.y && row < topRight.y)) {
                if (maxx > topRight.x) maxx = topRight.x;
                if (minx < bottomLeft.x) minx = bottomLeft.x;

                // checking if the size of the collider is large enough
                if (maxx-minx+1 < 0.0002 || wallWidth.y < 0.0002) return;

                // creating the collider, and giving it initial values
                collider = colliders.AddComponent<BoxCollider2D>();
                offset.x = Mathf.Floor((maxx+minx)/2)+1f;
                offset.y = row+2-wallWidth.y+Mathf.Floor(wallWidth.y/2);
                collider.size = new Vector2(maxx-minx+1, wallWidth.y);

                // accounting for a floating point centre
                if ((maxx+minx) % 2 != 0) offset.x += 0.5f;
                if (wallWidth.y % 2 == 0) offset.y -= 0.5f;

                collider.offset = new Vector2(offset.x, offset.y);

                // indicating which cells have been visited/scanned
                tileGenerator.UpdateVisited(scanned, new Cell(minx, row-wallWidth.y+1), new Cell(maxx, row));
            }
        }
    }

    // this procedure will create the box colliders for the border of the maze
    void CreateBorderBoxColliders(GameObject colliders, Tilemap groundMap) {
        BoxCollider2D collider;
        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int end = new Vector2Int(0, 0);
        Vector2 offset = new Vector2(0f, 0f);

        // iterating through each of the NSEW sides
        for (int side = 0; side < 4; side++) {
            // creating x box colliders based on the exits on that particular side + 1
            for (int exit = 0; exit < 1 + exitsOnEachSide[side]; exit++) {
                // setting the starting location
                if (exit == 0) {
                    if (side == 0) start = new Vector2Int(bottomLeft.x - borderWidth, topRight.y + borderWidth);        // north
                    else if (side == 1) start = new Vector2Int(bottomLeft.x - borderWidth, bottomLeft.y - borderWidth); // south
                    else if (side == 2) start = new Vector2Int(topRight.x + borderWidth, bottomLeft.y - borderWidth);   // east
                    else if (side == 3) start = new Vector2Int(bottomLeft.x - borderWidth, bottomLeft.y - borderWidth); // west

                    end = start;
                }

                // creating the collider
                collider = colliders.AddComponent<BoxCollider2D>();

                // determining the size of the collider
                if (side == 0 || side == 1) {
                    while (!groundMap.HasTile(new Vector3Int(end.x, end.y, 0)) && end.x < topRight.x+borderWidth+1) end.x++;
                    collider.size = new Vector2(Mathf.Abs(end.x - start.x),borderWidth);
                } else if (side == 2 || side == 3) {
                    while (!groundMap.HasTile(new Vector3Int(end.x, end.y, 0)) && end.y < topRight.y+borderWidth+1) end.y++;
                    collider.size = new Vector2(borderWidth, Mathf.Abs(end.y - start.y));
                }

                // determining the collider offset
                if (side==0) {      // north
                    offset.y = start.y - borderWidth/2 + 1;       
                    offset.x = start.x + (end.x - start.x)/2 + 1;

                    // adjusting offset for floating point centres
                    if (borderWidth % 2 == 0) offset.y += 0.5f;         
                    if ((end.x - start.x) %2 == 0) offset.x -= 0.5f;    
                } else if (side==1) {           // south
                    offset.y = start.y + borderWidth/2 + 1;       
                    offset.x = start.x + (end.x - start.x)/2 + 1;

                    // adjusting offset for floating point centres
                    if (borderWidth % 2 == 0) offset.y -= 0.5f;        
                    if ((end.x - start.x) %2 == 0) offset.x -= 0.5f;    
                } else if (side==2) {           // east
                    offset.y = start.y + (end.y - start.y)/2 + 1;  
                    offset.x = start.x - borderWidth/2 + 1;

                    // adjusting offset for floating point centres
                    if (borderWidth % 2 == 0) offset.x += 0.5f;         
                    if ((end.y - start.y) %2 == 0) offset.y -= 0.5f;         
                } else if (side==3) {           // west
                    // determining collider properties
                    offset.y = start.y + (end.y - start.y)/2 + 1;  
                    offset.x = start.x + borderWidth/2 + 1;

                    // adjusting offset for floating point centres
                    if (borderWidth % 2 == 0) offset.x -= 0.5f;         
                    if ((end.y - start.y) %2 == 0) offset.y -= 0.5f;     
                }

                // moving the collider
                collider.offset = offset;

                // updating the start and end positions
                if (side == 0 || side == 1) {           // horizontal
                    start.x = end.x + pathWidth.x - wallWidth.x + 1;
                    end.x = start.x;
                } else if (side == 2 || side == 3) {    // vertical
                    start.y = end.y + pathWidth.y - wallWidth.y + 1;
                    end.y = start.y;
                }
            }
        }
    }

    void CreateFinishCollider(GameObject colliders) {
        BoxCollider2D collider;

        // creating the collider
        collider = colliders.AddComponent<BoxCollider2D>();

        // changing the collider to be trigger only
        collider.isTrigger = true;

        // changing the tag of the collider
        collider.gameObject.tag = "Finish";

        // changing the size of the collider
        collider.size = new Vector2(mazeManager.GetActualMazeSize().x, mazeManager.GetActualMazeSize().y);

        // changing the location of the collider
        collider.offset = new Vector2(mazeManager.GetCentre().x + 1, mazeManager.GetCentre().y);
    }
}