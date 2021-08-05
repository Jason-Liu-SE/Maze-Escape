using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell {
    public int x;           // x pos
    public int y;           // y pos
    private int prevx;      // x pos
    private int prevy;      // y pos

    public Cell() {
        this.x = 0;
        this.y = 0;
        this.prevx = 0;
        this.prevy = 0;
    }

    public Cell(int X, int Y) {
        this.x = X;
        this.y = Y;
        this.prevx = X;
        this.prevy = Y;
    }

    public Cell(Vector2Int cell) {
        this.x = cell.x;
        this.y = cell.y;
        this.prevx = cell.x;
        this.prevy = cell.y;
    }

    public Cell getPriorCell() {
        return new Cell(this.prevx, this.prevy);
    }

    public void setPriorCell(Cell origin) {
        this.prevx = origin.x;
        this.prevy = origin.y;
    }

    public Cell translate(int X, int Y) {
        return new Cell(this.x+X, this.y+Y);
    } 

    public Cell addDirection(Directions direction, int magnitude) {
        switch (direction) {
            case Directions.North:
                return new Cell(this.x, this.y + magnitude);
            case Directions.South:
                return new Cell(this.x, this.y - magnitude);
            case Directions.East:
                return new Cell(this.x + magnitude, this.y);
            case Directions.West:
                return new Cell(this.x - magnitude, this.y);
        }

        return new Cell(this.x, this.y);
    }

    public static Cell operator+(Cell cell1, Cell cell2) {
        return new Cell(cell1.x + cell2.x, cell1.y + cell2.y);
    }
}
