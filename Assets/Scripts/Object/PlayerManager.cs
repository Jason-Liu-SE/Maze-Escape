using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public GameManager gameManager;

    // checking if the player collided with the enemy
    void OnTriggerEnter2D (Collider2D collider) {
        if (collider.gameObject.tag == "Enemy")
            gameManager.StopGame();
    }

    // checking if the player collided with the finish line (any of the exits on the maze)
    void OnTriggerExit2D(Collider2D collider) {
        if (collider.gameObject.tag == "Finish")
            gameManager.StopGame();
    }
}
