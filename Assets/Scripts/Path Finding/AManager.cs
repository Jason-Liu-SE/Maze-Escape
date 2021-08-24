using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AManager : MonoBehaviour {
    public AGenerator AGenerator;

    public MazeManager mazeManager;
    private Vector2 nodeOffset;
    private Vector2Int nodeSeparation;
    private List<Node> nodesList;
    private GameObject target;
    private List<Node> path = new List<Node>();
    private MapData mapData;

    private float minNodeX = 0;         // stores the minimum x and y to a node. This is used to convert negative x and y values (negative array index values) into positives integers
    private float minNodeY = 0;

    private bool fieldsArePopulated = false;

    // this is called when the maze is generated
    public void PopulateFields() {
        mazeManager = GameObject.Find("Map").GetComponent<MazeManager>();
        nodesList = mazeManager.GetNodes();
        nodeOffset = mazeManager.GetNodeOffset();
        nodeSeparation = mazeManager.GetNodeSeparation();
        mapData = mazeManager.GetMapData();

        // getting the player
        target = GameObject.Find("Objects").transform.Find("Player").gameObject;
        
        // centering each of the nodes (placing them in the centre of the path)
        // adding 0.5f to centre the character; the nodes will be off centre
        for (int index = 0; index < nodesList.Count; index++) 
            nodesList[index] = nodesList[index].translate(nodeOffset.x+0.5f, nodeOffset.y+0.5f);
        
        // placing the hunter in a random location (DELETE LATER)
        int randNum = (int)Random.Range(0f, (float)nodesList.Count);
        transform.position = new Vector3(nodesList[randNum].x, nodesList[randNum].y, transform.position.z);

        // determining the bottom left position
        DetermineMinCoordinates(nodesList);

        fieldsArePopulated = true;
    }

    public void GenerateAStarPath() {
        if (fieldsArePopulated) {
            // clearing the original path
            //if (path.Count > 0) foreach (Node node in path) Erase(node.x, node.y);

            // determining the start and end nodes
            Node start = new Node(transform.position.x, transform.position.y);               // hunter
            Node end = new Node(target.transform.position.x, target.transform.position.y);   // target

            // generating the path
            AGenerator.GenerateAPath(mapData, start, end, nodeSeparation);

            // retrieving the path
            path = AGenerator.GetAPath();

            /////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////// VISUALIZING THE A* PATH ////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////
            //foreach (Node node in nodesList) DrawNode(node.x, node.y);
            //foreach (Node node in path) Draw(node.x, node.y);
        }
    }

/*
    public void DrawNode(float x, float y) {
        mazeManager.wallMap.SetTile(new Vector3Int((int)x, (int)y, 0), mazeManager.tileGenerator.ChooseWeightedTile(mazeManager.tileGenerator.wallBottoms));
    }

    public void Draw(float x, float y) {
        mazeManager.wallMap.SetTile(new Vector3Int((int)x, (int)y, 0), mazeManager.tileGenerator.ChooseWeightedTile(mazeManager.tileGenerator.stones));
    }

    public void Erase(float x, float y) {
        mazeManager.wallMap.SetTile(new Vector3Int((int)x, (int)y, 0), mazeManager.tileGenerator.ChooseWeightedTile(mazeManager.tileGenerator.grasses));
    }*/

    // determining the minimum and max x and y positions in the node list
    private void DetermineMinCoordinates(List<Node> nodes) {
        // storing the minimum values
        float minx = System.Int32.MaxValue;
        float miny = System.Int32.MaxValue;

        // determining the lowest x and y values of the nodes
        foreach (Node node in nodes) {
            // min values
            if (node.x < minx) minx = node.x;
            else if (node.y < miny) miny = node.y;
        }

        // updating the lowest x and y values
        minNodeX = minx;
        minNodeY = miny;
    }

    // converting a node's coordinates to array indexes
    public Vector2 GetRelativePos(Node node) {
        return new Vector2(node.x-minNodeX, node.y-minNodeY);
    }

    // getting the instructions in order to follow the A star path
    public List<Node> GetPath() {
        return path;
    }
}
