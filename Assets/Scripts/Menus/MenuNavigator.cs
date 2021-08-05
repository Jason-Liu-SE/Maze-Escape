using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigator : MonoBehaviour {
    // this stack will hold the GameObjects of the open menus, where the first
    // element is the parent, and the subsequent elements are children of the previous element.
    // Thus, the final element is the currently open/displayed menu
    private Stack<GameObject> menuHierarchy = new Stack<GameObject>();   

    public List<GameObject> exceptions = new List<GameObject>();    

    public GameObject defaultMenu = null; 

    void Update() {
        if (menuHierarchy.Count > 0) {
            // checking if the escape key was pressed and the current menu is not omited from this function
            if (InputManager.GetKeyDown("escape") && !IsException()) {
                // closing the last opened menu
                CloseMenu();

                // opening the parent menu. The default menu will be opened if there is no parent menu
                // the CloseMenu procedure closed the last-opened menu, so the current menu is the parent of the closed menu
                OpenCurrentMenu();
            }
        }
    }

    public void AddMenuToHierarchy(GameObject menu) {
        if (!menuHierarchy.Contains(menu))
            menuHierarchy.Push(menu);
    }

    public void CloseMenu() {
        if (menuHierarchy.Count > 0) {
            // disabling the last opened menu if it is active
            GameObject menu = menuHierarchy.Pop();

            if (menu.activeSelf)
                menu.SetActive(false);
        }
    }

    private void OpenCurrentMenu() {
        // opening the menu 
        if (menuHierarchy.Count > 0) {  // there is another menu to open
            GameObject menu = menuHierarchy.Peek();
            
            if (!menu.activeSelf)           // the menu must not be active
                menu.SetActive(true);   
        } else // there are no more menus in the stack. Open the default menu if there is one
            if (defaultMenu != null) 
                defaultMenu.SetActive(true);
    }

    // this function will iterate through the exceptions list to determine if the escape key
    // was pressed in a menu that omits this function
    private bool IsException() {
        if (exceptions.Count > 0)
            foreach (GameObject exception in exceptions)
                if (menuHierarchy.Peek().name == exception.name)
                    return true;

        return false;
    }
}
