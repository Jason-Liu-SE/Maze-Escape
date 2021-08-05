using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuManager : MonoBehaviour {
    public GameObject effects;
    public GameObject pauseMenu;

    void Update() {
        // continously checking if the player pressed the pause menu key
        if (InputManager.GetKeyDown("escape")) {
            // checking if the menu is paused or unpaused
            if (Time.timeScale > 0) {   // game is running
                pauseMenu.SetActive(true);
                PauseGame(true);
            } else if (Time.timeScale == 0 && pauseMenu.activeSelf) {   // game is paused
                // iterating through each of the menus and disabling them
                foreach (Transform child in transform) child.gameObject.SetActive(false);
                PauseGame(false);
            }
        }
    }

    public void PauseGame(bool pause) {
        if (pause) {
            effects.SetActive(true);
            Time.timeScale = 0;
        } else if (!pause) {
            effects.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void LoadMainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }
}
