using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeybindMenuManager : ChildMenuManager {
    public GameObject keybindButtons;

    public void ResetKeybinds() {
        // resetting the keybinds in the input manager
        InputManager.Reset();
        
        // updating the TMPro text on each keybind button
        foreach (Transform button in keybindButtons.GetComponentInChildren<Transform>()) 
            // updating the displayed keybind text for each keybind
            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = InputManager.GetKeyForValue(button.gameObject.name).ToString();
        
        // setting each keybind colour to white since there should not be any duplicate keybinds
        foreach (Transform button in keybindButtons.GetComponentInChildren<Transform>()) 
            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = new Color32 (243, 243, 243, 255);
    }
}
