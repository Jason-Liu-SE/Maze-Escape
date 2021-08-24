using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("Scripts")]
    public MazeManager mazeManager;
    public ItemManager itemManager;
    public AManager aStarManager; 

    [Header("Player")]
    public GameObject player;

    void Start() {
        Time.timeScale = 0f;

        StartGame();
    }

    public void StartGame() {
        Time.timeScale = 1f;

        // generating the map
        mazeManager.GenerateMaze();

        // generating the items
        itemManager.GenerateItems();

        // generating the variables values for the path finding algorithm
        aStarManager.PopulateFields();

        // moving the player to the centre of the map
        player.transform.position = mazeManager.GetSpawnCentre();
    }

    public void StopGame() {
        Time.timeScale = 0f;
    }
}
