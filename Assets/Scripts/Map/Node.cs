using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public bool isDeadEnd;
    public bool isExit; 

    // location of the node
    public float x;           
    public float y;           
    
    // variables used for A* search algorithm
    public float g;       // cost of moving from one node to another; this value continuously changes
    public float h;       // heuristic distance between one node to another (use euclidean distance b/c diagonals are possible)
    public float f;       // the sum of g and h; least cost from one node to another

    // storing the node that this node came from
    public Node parent;  

    // constructors
    public Node() {
        this.isDeadEnd = false;
        this.isExit = false;
        this.x = 0;
        this.y = 0;

        this.g = 0;
        this.h = 0;
        this.f = 0;

        this.parent = null;
    }

    public Node(float X, float Y) {
        this.isDeadEnd = false;
        this.isExit = false;
        this.x = X;
        this.y = Y;

        this.g = 0;
        this.h = 0;
        this.f = 0;

        this.parent = null;
    }

    public Node(Vector2Int node) {
        this.isDeadEnd = false;
        this.isExit = false;
        this.x = node.x;
        this.y = node.y;

        this.g = 0;
        this.h = 0;
        this.f = 0;

        this.parent = null;
    }

    // moving the node
    public Node translate(float X, float Y) {
        return new Node(this.x+X, this.y+Y);
    } 

    public Node addDirection(Directions direction, int magnitude) {
        switch (direction) {
            case Directions.North:
                return new Node(this.x, this.y + magnitude);
            case Directions.South:
                return new Node(this.x, this.y - magnitude);
            case Directions.East:
                return new Node(this.x + magnitude, this.y);
            case Directions.West:
                return new Node(this.x - magnitude, this.y);
        }

        return new Node(this.x, this.y);
    }

    // type conversion overriding
    public static explicit operator Cell(Node cell)
    {
        return new Cell((int)cell.x, (int)cell.y);
    }

    public static implicit operator Node(Cell cell) {
        return new Node() { x = cell.x, y = cell.y};
    }
}
