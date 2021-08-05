using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class InputManager {
    // storing the keybinds
    public static Keybinds keybinds = new Keybinds();

    // storing information about each keybind. 1. Associated action; 2. Default keybinds for each action; 3. Times used and with which actions
    private static SortedDictionary<string, KeyCode> actionsToKeybinds = new SortedDictionary<string, KeyCode>();
    private static SortedDictionary<string, KeyCode> defaultKeybindsForAction = new SortedDictionary<string, KeyCode>();
    public static SortedDictionary<KeyCode, List<string>> keyCount = new SortedDictionary<KeyCode, List<string>>();

    static InputManager() {
        //////////// initializing the keybinds //////////////
        // loading the keybinds into the keybinds object
        keybinds = SaveManager.Load("config.json");

        // filling the actionsToKeybinds dictionary
        PopulateActionsToKeybindsDictionary();

        // keeping an accurate count of the used keybinds
        PopulateKeyCount();
    }

    ///////////////////////////////////////////////////////////////////
    // checking if a pre-set action key has been pressed or released //
    ///////////////////////////////////////////////////////////////////
    public static bool GetKeyDown(string action) {
        // converting the action to lowercase
        action = action.ToLower();

        if (!ValidAction(action)) return false;

        // checking if the associated keybind with the action has been pressed
        if (Input.GetKeyDown(actionsToKeybinds[action])) return true;

        return false;
    }

    public static bool GetKeyUp(string action) {
        // converting the action to lowercase
        action = action.ToLower();

        if (!ValidAction(action)) return false;

        // checking if the associated keybind with the action has been pressed
        if (Input.GetKeyUp(actionsToKeybinds[action])) return true;

        return false;
    }

    public static bool GetKey(string action) {
        // converting the action to lowercase
        action = action.ToLower();

        if (!ValidAction(action)) return false;

        // checking if the associated keybind with the action has been pressed
        if (Input.GetKey(actionsToKeybinds[action])) return true;

        return false;
    }

    private static bool ValidAction(string action) {
        // checking if the action exists in the dictionary
        if (!actionsToKeybinds.ContainsKey(action)) {
            Debug.Log("InputManger.GetKey: argument \"" + action + "\" does not exist");
            return false;
        }
        
        return true;
    }

    // this function will determine which key was pressed 
    public static KeyCode GetPressedKey() {
        foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            if (InputManager.GetKeyDown(key)) 
                return key;
        
        return KeyCode.None;
    }

    //////////////////////////////////////////////////////////
    /////////// getting key pressed through key codes ////////
    //////////////////////////////////////////////////////////
    public static bool GetKeyDown(KeyCode key) {
        if (Input.GetKeyDown(key)) return true;

        return false;
    }

    public static bool GetKeyUp(KeyCode key) {
        if (Input.GetKeyUp(key)) return true;

        return false;
    }

    public static bool GetKey(KeyCode key) {
        if (Input.GetKey(key)) return true;

        return false;
    }

    // checks if any key has been pressed down
    public static bool AnyKeyDown() {
        if (Input.anyKeyDown) return true;
        return false;
    }

    public static bool AnyKey() {
        if (Input.anyKey) return true;
        return false;
    }

    ////////////////////////////////////////////////////
    /////// keybind dictionary modification/access ////
    ///////////////////////////////////////////////////
    public static void SetKey (string action, KeyCode key) {
        if (!actionsToKeybinds.ContainsKey(action)) return;

        // changing the keyCount value associated with the keybind
        UpdateKeyCount(action, key);

        // changing the associated keybind
        actionsToKeybinds[action] = key;

        // updating the keybind associated with the action in the keybind class
        typeof(Keybinds).GetField(action).SetValue(keybinds, key.ToString());

        // saving the keybind
        SaveManager.Save(keybinds, "config.json");
    }

    public static KeyCode GetKeyForValue(string value) {
        value = value.ToLower();

        if (!actionsToKeybinds.ContainsKey(value))
            return KeyCode.None;
        
        return actionsToKeybinds[value];
    }

    // returns a list containing the actions associated with each keybind
    private static List<string> GetValuesForKey(KeyCode key) {
        List<string> actions = new List<string>();

        // iterating through each assigned keybind
        foreach (KeyValuePair<string, KeyCode> keybind in actionsToKeybinds) 
            // checking if the value of the keybind (keyCode) is equal to the desired key
            // if so, the keybind's key (associated action) will be added to the values list
            if (keybind.Value == key)
                actions.Add(keybind.Key);

        return actions;
    }

    private static void PopulateActionsToKeybindsDictionary() {
        if (actionsToKeybinds.Count == 0) {
            // mapping each action (field name) to it's associated keybind. This is stored in the actionsToKeybinds dictionary
            KeyCode keyCode;            // stores the currently-read keybind

            // iterating through each of the fields in the Keybinds class
            foreach(FieldInfo field in typeof(Keybinds).GetFields()) {
                // storing the keycode
                // checking if the field has a value
                if (field.GetValue(keybinds) != null && !field.GetValue(keybinds).Equals("")) {
                    // checking if the value can be converted into a keycode
                    if (CanConvertToKeyCode((string)(field.GetValue(keybinds))))
                        // parse the value and store it as a keybind
                        keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), (string)(field.GetValue(keybinds)));
                    else
                        keyCode = KeyCode.None;
                } else
                    keyCode = KeyCode.None;

                // adding the keycode to the dictionary
                actionsToKeybinds.Add(field.Name, keyCode);
            }
        }
    }

    ////////////////////////////////////////////////////
    ////////// modifying/accessing the keyCount/////////
    ////////////////////////////////////////////////////
    public static void PopulateKeyCount() {
        // checking if the keyCount list has been populated yet
        if (keyCount.Count == 0) {
            // populating the keyCount list with the action associated with each keybind
            // ie: fill the list with keyCode and string pairs, where the string represents the action
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode))) { 
                // note: keyCount is a <KeyCode, List<string>>
                if (!keyCount.ContainsKey(key))
                    keyCount.Add(key, GetValuesForKey(key));
            }
        }
    }

    // changing the associated value in a <key, List<string>> pair 
    public static void UpdateKeyCount(string action, KeyCode key) {
        // checking which keybind was previously associated with the action
        KeyCode prevKey = actionsToKeybinds[action];
        
        if (prevKey != key) {
            // adding the action to the new key 
            if (!keyCount[key].Contains(action))
                keyCount[key].Add(action);

            // removing the action from the previous key
            keyCount[prevKey].Remove(action);
        }
    }

    ///////////////////////////////////////////////////////////////////
    ////////// functions that interact with the keybinds object ///////
    ///////////////////////////////////////////////////////////////////
    private static bool CanConvertToKeyCode(string keyCode) {
        // iterating through each of the keyCodes
        foreach(KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            // checking if the key name is the same as the passed argument
            if (key.ToString().Equals(keyCode))
                return true;
        
        // none of the keycodes matched the passed argument
        return false;
    }

    ///////////////////////////////////////////////////////////////////
    ////////// functions that interact with the keybinds object ///////
    ///////////////////////////////////////////////////////////////////
    public static void Reset() {
        // defaulting the keybinds
        keybinds.up = "W";
        keybinds.down = "S";
        keybinds.left = "A";
        keybinds.right = "D";
        keybinds.escape = "Escape";
        keybinds.interact = "E";

        // saving the new keybinds
        SaveManager.Save(keybinds, "config.json");
    
        // updating the actionsToKeybinds dictionary
        actionsToKeybinds.Clear();
        PopulateActionsToKeybindsDictionary();

        // updating the keyCount
        keyCount.Clear();
        PopulateKeyCount();
    }
}
