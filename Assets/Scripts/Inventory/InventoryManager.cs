using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class InventoryManager : MonoBehaviour {
    // storing each item's texture
    [Serializable] public struct items {
        public Image Flashlight;
        public Image Map;
        public Image Trap;
    }

    public items itemTextures;
    public InventoryDisplay invDisplay;
    
    private Dictionary<string, bool> inventory = new Dictionary<string, bool>(){{"Flashlight", false}, 
                                                                               {"Map", false},
                                                                               {"Trap", false}};

    void Start() {
        inventory["Flashlight"] = true;
        inventory["Map"] = false;
        inventory["Trap"] = false;

        // updating the items
        UpdateItem("Flashlight");
        UpdateItem("Map");
        UpdateItem("Trap");
    }

    // removes an item from the inventory
    public void UseItem(string item) {
        if (inventory.ContainsKey(item)) {
            if (inventory[item]) {
                inventory[item] = false;
                UpdateItem(item);
            }
        }
    }

    // adds an item the the inventory
    public void AddItem(string item) {
        if (inventory.ContainsKey(item)) {
            if (!inventory[item]) {
                inventory[item] = true;
                UpdateItem(item);
            }
        }
    }

    // enables or disables item, but does not remove it from the inventory
    public void ToggleItem(string item) {
        if (item == "Map") {
            if (invDisplay.MapOff == itemTextures.Map.sprite)                   // the map is disabled
                invDisplay.Show(invDisplay.MapOn, itemTextures.Map);
            else if (invDisplay.MapOn == itemTextures.Map.sprite)               // the map is enabled
                invDisplay.Show(invDisplay.MapInactive, itemTextures.Map);

            // // the map has just be picked up
            if (invDisplay.MapOff != invDisplay.MapInactive)                    // first time picking up the map {
                invDisplay.MapOff = invDisplay.MapInactive;
        }
    }

    // returns true if the associated item is in the inventory and is true, otherwise, returns false
    public bool GetItem(string item) {
        if (inventory.ContainsKey(item))
            return inventory[item];
        
        return false;
    }

    // iterates through the entire inventory and displayed the corresponding icon
    public void UpdateItem(string item) {
        if (inventory.ContainsKey(item)) {
            if (item == "Flashlight")
                if (inventory["Flashlight"]) 
                    invDisplay.Show(invDisplay.FlashlightOn, itemTextures.Flashlight);
                else
                    invDisplay.Show(invDisplay.FlashlightOff, itemTextures.Flashlight);

            if (item == "Map")
                if (inventory["Map"])
                    invDisplay.Show(invDisplay.MapOn, itemTextures.Map);
                else
                    invDisplay.Show(invDisplay.MapOff, itemTextures.Map);

            if (item == "Trap")
                if (inventory["Trap"])
                    invDisplay.Show(invDisplay.TrapOn, itemTextures.Trap);
                else
                    invDisplay.Show(invDisplay.TrapOff, itemTextures.Trap);
        }
    }
}
