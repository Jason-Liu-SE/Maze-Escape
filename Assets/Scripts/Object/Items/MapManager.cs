using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
    [HideInInspector]
    public InventoryManager invManager;

    void OnTriggerEnter2D() {
        if (!invManager.GetItem("Map")) {
            invManager.AddItem("Map");
            Destroy(transform.gameObject);
        }
    } 
}
