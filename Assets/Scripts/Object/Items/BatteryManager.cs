using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryManager : MonoBehaviour {
    // this is how much energy the battery can refill the flashlight with (percent)
    [Range(0f, 100f)] public float batteryCapacityPercent;
    public bool activate;

    private FlashlightManager flashlightManager;

    void Start() {
        // finding the flashlightManager script
        flashlightManager = GameObject.Find("HUD/Inventory/Flashlight").GetComponent<FlashlightManager>();

        if (flashlightManager == null)
            Debug.Log("ERROR: No FlashlightManager component has been found.");
    }

    void Update() {
        if (activate) {
            RefillFlashlightEnergy();
            activate = false;    
        }
    }

    public void RefillFlashlightEnergy() {
        flashlightManager.Refill(batteryCapacityPercent);
    }
}
