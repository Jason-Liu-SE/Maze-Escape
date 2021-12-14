using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// this class manages the flashlight. It will mainly control turning off/on the flashlight and battery life (energy).
public class FlashlightManager : MonoBehaviour {
    [Header("Energy")]
    [Min(0f)] public float totalEnergyCapacity;
    [Min(0f)] public float currentEnergy;
    [Min(0f)] public float energyLossPerSecond;

    [Header("Display")]
    public Vector2 dimensions;
    public Image energyDisplayArea;
    public Image flashlightIcon;
    public Sprite onSprite;
    public Sprite offSprite;
    public TextMeshProUGUI text;

    [Header("Flashlight")]
    public GameObject reflectionLight;
    public GameObject projectedLight;

    private bool lightIsEnabled;
    private bool flashlightEnabled;

    void Update() {
        if (flashlightEnabled) {
            DecreaseEnergy(Time.deltaTime * dimensions.x/totalEnergyCapacity * energyLossPerSecond);
            UpdateDisplayedEnergy();
        }
    }

    // this function will refill this flashlight's battery
    public void Refill(float percentRefill) {
        currentEnergy += totalEnergyCapacity * percentRefill/100;

        // ensuring that the current energy is within bounds
        if (currentEnergy > totalEnergyCapacity)
            currentEnergy = totalEnergyCapacity;
        else if (currentEnergy < 0)
            currentEnergy = 0;
    }

    // this function will remove energy from currentEnergy
    private void DecreaseEnergy(float decreaseAmount) {
        if (lightIsEnabled) {
            // checking if energy can be removed from the battery
            if (currentEnergy > 0) {
                currentEnergy -= decreaseAmount;

                // ensuring that the current energy is within bounds after removing energy
                if (currentEnergy > totalEnergyCapacity)        // greater than the total capacity
                    currentEnergy = totalEnergyCapacity;
                else if (currentEnergy < 0)     // less than zero
                    currentEnergy = 0;
            } else if (currentEnergy < 0)
                currentEnergy = 0;
        }
    }

    // this function will change the displayed energy, along with the icon
    private void UpdateDisplayedEnergy() {
        energyDisplayArea.rectTransform.offsetMax = new Vector2(-100+currentEnergy*dimensions.x/totalEnergyCapacity, dimensions.y);

        // changing the icon if the currentEnergy is > 0 or = 0;
        if (currentEnergy == 0 && flashlightIcon.sprite != offSprite)
            flashlightIcon.sprite = offSprite;
        else if (currentEnergy > 0 && flashlightIcon.sprite != onSprite)
            flashlightIcon.sprite = onSprite;

        // changing the displayed battery text
        text.text = (int)Mathf.Round(currentEnergy/totalEnergyCapacity*100) + "%";
        
        // disabling or enabling the flashlight object
        if (currentEnergy == 0 && lightIsEnabled) {       // there is no energy left, but the lights are still enabled. Therefore, turn off the lights
            if (projectedLight != null)
                projectedLight.SetActive(false);
            if (reflectionLight != null)
                reflectionLight.SetActive(false);
        } else if (currentEnergy > 0 && !lightIsEnabled) { // there is energy left, but the lights are not enabled. So, enable them
            if (projectedLight != null)
                projectedLight.SetActive(true);
            if (reflectionLight != null)
                reflectionLight.SetActive(true);
        }

        // checking if the flashlight's lights are currently enabled
        if (currentEnergy == 0) 
            lightIsEnabled = false; 
        else if (currentEnergy > 0)
            lightIsEnabled = true; 
    }

    // turns on the flashlight
    public void Enable() {
        flashlightEnabled = true;

        // enabling the light
        if (projectedLight != null)
                projectedLight.SetActive(true);
        if (reflectionLight != null)
            reflectionLight.SetActive(true);
    }

    // turns off the flashlight
    public void Disable() {
        flashlightEnabled = false;

        // changing the displayed sprite
        flashlightIcon.sprite = offSprite;

        // disabling the light
        if (projectedLight != null)
                projectedLight.SetActive(false);
        if (reflectionLight != null)
            reflectionLight.SetActive(false);
    }
}
