using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class MazeGenerator : MonoBehaviour {
    [Header("Scripts")]
    public MazeManager mazeManager;
    public TileGenerator tileGenerator;

    private Vector2Int bottomLeft;              // storing the bottom left corner of the playable map area (the bottom left most path cell)
    private Vector2Int topRight;                // storing the top right corner of the playable map area
    private Vector2 spawnCentre;                // the centre of the player spawn platform
    private int stepSize = 0;
    private List<Node> nodes = new List<Node>();
    private List<Node> exitNodes = new List<Node>();
    private MapData mapData = new MapData();

    // variables from the maze manager
    private bool isInit = false;        // checking if the class has all of the required values
    private bool autoDistributeExits;
    private int exits;
    private int borderWidth;
    private int extraSolutions;
    private Vector2Int mazeSize;
    private Vector2Int pathWidth;
    private Vector2Int wallWidth;
    private int[] exitsOnEachSide = new int[4];
    private List<List<Cell>> exitLocations;

    // performing pre-use tasks
    public void Init() {
        autoDistributeExits = mazeManager.GetAutoDistributeExits();
        exits = mazeManager.GetMazeExits();
        borderWidth = mazeManager.GetBorderWidth();
        mazeSize = mazeManager.GetMazeSize();
        pathWidth = mazeManager.GetPathWidth();
        wallWidth = mazeManager.GetWallWidth();
        extraSolutions = mazeManager.GetExtraSolutions();
        exitsOnEachSide = mazeManager.GetExitDistribution();
        exitLocations = mazeManager.GetExitLocations();

        topRight = new Vector2Int(0, 0);
        bottomLeft = new Vector2Int(mazeSize.x + pathWidth.x, mazeSize.y + pathWidth.y);

        isInit = true;
    }

    // creating the base of the map, given the dimensions and width
    public void GenerateBase(bool[,] path, List<TileBase> tiles, Tilemap map, int width, int minx, int miny, int maxx, int maxy) {
        // filling each cell in the tilemap with a tile
        for (int row = miny-width; row < maxy+width+1; row++) {
            for (int col = minx-width; col < maxx+width+1; col++) {
                // the cell is not part of the part and doesn't already have a tile
                if (!mazeManager.IsPath(col, row, path) && !map.HasTile(new Vector3Int(col, row, 0))) map.SetTile(new Vector3Int(col, row, 0), tileGenerator.ChooseWeightedTile(tiles));
            }
        }
    }

    // Determine the locations of the path (create the path)
    // A modification of recursive backtracking
    // this procedure will modify the given array, thus storing the location of the path
    public void GeneratePaths(bool[,] visited, Cell currentCell) {
        if (isInit) {
            // variables
            Stack cellStack = new Stack();              // checking for possible paths at these cells   
            Directions[] directionList = new Directions[]{Directions.North, Directions.South, Directions.East, Directions.West};        // storing the possible directions
            Cell nextCell = new Cell();                 // storing the next possible cell
            Cell segmentTopRight = new Cell();          // top right cell in a path segment
            Cell segmentBottomLeft = new Cell();        // bottom Left cell in a path segment
            
            // indicating that the first cell is part of the path
            cellStack.Push(currentCell);

            // determining the path while there are cells to travel to
            while (cellStack.Count > 0) {
                currentCell = (Cell)cellStack.Pop();      // storing the last cell 

                mazeManager.ShuffleDirections(directionList);
                // iterating through each of the directions
                foreach (Directions currentDirection in directionList) {
                    // determing the next cell
                    if (currentDirection == Directions.North || currentDirection == Directions.South) nextCell = currentCell.addDirection(currentDirection, pathWidth.y+1);   
                    if (currentDirection == Directions.East || currentDirection == Directions.West) nextCell = currentCell.addDirection(currentDirection, pathWidth.x+1);

                    // checking if next cell has not been reached and is valid
                    if (mazeManager.IsInBounds(nextCell.x, nextCell.y)) {
                        // Checking if there is "open" adjecent node
                        if (!mazeManager.CollidedWithPath(visited, currentCell, nextCell, currentDirection)) {   
                            // indicate that we want to check the next cell
                            nextCell.setPriorCell(currentCell);
                            cellStack.Push(nextCell);
                        } 
                    } 
                }
                
                // check if the current cell has not been previously visited
                if (!visited[currentCell.x, currentCell.y]) {
                    // find the top-left and bottom-right corners
                    segmentTopRight = new Cell(Mathf.Max(currentCell.x+pathWidth.x-wallWidth.x, currentCell.getPriorCell().x+pathWidth.x-wallWidth.x), Mathf.Max(currentCell.y+pathWidth.y-wallWidth.y, currentCell.getPriorCell().y+pathWidth.y-wallWidth.y));
                    segmentBottomLeft = new Cell(Mathf.Min(currentCell.x, currentCell.getPriorCell().x), Mathf.Min(currentCell.y, currentCell.getPriorCell().y));

                    // set cells as part of the path
                    tileGenerator.UpdateVisited(visited, segmentTopRight, segmentBottomLeft);

                    // updating the map data
                    mapData = tileGenerator.UpdateMapData(mapData, segmentTopRight, segmentBottomLeft);

                    // indicating that the cell is also a node
                    nodes.Add(currentCell);

                    // determining the max and min, x and y values for the maze
                    if (currentCell.x < bottomLeft.x) bottomLeft.x = currentCell.x;
                    if (currentCell.x > topRight.x) topRight.x = currentCell.x+pathWidth.x-wallWidth.x;

                    if (currentCell.y < bottomLeft.y) bottomLeft.y = currentCell.y;
                    if (currentCell.y > topRight.y) topRight.y = currentCell.y+pathWidth.y-wallWidth.y;
                }
                
                // marking the cell as visited
                visited[currentCell.x, currentCell.y] = true;
            }
        }
    }

    // Generates the area where the player(s) will spawn. This area should be rectangular in shape
    public void GenerateSpawningArea(bool[,] path, int spawnWidth) {
        // ensuring that the size of the spawning area is not larger than the maze size
        if (spawnWidth > (topRight.x-bottomLeft.x)/(pathWidth.x+1)) spawnWidth = (topRight.x-bottomLeft.x)/(pathWidth.x+1);
        if (spawnWidth > (topRight.y-bottomLeft.y)/(pathWidth.y+1)) spawnWidth = (topRight.y-bottomLeft.y)/(pathWidth.y+1);
        
        // moving the bottom left corner to one of the centre nodes
        int xTranslation = (pathWidth.x+1)*((topRight.x-bottomLeft.x)/(pathWidth.x+1)/2);         
        int yTranslation = (pathWidth.y+1)*((topRight.y-bottomLeft.y)/(pathWidth.y+1)/2);

        // moving the translation values in order to centre the spawning area   
        if (spawnWidth%2 == 0) {            
            xTranslation -= (pathWidth.x+1)*(int)(spawnWidth/2-1);
            yTranslation -= (pathWidth.y+1)*(int)(spawnWidth/2-1);
        } else {
            xTranslation -= (pathWidth.x+1)*(int)(spawnWidth/2);
            yTranslation -= (pathWidth.y+1)*(int)(spawnWidth/2);
        }
        
        // marking the spawning area
        tileGenerator.UpdateVisited(path, new Cell(bottomLeft).translate(xTranslation, yTranslation), new Cell(bottomLeft).translate(xTranslation+spawnWidth*pathWidth.x-wallWidth.x+(spawnWidth-1), yTranslation+spawnWidth*pathWidth.y-wallWidth.y+(spawnWidth-1)));

        // updating the map data
        mapData = tileGenerator.UpdateMapData(mapData, new Cell(bottomLeft).translate(xTranslation, yTranslation), new Cell(bottomLeft).translate(xTranslation+spawnWidth*pathWidth.x-wallWidth.x+(spawnWidth-1), yTranslation+spawnWidth*pathWidth.y-wallWidth.y+(spawnWidth-1)));

        // storing the centre of the spawning platform
        spawnCentre = new Vector2(bottomLeft.x+xTranslation+(spawnWidth*pathWidth.x-wallWidth.x+(spawnWidth-1))/2f, bottomLeft.y+yTranslation+(spawnWidth*pathWidth.y-wallWidth.y+(spawnWidth-1))/2f);
    }

    // determing where to place the exits on the edge of the maze. In order for this to make sense with a gameplay POV, the player must spawn somewhere in the centre of the maze
    public void GenerateExits(bool[,] path) {
        if (isInit) {
            // variables
            int exitsCreated = 0;
            Cell segmentTopRight = new Cell();      // temp top right corner
            Cell segmentBottomLeft = new Cell();    // temp bottom left corner
            Cell segmentTopLeft = new Cell();       // temp top left corner
            Cell segmentBottomRight = new Cell();   // temp bottom right corner

            // verifying values; there must be enough room for the specified amount of exits
            if (exits > 2*(int)(mazeSize.x/(pathWidth.x+1)) + 2*(int)(mazeSize.y/(pathWidth.y+1))) {
                // capping the amount of exits
                exits = (int)(mazeSize.x/(pathWidth.x+1)) + (int)(mazeSize.y/(pathWidth.y+1));
            }

            // evenly determining the sides to place the exits
            if (autoDistributeExits) EvenlyDistributeExits(exitsCreated, exitsOnEachSide);

            // creating the exits
            // itereating through each of the sides, as long as the number of exits is not 0
            if (exits > 0 || exitsOnEachSide[0]+exitsOnEachSide[1]+exitsOnEachSide[2]+exitsOnEachSide[3] > 0) {
                for (int side = 0; side < 4; side++) {
                    // checking if there should be any exits for the particular side
                    if (side == 0 || side == 1) {
                        // distance between two exits
                        stepSize = (int)((float)mazeSize.x/((float)pathWidth.x+1)/((float)exitsOnEachSide[side]+1));           // north and south
                        
                        // checking for a minimum distance between two exits
                        if (stepSize < 2) {
                            stepSize = 2;
                            exitsOnEachSide[side] = (int)((mazeSize.x/(pathWidth.x+1))/2-1);
                        }

                        // placing each of the exits
                        for (int exit = 1; exit < exitsOnEachSide[side]+1; exit++) {
                            if (side == 0) {    // north
                                segmentBottomLeft = new Cell(bottomLeft.x + exit*(pathWidth.x+1)*stepSize, topRight.y-pathWidth.y+wallWidth.y);
                                segmentTopRight = new Cell(segmentBottomLeft.x + pathWidth.x - wallWidth.x, segmentBottomLeft.y + pathWidth.y - wallWidth.y + borderWidth);
                                
                                exitLocations.Add(new List<Cell>(){segmentBottomLeft, segmentTopRight});            // storing the location and dimensions of one of the exits
                                exitNodes.Add(new Node(segmentBottomLeft.x, segmentBottomLeft.y+pathWidth.y+1));    // adding the exit as a node
                                mapData = tileGenerator.UpdateMapData(mapData, segmentBottomLeft, segmentTopRight.translate(0, wallWidth.y));
                            } else if (side == 1) {    // south
                                segmentTopLeft = new Cell(bottomLeft.x + exit*(pathWidth.x+1)*stepSize, bottomLeft.y);
                                segmentBottomRight = new Cell(segmentTopLeft.x + pathWidth.x - wallWidth.x, segmentTopLeft.y - borderWidth - 2);
                                
                                exitLocations.Add(new List<Cell>(){segmentTopLeft, segmentBottomRight});
                                exitNodes.Add(new Node(segmentTopLeft.x, segmentTopLeft.y-pathWidth.y-1));
                                mapData = tileGenerator.UpdateMapData(mapData, segmentTopLeft, segmentBottomRight.translate(0, -wallWidth.y));
                            }   

                            // indicating that the last node added the nodes list is an exit node
                            exitNodes[exitNodes.Count-1].isExit = true;
                        }
                    } else if (side == 2 || side == 3) {
                        stepSize = (int)((float)mazeSize.y/((float)pathWidth.y+1)/((float)exitsOnEachSide[side]+1));      // east and west

                        if (stepSize < 2) {
                            stepSize = 2;
                            exitsOnEachSide[side] = (int)(mazeSize.y/(pathWidth.y+1)/2-1);
                        }
                        
                        for (int exit = 1; exit < exitsOnEachSide[side]+1; exit++) {
                            if (side == 2) {    // east
                                segmentBottomLeft = new Cell(topRight.x-pathWidth.x+wallWidth.x, bottomLeft.y + exit*(pathWidth.y+1)*stepSize);
                                segmentTopRight = new Cell(segmentBottomLeft.x + pathWidth.x - wallWidth.x + borderWidth, segmentBottomLeft.y + pathWidth.y - wallWidth.y);
                                
                                exitLocations.Add(new List<Cell>(){segmentBottomLeft, segmentTopRight});
                                exitNodes.Add(new Node(segmentBottomLeft.x+pathWidth.x+1, segmentBottomLeft.y));
                                mapData = tileGenerator.UpdateMapData(mapData, segmentBottomLeft, segmentTopRight.translate(wallWidth.x, 0));
                            } else if (side == 3) {    // west
                                segmentBottomRight = new Cell(bottomLeft.x, bottomLeft.y + exit*(pathWidth.y+1)*stepSize);
                                segmentTopLeft = new Cell(segmentBottomRight.x - borderWidth, segmentBottomRight.y + pathWidth.y - wallWidth.y);

                                exitLocations.Add(new List<Cell>(){segmentBottomRight, segmentTopLeft});
                                exitNodes.Add(new Node(segmentBottomRight.x-pathWidth.x-1, segmentBottomRight.y));
                                mapData = tileGenerator.UpdateMapData(mapData, segmentBottomRight, segmentTopLeft.translate(-wallWidth.x, 0));
                            }

                            // indicating that the last node added the nodes list is an exit node
                            exitNodes[exitNodes.Count-1].isExit = true;
                        }
                    }
                }
            }
        }
    }

    // This procedure will evenly distribute the locations of the exits
    void EvenlyDistributeExits(int exitsCreated, int[] exitsOnEachSide) {
        if (isInit) {
            Directions[] directions = {Directions.North, Directions.South, Directions.East, Directions.West};

            // resetting the exits on each side
            for (int side = 0; side < 4; side++) exitsOnEachSide[side] = 0;

            // equally distributed exits
            while (exitsCreated < exits) {
                mazeManager.ShuffleDirections(directions);

                // iterating through each of the sides
                foreach (Directions direction in directions) {
                    if (direction == Directions.North && exitsOnEachSide[0] < (int)(mazeSize.x/(pathWidth.x+1)) && mazeManager.GetEnabledExits()[0]) {          // north
                        exitsOnEachSide[0]++;
                        exitsCreated++;
                    } else if (direction == Directions.South && exitsOnEachSide[1] < (int)(mazeSize.x/(pathWidth.x+1)) && mazeManager.GetEnabledExits()[1]) {   // south
                        exitsOnEachSide[1]++;
                        exitsCreated++;
                    } else if (direction == Directions.East && exitsOnEachSide[2] < (int)(mazeSize.y/(pathWidth.y+1)) && mazeManager.GetEnabledExits()[2]) {    // east
                        exitsOnEachSide[2]++;
                        exitsCreated++;
                    } else if (direction == Directions.West && exitsOnEachSide[3] < (int)(mazeSize.y/(pathWidth.y+1)) && mazeManager.GetEnabledExits()[3]) {    // west
                        exitsOnEachSide[3]++;
                        exitsCreated++;
                    }

                    // if enough exits have been created, exit
                    if (exitsCreated >= exits) break;
                }
            }
        }
    }

    // "Poking" holes in the walls to create more solutions
    public void GenerateExtraSolutions(bool[,] path) {
        if (isInit) {
            // Variables
            Node node = new Cell();                 // starting cell for possible hole location 
            Node nextNode = new Cell();             // ending cell for possible hole location
            Cell segmentTopRight = new Cell();      // top right corner of a path segment
            Cell segmentBottomLeft = new Cell();    // bottom Left corner of a path segment
            int randNodeIndex;                      // choosing a random node in the maze
            bool[,] visitedNodes = new bool[path.GetLength(0), path.GetLength(1)];
            Directions[] directions = {Directions.North, Directions.South, Directions.East, Directions.West};       // possible directions

            // ensuring arguments are positive integers
            extraSolutions = Mathf.Abs(extraSolutions);

            // checking if there are enough locations for a hole to be. Otherwise, a infinite loop will occur.
            if (extraSolutions > nodes.Count/2.5f) extraSolutions = (int)(nodes.Count/2.5f);

            // create the specified amount of extra solutions
            for (int solution = 0; solution < extraSolutions; solution++) {
                while (true) {
                    // Choosing a new random node
                    do {
                        randNodeIndex = (int)(Random.Range(0f, nodes.Count));

                        node = nodes[randNodeIndex];
                    } while (visitedNodes[(int)node.x, (int)node.y]);
                    
                    // choosing a random node, adjacent to the original node
                    do {
                        mazeManager.ShuffleDirections(directions);

                        if (directions[0] == Directions.North || directions[0] == Directions.South) nextNode = node.addDirection(directions[0], pathWidth.y+1);
                        if (directions[0] == Directions.East || directions[0] == Directions.West) nextNode = node.addDirection(directions[0], pathWidth.x+1);
                    } while(!mazeManager.IsInBounds((int)nextNode.x, (int)nextNode.y));

                    // checking if either of cells have previously visited
                    if (!visitedNodes[(int)node.x, (int)node.y] && !visitedNodes[(int)nextNode.x, (int)nextNode.y]) break;
                } 

                // find the top-left and bottom-right corners
                segmentTopRight = new Cell((int)Mathf.Max(node.x+pathWidth.x-wallWidth.x, nextNode.x+pathWidth.x-wallWidth.x), (int)Mathf.Max(node.y+pathWidth.y-wallWidth.y, nextNode.y+pathWidth.y-wallWidth.y));
                segmentBottomLeft = new Cell((int)Mathf.Min(node.x, nextNode.x), (int)Mathf.Min(node.y, nextNode.y));

                // updating visited path
                tileGenerator.UpdateVisited(path, segmentTopRight, segmentBottomLeft);

                // updating the map data
                mapData = tileGenerator.UpdateMapData(mapData, segmentTopRight, segmentBottomLeft);
    
                visitedNodes[(int)node.x, (int)node.y] = true;
                visitedNodes[(int)nextNode.x, (int)nextNode.y] = true;
            }
        }
    }

    ////// getters //////
    public Vector2Int GetTopRightCorner() {
        return topRight;
    }

    public Vector2Int GetBottomLeftCorner() {
        return bottomLeft;
    }

    public int[] GetExitsOnEachSide() {
        return exitsOnEachSide;
    }

    public List<Node> GetNodes() {
        return nodes;
    }

    public List<Node> GetExitNodes() {
        return exitNodes;
    }

    public Vector2 GetSpawnCentre() {
        return spawnCentre;
    }

    public MapData GetMapData() {
        return mapData;
    }
}
