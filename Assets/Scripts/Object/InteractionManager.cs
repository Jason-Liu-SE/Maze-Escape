using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

// this class will allow the player to interact with a given object, through triggers
public class InteractionManager : MonoBehaviour {
    // variables required for the InteractionManager to work properly
    [Header("Button Functionality")]
    public GameObject interactPrompt;
    public bool isInstant;
    [Min(0f)] public float holdDuration;

    // storing the events to be executed
    public UnityEvent events;

    // storing the coroutine
    private IEnumerator coroutine;
    private bool coroutineIsRunning = false;

    // text mesh pro component
    [Header("Displayed Interact Text")]
    [SerializeField] private string start;
    [SerializeField] private string end;

    private TextMeshProUGUI text;

    // progress bar variables
    [Header("Progress Bar")]
    public GameObject progressBar;
    public Image background;
    public Image bar;

    // locating the components
    void Start() {
        // text mesh pro
        text = interactPrompt.GetComponent<TextMeshProUGUI>();
    }

    void Update() {
        // changing the displayed keybind text for the textmeshpro component
        if (text != null) 
            text.SetText(start + "<color=#FFEA00>" + InputManager.GetKeyForValue("interact") + "</color>" + end);
        
        // checking if a coroutine is not currently running and the the input prompt is displayed
        if (!coroutineIsRunning && interactPrompt.activeSelf) {
            // starting a new coroutine
            coroutine = Interact();
            StartCoroutine(coroutine);
            coroutineIsRunning = true;
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if (interactPrompt == null)
            return;

        // displaying the prompt
        if (!interactPrompt.activeSelf)
            interactPrompt.SetActive(true);

        // indicating that a coroutine can be started
        coroutineIsRunning = false;
    }

    void OnTriggerExit2D(Collider2D collider) {
        if (interactPrompt == null)
            return;
        
        // hiding the prompt
        interactPrompt.SetActive(false);

        // stopping the interact coroutine
        if (coroutineIsRunning) {
            StopCoroutine(coroutine);
            coroutineIsRunning = false;
        }
    }

    // this coroutine will will for a key press to execute the events (functions) attached to this script
    IEnumerator Interact() {
        // waiting for key press
        while (!InputManager.GetKey("interact"))
            yield return null;

        ///////////////////////////////
        // the interact key was pressed
        ///////////////////////////////
        // checking if the interact key must be held
        if (!isInstant) {
            float timer = 0;

            // activating the progress bar
            ActivateProgressBar();

            // timing how long the player presses the interact key
            while (InputManager.GetKey("interact")) {
                // counting the amount of time passesd since the interact key was pressed
                timer += Time.deltaTime;

                // increasing the progress on the progress bar.
                bar.rectTransform.offsetMax = new Vector2(2*(-background.rectTransform.offsetMax.x - timer * (-background.rectTransform.offsetMax.x/holdDuration)), background.rectTransform.offsetMax.y);

                // if the held time exeeds "holdDuration" seconds, exit this loop
                if (timer > holdDuration) 
                    break;

                // pauses the coroutine for a short duration of time. Otherwise, the timer will be updated too quickly
                yield return null;
            }

            // deactivating the progress bar
            DeactivateProgressBar();

            // checking how long the interact button was pressed
            if (timer < holdDuration) {
                coroutineIsRunning = false;
                yield break;
            }
        }

        coroutineIsRunning = false;

        // executing each attached event
        events.Invoke();
    }

    private void ActivateProgressBar() {
        progressBar.SetActive(true);

        // setting the progress bar to 0%
        bar.rectTransform.offsetMin = new Vector2(0, background.rectTransform.offsetMin.y);
    }

    private void DeactivateProgressBar() {
        progressBar.SetActive(false);
    }
}

