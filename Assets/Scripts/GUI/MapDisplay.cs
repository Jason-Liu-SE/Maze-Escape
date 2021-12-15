using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {
    public MazeManager mazeManager;
    public Transform locationMarker;
    public Transform mapDisplay;
    public Transform player;
    private bool isActive = false;
    private Vector2 bottomLeft;
    private float sizeRatio = 0;
    private Vector2 locationMarkerSize;
    public void Show() {
        isActive = true;
        EnableChildren(true);
    }

    public void Hide() {
        isActive = false;
        EnableChildren(false);
    }

    // continuously update the location of the player on the map, and the resulting path
    void Update() {
        if (isActive) {
            bottomLeft = mazeManager.GetBottomLeft();

            // setting first-time variables
            if (sizeRatio == 0)
                sizeRatio = mapDisplay.GetComponent<RectTransform>().sizeDelta.x/mazeManager.GetActualInnerMazeSize().x;

            ////////// showing the current location of the player relative to the bottom left corner of the maze ///////////
            locationMarker.GetComponent<RectTransform>().anchoredPosition = new Vector2(locationMarkerSize.x/2 + (player.position.x - bottomLeft.x)*sizeRatio,
                                                                                        locationMarkerSize.y/2 + (player.position.y - bottomLeft.y)*sizeRatio);
        }
    }

    private void EnableChildren(bool isActive) {
        foreach (Transform child in transform)
            child.gameObject.SetActive(isActive);
    }
}
