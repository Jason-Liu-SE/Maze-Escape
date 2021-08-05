using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AGenerator : MonoBehaviour {
    public AManager AManager;

    private List<Node> path = new List<Node>();         // stores the best a* path found
    
    // this will generate the best a* path
    public void GenerateAPath(MapData map, Node start, Node end, Vector2Int nodeSep) {
        // clearing the previous path
        path.Clear();
        
        // checking if the start and end node are the same
        if (start.x == end.x && start.y == end.y) {
            end.parent = start;
            RetracePath(start, end);
            return;
        }

        // setting the start node's parent to itself
        start.parent = start;

        // a* variables
        List<Node> open = new List<Node>();
        HashSet<Node> closed = new HashSet<Node>();
        Node curr = start;
        Node openNode = new Node(0, 0);
        int openNodeIndex = 0;
        List<Node> nodes = new List<Node>();

        // adding the starting node to the open list
        open.Add(start);

        // algorithm
        while(open.Count > 0) {
            // popping the element on the open list with the lowest f value
            curr = open[open.Count-1];
            open.RemoveAt(open.Count-1);

            // indicating that the curr node has been visited
            closed.Add(curr);

            // checking if the node is the end node
            if (curr.x == end.x && curr.y == end.y) {
                // setting the end node's parent to the current node. Retracing the parent will lead back to the start node
                end.parent = curr.parent;

                RetracePath(start, end);
                return;
            }

            nodes = GenerateNextNodes(curr, end, map, nodeSep);
            // itering through each of the possible jumps from the curr cell
            foreach (Node node in nodes) {
                // checking if the node is in the closed list
                if (closed.Any(x => x.x == node.x && x.y == node.y)) continue;

                // setting the parent of node
                node.parent = curr;

                // calculting the node's g, h, and f scores 
                node.g = curr.g + 1;
                node.h = h(node, end);
                node.f = Mathf.Pow(node.g, 2) + node.h;

                // checking if the node is already in the open list
                openNodeIndex = open.FindIndex(x => x.x == node.x && x.y == node.y);
                if (openNodeIndex < 0) {        // the node is not in the open list
                    open.Add(node);
                } else {                        // the node is in the open list
                    openNode = open[openNodeIndex];

                    // the current g cost is lower than the g cost of the open node
                    if (node.g < openNode.g) {
                        openNode.g = node.g;
                        openNode.parent = node.parent;

                        // replacing the node in the open list
                        open[openNodeIndex] = openNode;
                    }
                }
            }

            // sorting the open list by f value. Descending order
            open.Sort((x, y) => y.f.CompareTo(x.f));
        }
        RetracePath(start, curr);
    }

    // returns a list of nodes containing the possible next nodes
    private List<Node> GenerateNextNodes(Node curr, Node end, MapData map, Vector2Int nodeSep) {
        List<Node> possibleNodes = new List<Node>();   // stores the possible nodes

        // storing the location of the node relative to 0, 0
        Vector2 zeroedPos = AManager.GetRelativePos(curr);

        // checking if the node is aligned with the node grid, or if it is in between the nodes on the node grid
        if ((zeroedPos.x/(float)nodeSep.x)%1==0 && (zeroedPos.y/(float)nodeSep.y)%1==0) {       // aligned with node grid
            if (IsWalkable(map, curr, new Node(curr.x - nodeSep.x, curr.y))) possibleNodes.Add(new Node(curr.x - nodeSep.x, curr.y));       // west
            if (IsWalkable(map, curr, new Node(curr.x + nodeSep.x, curr.y))) possibleNodes.Add(new Node(curr.x + nodeSep.x, curr.y));       // east
            if (IsWalkable(map, curr, new Node(curr.x, curr.y - nodeSep.y))) possibleNodes.Add(new Node(curr.x, curr.y - nodeSep.y));       // south
            if (IsWalkable(map, curr, new Node(curr.x, curr.y + nodeSep.y))) possibleNodes.Add(new Node(curr.x, curr.y + nodeSep.y));       // north

            if (IsWalkable(map, curr, new Node(curr.x - nodeSep.x, curr.y - nodeSep.y))) possibleNodes.Add(new Node(curr.x - nodeSep.x, curr.y - nodeSep.y));   // sw
            if (IsWalkable(map, curr, new Node(curr.x + nodeSep.x, curr.y + nodeSep.y))) possibleNodes.Add(new Node(curr.x + nodeSep.x, curr.y + nodeSep.y));   // ne
            if (IsWalkable(map, curr, new Node(curr.x + nodeSep.x, curr.y - nodeSep.y))) possibleNodes.Add(new Node(curr.x + nodeSep.x, curr.y - nodeSep.y));   // se
            if (IsWalkable(map, curr, new Node(curr.x - nodeSep.x, curr.y + nodeSep.y))) possibleNodes.Add(new Node(curr.x - nodeSep.x, curr.y + nodeSep.y));   // nw
        } else {    // not aligned with node grid
            // the zeroedPos variabled will be changed in order to store the bottom left node, relative to the transform
            zeroedPos.x = (curr.x - zeroedPos.x%nodeSep.x);
            zeroedPos.y = (curr.y - zeroedPos.y%nodeSep.y);

            // adding 4 of the neighbouring nodes
            if (IsWalkable(map, curr, new Node(zeroedPos.x, zeroedPos.y))) possibleNodes.Add(new Node(zeroedPos.x, zeroedPos.y));   // sw
            if (IsWalkable(map, curr, new Node(zeroedPos.x + nodeSep.x, zeroedPos.y))) possibleNodes.Add(new Node(zeroedPos.x + nodeSep.x, zeroedPos.y));   // se
            if (IsWalkable(map, curr, new Node(zeroedPos.x, zeroedPos.y + nodeSep.y))) possibleNodes.Add(new Node(zeroedPos.x, zeroedPos.y + nodeSep.y));   // nw
            if (IsWalkable(map, curr, new Node(zeroedPos.x + nodeSep.x, zeroedPos.y + nodeSep.y))) possibleNodes.Add(new Node(zeroedPos.x + nodeSep.x, zeroedPos.y + nodeSep.y));   // ne
        }

        // if the straigh-line distance btween the curr node and end node is unimpeded, then curr<-->end is the shortest path
        if (IsWalkable(map, curr, end)) possibleNodes.Add(end);

        return possibleNodes;
    }

    // checking if two nodes can be connected without colliding with a wall
    private bool IsWalkable(MapData map, Node start, Node end) {
        // checking if start and node have values
        if (start == null || end == null) return false;

        // checking the nodes between the start node and end node to see if they are all part of the walkable path
        for (int col = (int)Mathf.Min(start.x, end.x); col < (int)Mathf.Max(start.x, end.x) + 1; col++) 
            for (int row = (int)Mathf.Min(start.y, end.y); row < (int)Mathf.Max(start.y, end.y) + 1; row++)
                // checking if there are any cells that are not walkable. Ie: is not part of the path
                if (!map.IsPath(col, row)) 
                    return false;

        return true;
    }

    // retracing the path used to reach the end node. To do this, use the parent of a node
    private void RetracePath(Node start, Node end) {
        // iterting through the parent nodes until it reaches the start node
        while ((end.x != start.x || end.y != start.y) && (end.x != end.parent.x || end.y != end.parent.y)) {
            // adding the node to the path
            path.Add(end);

            // setting the node to its parent
            end = end.parent;
        }

        // reversing the path. This will yield a list of steps to get to the end node
        path.Reverse();
    }

    // returning the best path
    public List<Node> GetAPath() {
        return path;
    }

    // calculating the h score using Euclidean distance
    private float h(Node curr, Node end) {
        return Mathf.Pow(curr.x-end.x, 2) + Mathf.Pow(curr.y-end.y, 2);
    }
}