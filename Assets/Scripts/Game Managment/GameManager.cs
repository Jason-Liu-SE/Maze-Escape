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

    [Header("Menus")]
    public GameObject pauseMenu;
    public GameObject victoryMenu;
    public GameObject lossMenu;

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

    public void StopGame(string cause) {
        Time.timeScale = 0f;

        // checking what caused the game to stop (eg: Game Over; Victory)
        if (cause.ToLower().Equals("game over"))
            GameOver();
        else if (cause.ToLower().Equals("victory"))
            Victory();
    }

    private void GameOver() {
        pauseMenu.SetActive(false);
        lossMenu.SetActive(true);
    }

    private void Victory() {
        pauseMenu.SetActive(false);
        victoryMenu.SetActive(true);
    }
}
