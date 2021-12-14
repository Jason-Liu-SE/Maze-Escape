using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour {
    public InventoryManager invManager;
    public FlashlightManager flashlightManager;

    // waiting for key presses
    void Update() {
        if (InputManager.GetKeyDown("ability1")) {      // flashlight
            // checking if the flashlight was on or off
            if (invManager.GetItem("Flashlight")) {     // on
                invManager.UseItem("Flashlight");
                flashlightManager.Disable();
            } else {        // off
                invManager.AddItem("Flashlight");
                flashlightManager.Enable();
            }
        } 

        else if (InputManager.GetKeyDown("ability2")) {   // map
            if (invManager.GetItem("Map")) {   
                invManager.ToggleItem("Map");
            }
        } 

        else if (InputManager.GetKeyDown("ability3")) {   // trap
            if (invManager.GetItem("Trap")) {   
                invManager.UseItem("Trap");
            }
        } 
    }
}
