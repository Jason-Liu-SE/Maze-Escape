using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TileGenerator : MonoBehaviour {
    [Header("Ground Tiles")]
    public List<TileBase> grasses;
    public List<TileBase> stones;

    [Header("Wall Tiles")]
    public List<TileBase> wallBottoms;
    public List<TileBase> wallTops;
    public List<TileBase> wallFrontLeftBottoms;
    public List<TileBase> wallFrontLeftTops;
    public List<TileBase> wallFrontRightBottoms;
    public List<TileBase> wallFrontRightTops;
    public List<TileBase> wallSingleBottoms;
    public List<TileBase> wallSingleTops;

    [Header("Siding Top")]
    public List<TileBase> topSidings;
    public List<TileBase> topSidingCorners;

    [Header("Siding Left")]
    public List<TileBase> leftSidings;
    public List<TileBase> leftSidingsSharp;
    public List<TileBase> leftSidingsSoft;
    public List<TileBase> leftSidingCorners;

    [Header("Siding Right")]
    public List<TileBase> rightSidings;
    public List<TileBase> rightSidingsSharp;
    public List<TileBase> rightSidingsSoft;
    public List<TileBase> rightSidingCorners;

    [Header("Scripts")]
    public MazeManager mazeManager;

    // Marks cells as part of the path
    public void UpdateVisited(bool[,] visited, Cell start, Cell end) {
        // drawing the path by iterating through each cell within the start and end cells
        for (int row = Mathf.Min(start.y, end.y); row < Mathf.Max(start.y, end.y)+1; row++) {           
            for (int col = Mathf.Min(start.x, end.x); col < Mathf.Max(start.x, end.x)+1; col++) {
                // if the cell is within maze size and has not been visited yet
                if (mazeManager.IsInBounds(col, row, maxx:visited.GetLength(0), maxy:visited.GetLength(1))) if (!visited[col, row]) visited[col, row] = true;          
            }
        }
    }

    // Same concept as UpdateVisited, but with a MapData type instead of bool[,]
    public MapData UpdateMapData(MapData map, Cell start, Cell end) {
        // drawing the path by iterating through each cell within the start and end cells
        for (int row = Mathf.Min(start.y, end.y); row < Mathf.Max(start.y, end.y)+1; row++) {           
            for (int col = Mathf.Min(start.x, end.x); col < Mathf.Max(start.x, end.x)+1; col++) {
                // checking if the mapData already contains a value for the given position
                if (!map.Contains(col, row)) map.Add(new TileData(col, row, true));
                else map.SetPath(col, row, true);
            }
        }

        return map;
    }

    // this procedure will take two corners and fill in the area between them (inclusing) with a chosen tile
    void PlaceTiles(Cell start, Cell end, List<TileBase> tiles, Tilemap map) {
        for (int row = Mathf.Min(start.y, end.y); row < Mathf.Max(start.y, end.y)+1; row++) {           
            for (int col = Mathf.Min(start.x, end.x); col < Mathf.Max(start.x, end.x)+1; col++) {
                map.SetTile(new Vector3Int(col, row, 0), ChooseWeightedTile(tiles));          
            }
        }
    }

    // this function will take a MapData variable and place tiles in the positions that constitute the path
    public void DrawMap(MapData mapData, List<TileBase> tiles, Tilemap map) {
        // drawing the interior paths of the path
        /*for (int col = 0; col < mapData.GetLength(0); col++) {
            for (int row = 0; row < mapData.GetLength(1); row++) {
                if (mapData.IsPath(col, row)) map.SetTile(new Vector3Int(col, row, 0), ChooseWeightedTile(tiles));
            }
        }*/
        foreach (System.Tuple<int, int> key in mapData.GetKeys()) {
            if (mapData.IsPath(key.Item1, key.Item2)) map.SetTile(new Vector3Int(key.Item1, key.Item2, 0), ChooseWeightedTile(tiles));
        }
    }

    // this function draws a pair of walls
    public void PlaceWallTiles(int col, int row, List<TileBase> bottoms, List<TileBase> tops, List<TileBase> backgrounds, Tilemap wallMap, Tilemap backgroundMap) {
        // changing the background colour of the tile
        backgroundMap.SetTile(new Vector3Int(col, row, 0), ChooseWeightedTile(backgrounds));
        backgroundMap.SetTile(new Vector3Int(col, row+1, 0), ChooseWeightedTile(backgrounds));

        // drawing the wall on top
        wallMap.SetTile(new Vector3Int(col, row, 0), ChooseWeightedTile(bottoms));
        wallMap.SetTile(new Vector3Int(col, row+1, 0), ChooseWeightedTile(tops));
    }

    // checking if a given cell already has a tile on the given Tilemap
    public bool HasTile(int col, int row, Tilemap map) {
        return map.HasTile(new Vector3Int(col, row, 0));
    }

    // determining which tile from a list to use, based on a weight
    public TileBase ChooseWeightedTile(List<TileBase> tiles) {
        if (tiles.Count < 1)
            return null;

        int randIndex = (int)Random.Range(0, (float)tiles.Count);

        return tiles[randIndex];
    }
}
