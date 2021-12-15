using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public enum Directions {North, South, East, West, NE, NW, SE, SW};

public class MazeManager : MonoBehaviour {
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////// SETUP ////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Maze objects
    [Header("Maze Objects")]
    public Tilemap groundMap;               // the path map
    public Tilemap wallMap;                 // wall map
    public Tilemap leftSidingMap;           // stores the wall sidings/borders, that hug the left side of the cell
    public Tilemap rightSidingMap;          // stores the wall sidings/borders, that hug the right side of the cell
    public Tilemap topSidingMap;            // stores the wall sidings/borders, that hug the bottom side of the cell (these will always be ABOVE the wall)
    public GameObject colliders;

    // Maze fields
    [Header("Properties")]
    [SerializeField] private Vector2Int mazeSize;           // dimensions of the map in cells
    [SerializeField] private Vector2Int wallWidth;          // cell width of each wall
    [SerializeField] private Vector2Int pathWidth;          // cell width of a path
    [SerializeField] private int extraSolutions;            // the amount of solutions for a given maze (add 1 for the initial maze solution)
    [SerializeField] private int borderWidth;               // the cell width of the enclosing wall around the maze
    [SerializeField] private int spawnWidth;                // size of the player's spawning area (spawnCentre of the maze)
    [SerializeField] private bool autoDistributeExits = true;   // true = evenly distribute maze exits; false = manually distribute maze exits
    [SerializeField] private int mazeExits;                     // if autoDistributeExits == true, this variable defines how many exits are to be distributed
    [SerializeField] private InspectorDirections exitsEnabled;  // storing which sides may have exits
    [SerializeField] private InspectorSideExits exitDistribution;   // the inspector field to determine the manual location of exits

    [System.NonSerialized] private List<List<Cell>> exitLocations = new List<List<Cell>>();     // this list will hold the bottomLeft and topRight corners of a particular exit. Useful to locate the exit or to draw the exit
    [System.NonSerialized] private int[] exitsOnEachSide = new int[4];              // stores the amount of exits on each side  
    [System.NonSerialized] private Vector2Int bottomLeft = new Vector2Int(0, 0);    // cell bottom left corner of the playable map area (path)
    [System.NonSerialized] private Vector2Int topRight = new Vector2Int(0, 0);      // cell top right corner of the playable map area (path)
    [System.NonSerialized] private Vector2 spawnCentre = new Vector2(0, 0);              // storing the centre of the player spawning platform of the maze

    // Required scripts in order to orperate
    [Header("Required Scripts")]
    public ColliderGenerator colliderGenerator;     
    public SidingsGenerator sidingsGenerator;
    public TileGenerator tileGenerator;
    public WallGenerator wallGenerator;
    public MazeGenerator mazeGenerator;

    // Maze generation 
    private bool[,] path;                                   // holds maze path
    private MapData mapData;                                
    private List<Node> nodes = new List<Node>();            // storing key pivots for maze generation. Eg: possible path intersections. These nodes can also be used for the path finding algorithm
    private Vector2 nodeOffset;                             // used to shift the nodes towards the spawnCentre of the path (for the path finding algorithm)

    // Data types
    [System.Serializable]
    // this struct is used to store 4 bool values, each keyworded
    private struct InspectorDirections {
        public bool north;
        public bool south;
        public bool east;
        public bool west;

        public InspectorDirections(bool n, bool s, bool e, bool w) {
            this.north = n;
            this.south = s;
            this.east = e;
            this.west = w;
        }
    }

    [System.Serializable]
    // this struct is used to store 4 int values, each keyworded
    private struct InspectorSideExits {
        public int north;
        public int south;
        public int east;
        public int west;

