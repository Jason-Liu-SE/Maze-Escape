using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedManager : MonoBehaviour {
    [Min(0f)] public float duration;
    [Min(0f)] public float speedIncreasePercent;

    private PlayerMovement playerMovement;
    private float originalSpeed;

    public bool activateScript = false;
    public bool speedActive = false;

    private GameObject speedEffect;

    void Start() {
        // locating the player's movement script
        playerMovement = GameObject.Find("Objects/Player").GetComponent<PlayerMovement>();

        // storing the original speed of the player
        originalSpeed = playerMovement.walkSpeed;

        // locating the speed HUD effect
        speedEffect = GameObject.Find("HUD/Effects/Speed Effect");
    }

    void Update() {
        if (!speedActive)
            if (activateScript) {
                ActivateAdrenaline();
                speedActive = true;
                activateScript = false;
            }
    }

    public void ActivateAdrenaline() {
        // applying the speed boost 
        playerMovement.walkSpeed *= speedIncreasePercent/100+1; 
        speedEffect.SetActive(true);

        // starting a timer for the adrenaline boost. Once the timer is over, the boost should be removed
        StartCoroutine(Timer());
    }

    public void RemoveAdrenaline() {
        playerMovement.walkSpeed = originalSpeed;
        speedEffect.SetActive(false);
        speedActive = false;
    }

    IEnumerator Timer() {
        float timer = 0f;

        while (timer < duration) {
            timer += Time.deltaTime;

            yield return null;
        }

        RemoveAdrenaline();
    }
}
