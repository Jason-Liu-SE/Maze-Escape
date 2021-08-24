using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

// this script will be attached to each keybind button
public class KeybindManager : MonoBehaviour {
    private TextMeshProUGUI text;
    private Image backgroundImage;

    public GameObject waitingMenu;
    public GameObject keybindButtons;

    void Start() {
        // getting the background image
        foreach (Transform child in waitingMenu.transform) {
            if (child.gameObject.name == "Background") {
                backgroundImage = child.gameObject.GetComponent<Image>();
                break;
            }
        }

        // changing the dimensions of the background image
        if (backgroundImage != null) {
            if (backgroundImage.minWidth != Screen.width || backgroundImage.minHeight != Screen.height) 
                backgroundImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        }

        // updating the displayed keybind text for each keybind
        transform.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = InputManager.GetKeyForValue(transform.gameObject.name).ToString();
    
        // checking for duplicate keybinds
        UpdateDuplicateKeybinds(InputManager.GetKeyForValue(transform.gameObject.name));
    }

    // starting the waiting for key coroutine
    public void WaitForKey(string action) {
        // activing the game object
        waitingMenu.SetActive(true);
        
        // activate the coroutine 
        StartCoroutine(WaitForKeyCoroutine(action));
    }

    // this coroutine will indicate that the program is waiting for a key to be pressed
    // as well, a string containing the action keybind to be modified will be passed in
    private IEnumerator WaitForKeyCoroutine(string action) { 
        // wait for a key to be pressed
        while (!InputManager.AnyKey()) yield return null;
        
        // checking if escape is being held
        float timer = 0f;

        while (InputManager.GetKey(KeyCode.Escape)) {
            // counting the amount of time passesd since the escape key was pressed
            timer += Time.unscaledDeltaTime;

            // if the held time exeeds 1 second, clear the button
            if (timer > 1) {
                ClearText();
                InputManager.SetKey(action, KeyCode.None);
                break;
            } 

            // pauses the coroutine for a short duration of time. Otherwise, the timer will be updated too quickly
            yield return null;
        }

        // closing the menu if it is active
        if (waitingMenu.activeSelf)
            waitingMenu.SetActive(false);

        // check which key was pressed 
        KeyCode key = InputManager.GetPressedKey();

        // updating keybinds
        if (!InputManager.GetKey(KeyCode.Escape)) {
            // a key was pressed (other than escape)
            if (key != KeyCode.None) {
                // updating an action's associated keybind and changing the displayed text beside the action
                UpdateKeybind(action, key);
            }
        }

        // checking and indicating duplicate keybinds
        UpdateDuplicateKeybinds(key);
    }

    // this function will handle the updating of a keybind
    private void UpdateKeybind(string action, KeyCode key) {
        // changing the action's associated keybind
        InputManager.SetKey(action, key);

        // updating the displayed text
        UpdateText(key.ToString());
    }

    // this function will check for any re-used keybinds and change the colour of the text accordingly
    private void UpdateDuplicateKeybinds(KeyCode key) {
        // iterating through each of the keybind buttons
        KeyCode keyCode;

        // storing the key name
        string keyText = "";

        foreach (Transform button in keybindButtons.GetComponentInChildren<Transform>()) {
            // if the keytext is a null value, skip this key
            keyText = button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text;

            if (keyText == "" || keyText == null)
                continue;

            // checking how many times the associated keybind has been used.
            // If it has been used more than once, than don't change the colour to white
            keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), keyText);

            if (InputManager.keyCount[keyCode].Count < 2)
                button.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = new Color32 (243, 243, 243, 255);
        }

        // determining which actions have duplicate keybinds
        if (InputManager.keyCount[key].Count > 1)
            foreach (string action in InputManager.keyCount[key]) 
                // iterating through each of the buttons to find the duplicate buttons
                foreach (Transform button in keybindButtons.GetComponentInChildren<Transform>()) 
                    // checking if the button text is the duplicate key
                    if (button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text.Equals(key.ToString())) 
                        // changing the button text to another colour
                        button.gameObject.GetComponentInChildren<TextMeshProUGUI>().color = new Color32 (255, 0, 0, 255);
    }

    // this function will change the text of a button, given a string
    private void UpdateText (string newText) {
        // checking if a text object has been located yet
        if (text == null) text = GetComponentInChildren<TextMeshProUGUI>();

        // changing the displayed text
        text.text = newText;
    }

    // this function will clear the text displayed on a button
    public void ClearText () {
        // checking if a text object has been located yet
        if (text == null) text = GetComponentInChildren<TextMeshProUGUI>();

        // clearing the text
        text.text = "";
    }
}