        private InspectorSideExits(int n, int s, int e, int w) {
            this.north = n;
            this.south = s;
            this.east = e;
            this.west = w;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////// GENERATING MAZE ////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Called when a new map is to be created
    private void ValidateInspectorFields() {
        // Ensuring properties are valid
        if (mazeSize.x < 1) mazeSize.x = 1;
        if (mazeSize.y < 1) mazeSize.y = 1;

        if (wallWidth.x < 1) wallWidth.y = 1;
        if (wallWidth.y < 1) wallWidth.y = 1;

        if (pathWidth.x < 1) pathWidth.x = 1;
        if (pathWidth.y < 1) pathWidth.y = 1;

        if (extraSolutions < 0) extraSolutions = 0;

        if (spawnWidth < 1) spawnWidth = 1;

        // PathWidth becomes the distance inbetween two nodes
        pathWidth.x += wallWidth.x - 1;
        pathWidth.y += 2 + wallWidth.y - 1;
    }

    private void PopulateHiddenFields() {
        // Calculating the offset required to spawnCentre a node on a path
        nodeOffset = new Vector2((pathWidth.x-wallWidth.x)/2f, (pathWidth.y-wallWidth.y)/2f);
        
        // Defaulting the top left and bottom right corners of the playable map area
        bottomLeft = new Vector2Int(mazeSize.x + pathWidth.x + 1, mazeSize.y + pathWidth.y+1);
        topRight = new Vector2Int(-1, -1);
    }

    private void SetupMapCanvas() {
        // Erasing the previous map
        groundMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        leftSidingMap.ClearAllTiles();
        rightSidingMap.ClearAllTiles();
        topSidingMap.ClearAllTiles();

        foreach (BoxCollider2D collider in colliders.GetComponentsInChildren<BoxCollider2D>()) {    // erasing each of the previous colliders
            Destroy(collider);
        }
    
        // Centering the maps
        groundMap.transform.position = new Vector3(0f, 0f, 0f);
        wallMap.transform.position = groundMap.transform.position;
        colliders.transform.position = groundMap.transform.position + new Vector3(-0.5f, -0.5f, 0);
        leftSidingMap.transform.position = groundMap.transform.position;
        rightSidingMap.transform.position = groundMap.transform.position;
        topSidingMap.transform.position = groundMap.transform.position;
    }

    public void GenerateMaze() {
        ValidateInspectorFields();
        PopulateHiddenFields();

        // Initiallizing a path
        path = new bool[mazeSize.x + pathWidth.x, mazeSize.y + pathWidth.y];

        // clearing, creating, and moving map objects
        SetupMapCanvas();

        // Generating the path location
        mazeGenerator.Init();
        mazeGenerator.GeneratePaths(path, ChooseRandomCell(mazeSize));
        topRight = mazeGenerator.GetTopRightCorner();
        bottomLeft = mazeGenerator.GetBottomLeftCorner();
        nodes = mazeGenerator.GetNodes();

        // Clearing out the spawnCentre of the map. This is where the player(s) will spawn
        mazeGenerator.GenerateSpawningArea(path, spawnWidth);
        spawnCentre = mazeGenerator.GetSpawnCentre();

        // Determining the locations of the exit(s) and updating the amount of exits on each side of the maze
        mazeGenerator.GenerateExits(path); 
        exitsOnEachSide = mazeGenerator.GetExitsOnEachSide(); 

        // Increasing the number of solutions
        mazeGenerator.GenerateExtraSolutions(path); 
        
        // The path variable contains the inner path for the maze, but none of the exit paths (outer paths)
        // mapData will store both the inner path and outer path, thereby storing the entire maze in one variable
        mapData = mazeGenerator.GetMapData();

        // Drawing the map
        tileGenerator.DrawMap(mapData, tileGenerator.grasses, groundMap);
        
        // Creating box colliders on the walls of the maze
        colliderGenerator.Init();
        colliderGenerator.GenerateBoxColliders(path, nodes, colliders, groundMap);
        
        // Placing the walls
        wallGenerator.Init();
        wallGenerator.GenerateWallMap(path, groundMap, wallMap);
        
        // Placing the siding for the walls
        sidingsGenerator.Init();
        sidingsGenerator.GenerateSidingsMap(path, groundMap);

        // Creating the base of the map
        mazeGenerator.GenerateBase(path, tileGenerator.stones, groundMap, borderWidth, bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);            // stone base
        mazeGenerator.GenerateBase(path, tileGenerator.grasses, groundMap, 20*borderWidth, bottomLeft.x, bottomLeft.y, topRight.x, topRight.y);        // grass base
        
        // retrieving the final list of nodes. This list will be used for the path finding
        nodes.AddRange(mazeGenerator.GetExitNodes());
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////// Helper Functions //////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Shuffling a list of directions
    public void ShuffleDirections(Directions[] directionList) {
        // stores the element 
        int num;

        // iterating through each direction
        for (int i = 0; i < directionList.Length-1; i++) {
            // choosing random index
            num = Random.Range(i, directionList.Length);
            Directions tempDirection = directionList[num];

            // swapping the directions
            directionList[num] = directionList[i];
            directionList[i] = tempDirection;
        }
    }

    // Checking if a node can connect to an adjacent node (N, S, E, W), without colliding with a pre-existing path
    public bool CollidedWithPath(bool[,] path, Cell current, Cell end, Directions direction) {
        // Checking if the current cell is the end cell
        while (!(current.x==end.x && current.y==end.y)) {
            // Moving the current cell
            current = current.addDirection(direction, 1);

            // Checking if the current cell is on another cell that has already been visited
            if (path[current.x, current.y]) return true;

            // Checking if the searching cell, current, is out of bounds
            if (!IsInBounds(current.x, current.y)) return true;
        }
        return false;
    }

    // Checking if a cell is in the map area
    public bool IsInBounds(int col, int row, int minx = -1, int miny = -1, int maxx = -1, int maxy = -1) {
        // assigning default maximum maze bounds
        if (maxx == -1) maxx = mazeSize.x;
        if (maxy == -1) maxy = mazeSize.y;

        // checking if the col and row are within the min bounds and max bounds
        if (col > minx && col < maxx && row > miny && row < maxy) return true;

        return false;
    }

    // Checking if a cell can connect to another cell without colliding with any walls
    public bool PathIsBlocked(bool[,] path, Cell curr, Cell end, Directions direction) {
        // Checking if the current cell is the end cell
        while (!curr.Equals(end)) {
            // Moving the current cell
            curr = curr.addDirection(direction, 1);

            // Checking if the current cell is on another cell that has already been visited
            if (!path[curr.x, curr.y]) return true;

            // Checking if the searching cell, current, is out of bounds
            if (!IsInBounds(curr.x, curr.y)) return true;
        }
        return false;
    }

    // determines if a cell is part of the path
    public bool IsPath(int col, int row, bool[,] path) {
        if (IsInBounds(col, row, maxx:mazeSize.x+pathWidth.x, maxy:mazeSize.y+pathWidth.y))                
            if (path[col, row]) return true;
        return false;
    }

    // choosing a random cell, within the maze size
    public Cell ChooseRandomCell(Vector2Int mazeSize) {
        int col = (int)Random.Range(0f, (float)mazeSize.x);
        int row = (int)Random.Range(0f, (float)mazeSize.y);

        return new Cell(col, row);
    }

    ///////////// Getters //////////////
    public Vector2 GetNodeOffset() {
        return nodeOffset;
    }

    public Vector2Int GetNodeSeparation() {
        return new Vector2Int(pathWidth.x+1, pathWidth.y+1);
    }

    public Vector2Int GetMazeSize() {       // this is the size specified in the inspector; not the actual size
        return mazeSize;
    }

    public Vector2Int GetActualMazeSize() {
        return new Vector2Int(topRight.x-bottomLeft.x+1+borderWidth*2, topRight.y-bottomLeft.y+borderWidth*2+3);
    }

    public Vector2Int GetActualInnerMazeSize() {
        return new Vector2Int(topRight.x-bottomLeft.x+1, topRight.y-bottomLeft.y+1);
    }
    
    public Vector2Int GetWallWidth() {
        return wallWidth;
    }

    public Vector2Int GetPathWidth() {
        return pathWidth;
    }

    public int GetExtraSolutions() {
        return extraSolutions;
    }

    public int GetBorderWidth() {
        return borderWidth;
    }

    public int GetMazeExits() {
        return mazeExits;
    }

    public bool GetAutoDistributeExits() {
        return autoDistributeExits;
    }

    public int[] GetExitDistribution() {
        return new int[]{exitDistribution.north, exitDistribution.south, exitDistribution.east, exitDistribution.west};
    }

    public int[] GetExitsOnEachSide() {
        return exitsOnEachSide;
    }

    public List<List<Cell>> GetExitLocations() {
        return exitLocations;
    }

    public Vector2Int GetBottomLeft() {
        return bottomLeft;
    }

    public Vector2Int GetTopRight() {
        return topRight;
    }

    public bool[] GetEnabledExits() {
        return new bool[]{exitsEnabled.north, exitsEnabled.south, exitsEnabled.east, exitsEnabled.west};
    }

    public List<Node> GetNodes() {
        // passing a clone of the nodes List. Otherwise, it will be passed by reference
        return nodes.GetRange(0, nodes.Count);
    }

    public Vector2 GetSpawnCentre() {
        return spawnCentre;
    }

    public Vector2 GetCentre() {
        return new Vector2((topRight.x-bottomLeft.x)/2f + bottomLeft.x, (topRight.y-bottomLeft.y)/2f + bottomLeft.y);
    }

    public MapData GetMapData() {
        return mapData;
    }
}
