using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    public void LoadMenu(string menu) {
        Time.timeScale = 1;
        SceneManager.LoadScene(menu);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
