using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour {
    // storing the texture for each item
    [Header("Flashlight Textures")]
    public Sprite FlashlightOff;
    public Sprite FlashlightOn;

    [Header("Map Textures")]
    public Sprite MapOff;
    public Sprite MapOn;
    public Sprite MapInactive;

    [Header("Trap Textures")]
    public Sprite TrapOff;
    public Sprite TrapOn;

    // displaying an object as on
    public void Show(Sprite sprite, Image target) {
        if (sprite)
            target.sprite = sprite;
    }
}
