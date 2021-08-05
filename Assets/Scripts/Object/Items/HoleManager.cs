using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class will hold the functionally for the hole item
public class HoleManager : MonoBehaviour {
    public Transform player;
    public Transform destination;

    void Start() {
        // locating the player
        player = GameObject.Find("Objects/Player").transform;
    }

    public void Teleport() {
        if (player == null || destination == null)
            return;

        player.position = new Vector3(destination.position.x, destination.position.y, player.position.z);
    }
}
