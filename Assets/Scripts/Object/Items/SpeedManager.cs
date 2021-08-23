using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedManager : MonoBehaviour {
    [Min(0f)] public float duration;
    [Min(0f)] public float speedIncreasePercent;

    private PlayerMovement playerMovement;
    private float originalSpeed;

    private GameObject speedEffect;

    void Start() {
        // locating the player's movement script
        playerMovement = GameObject.Find("Objects/Player").GetComponent<PlayerMovement>();

        // storing the original speed of the player
        originalSpeed = playerMovement.walkSpeed;

        // locating the speed HUD effect
        speedEffect = GameObject.Find("HUD/Effects/Speed Effect");
    }

    void OnTriggerStay2D() {
        // checking if the player already has the speed effect. If they do, don't allow the player to pick up the speed boost
        if (playerMovement.walkSpeed == originalSpeed) {
            ActivateAdrenaline();
            
            // deleting the sprite renderer and the collision trigger.
            // if the transform is disabled, the timer will never stop
            Destroy(transform.gameObject.GetComponent<BoxCollider2D>());
            Destroy(transform.gameObject.GetComponent<SpriteRenderer>());
        }
    }

    public void ActivateAdrenaline() {
        // applying the speed boost if the player is at normal speed. Otherwise, just increase the duration of the effect
        if (playerMovement.walkSpeed == originalSpeed) 
            playerMovement.walkSpeed *= speedIncreasePercent/100+1; 
        
        speedEffect.SetActive(true);

        // starting a timer for the adrenaline boost. Once the timer is over, the boost should be removed
        StartCoroutine(Timer());
    }

    public void RemoveAdrenaline() {
        playerMovement.walkSpeed = originalSpeed;
        speedEffect.SetActive(false);
    }

    IEnumerator Timer() {
        float timer = 0f;

        while (timer < duration) {
            // checking if the player

            // increasing the timer
            timer += Time.deltaTime;

            yield return null;
        }

        RemoveAdrenaline();
    }
}
