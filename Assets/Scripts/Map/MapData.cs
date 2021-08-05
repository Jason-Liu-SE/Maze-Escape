using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapData {
    // dictionary to store a map of TileData
    private SortedDictionary<Tuple<int, int>, TileData> map;

    // constructors
    public MapData() {
        this.map = new SortedDictionary<Tuple<int, int>, TileData>();
    }

    // function to add values to the map
    public void Add(TileData data) {
        map.Add(new Tuple<int, int>(data.x, data.y), data);
    }

    // removes keys and values from the map
    public void Remove(TileData data) {
        map.Remove(Tuple.Create(data.x, data.y));
    }

    // checks if a key exits for a given x and y position
    public bool Contains(int x, int y) {
        // contains the key
        if (map.ContainsKey(Tuple.Create(x, y))) return true;

        // does not contain the key
        return false;
    }

    // getters
    public IEnumerator GetEnumerator() {
        return map.GetEnumerator();
    }

    public int GetX(int x, int y) {
        // checking if the TileData exists in the dictionary
        if (map.ContainsKey(Tuple.Create(x, y)))
            return map[Tuple.Create(x, y)].x;
        
        return 0;
    }

    public int GetY(int x, int y) {
        // checking if the TileData exists in the dictionary
        if (map.ContainsKey(Tuple.Create(x, y)))
            return map[Tuple.Create(x, y)].y;

        return 0;
    }

    public bool IsPath(int x, int y) {
        // checking if the TileData exists in the dictionary
        if (map.ContainsKey(Tuple.Create(x, y)))
            return map[Tuple.Create(x, y)].isPath;

        return false;
    }

    public SortedDictionary<Tuple<int, int>, TileData>.KeyCollection GetKeys() {
        return map.Keys;
    }

    // setters
    public void SetX(int x, int y) {
        // checking if the TileData exists in the dictionary
        if (map.ContainsKey(Tuple.Create(x, y)))
            map[Tuple.Create(x, y)].x = x;
    }

    public void SetY(int x, int y) {
        // checking if the TileData exists in the dictionary
        if (map.ContainsKey(Tuple.Create(x, y)))
            map[Tuple.Create(x, y)].y = y;
    }

    public void SetPath(int x, int y, bool isPath) {
        // checking if the TileData exists in the dictionary
        if (map.ContainsKey(Tuple.Create(x, y)))
            map[Tuple.Create(x, y)].isPath = isPath;
    }
}
