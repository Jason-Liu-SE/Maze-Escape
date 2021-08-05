using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildMenuManager : MonoBehaviour {
    public MenuNavigator menuNavigator;

    // the following variable:
    // 1. represents whether or not the previous menu must be closed in order for the next menu to be displayed
    // 2. if false, the previous menu must be close; if true, the previous menu can stay open
    public bool isOverlayMenu = false;      

    void OnEnable() {
        menuNavigator.AddMenuToHierarchy(transform.gameObject);
    }

    void OnDisable() {
        if (isOverlayMenu)
            menuNavigator.CloseMenu();
    }
}
