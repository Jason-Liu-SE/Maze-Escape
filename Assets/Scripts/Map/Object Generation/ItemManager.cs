using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class ItemManager : MonoBehaviour {
    public MazeManager mazeManager;
    public ItemGenerator ItemGenerator;
    public Tilemap wall;
    public Tile testTile;

    private List<Vector2> spawningLocations;        // the possible locations that items can spawn at 

    // once the map is loaded and generated, generate the items and their locations
    void Start() {
        // determining which nodes are dead ends. These are the possible locations that items can spawn at
        spawningLocations = DetermineDeadEnds();
        
        //////////////////////////////////////////
        // drawing the possible spawning locations
        //////////////////////////////////////////
        /*foreach (Vector2 pos in spawningLocations) 
            wall.SetTile(new Vector3Int((int)pos.x, (int)pos.y, 0), testTile);*/

        // generating the items
        ItemGenerator.GenerateItems(spawningLocations);
    }

    // retrieving the location of the nodes from the mazeManager. These nodes will be centred
    private List<Node> GetNodes() {
        Vector2 nodeOffset = mazeManager.GetNodeOffset();
        List<Node> nodes = mazeManager.GetNodes();

        // moving the nodes with accordance to their offset
        for (int index = 0; index < nodes.Count; index++)
            nodes[index] = nodes[index].translate(nodeOffset.x, nodeOffset.y);

        return nodes;
    }

    // determining the dead ends in the maze. These are the locations where items can spawn
    private List<Vector2> DetermineDeadEnds() {
        List<Vector2> deadEnds = new List<Vector2>();
        List<Node> nodes = GetNodes();
        Vector2 nodeSep = mazeManager.GetNodeSeparation();
        MapData mapData = mazeManager.GetMapData();

        // determining which nodes are dead ends.
        // iterating through each of the nodes
        foreach (Node node in nodes) {
            // checking if the node is within the maze playing area
            if (!IsWithinBounds(node, mapData))
                continue;

            // storing the amount of paths that exist at each node
            int paths = 0;

            // checking if a node exists in exactly one of the cardinal directions (NSEW)
            // north
            if (HasPathBetween(node, new Node(node.x, node.y + nodeSep.y), mapData))         
                paths++;

            // south
            if (HasPathBetween(node, new Node(node.x, node.y - nodeSep.y), mapData))         
                paths++;
            if (paths > 1) 
                continue;

            // east
            if (HasPathBetween(node, new Node(node.x + nodeSep.x, node.y), mapData))         
                paths++;
            if (paths > 1) 
                continue;

            // west
            if (HasPathBetween(node, new Node(node.x - nodeSep.x, node.y), mapData))         
                paths++;
            if (paths > 1) 
                continue;

            // adding the node to the list of dead ends, if it only has one possible path
            if (paths == 1)
                deadEnds.Add(new Vector2(node.x, node.y));
        }

        return deadEnds;
    }

    
    // Checking a path exists between two nodes
    private bool HasPathBetween(Node start, Node end, MapData mapData) {
        // iterating through each cell between the two nodes (inclusive)
        for (int col = (int)Mathf.Min(start.x, end.x); col < (int)Mathf.Max(start.x, end.x)+1; col++) {
            for (int row = (int)Mathf.Min(start.y, end.y); row < (int)Mathf.Max(start.y, end.y)+1; row++) {
                if (!mapData.Contains(col, row))
                    return false;
            }
        }
        return true;
    }

    // checking if a node is within the map bounds. If it is out of bounds, it cannot be a dead end
    private bool IsWithinBounds(Node node, MapData mapData) {
        Vector2 topRight = mazeManager.GetTopRight();
        Vector2 bottomLeft = mazeManager.GetBottomLeft();

        // checking if the node is not between the top-right and bottom-left corners (inclusive)
        if (node.x < bottomLeft.x || node.x > topRight.x || node.y < bottomLeft.y || node.y > topRight.y)
            return false;

        return true;
    }
}

