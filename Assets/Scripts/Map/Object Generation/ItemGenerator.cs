using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour {
    // these fields will indicate the likely hood of a certain item spawning.
    // a lower factor will indicate a lower chance to spawn
    [Header("Spawning Rates")]
    [Min(0)] public int holeRatio = 0;
    [Min(0)] public int mapRatio = 0;
    [Min(0)] public int batteryRatio = 0;
    [Min(0)] public int trapRatio = 0;
    [Min(0)] public int speedRatio = 0;

    // these public variables will hold sprites that correspond to their item
    [Header("Item Sprites")]
    public Sprite holeSprite;
    public Sprite mapSprite;
    public Sprite batterySprite;
    public Sprite trapSprite;
    public Sprite speedSprite;

    // elements will be randomly chosen from this list in order to determine which items to spawn
    private List<string> weightedItems = new List<string>();
    [Header("Other Dependencies")]
    public InventoryManager invManager;

    // determining which items to spawn and where to spawn them
    public void GenerateItems(List<Vector2> spawningLocations) {
        // deleting all the previous items
        foreach (Transform child in transform.gameObject.GetComponentsInChildren<Transform>())
            if (!child.name.Equals(transform.name))
                Destroy(child.gameObject); 

        // variabled used to hold random locations and numbers
        int randItem = 0;

        // filling the weightedItems list with items
        FillWeightedItems();

        // spawning items until there are no more spawning locations
        while (spawningLocations.Count > 0 && weightedItems.Count > 0) {
            // Checking if the weightedItems list only contains hole objects. 
            // Since holes need 2 location, if only 1 location is left, an infinite loop will occur
            if (weightedItems.Count == holeRatio && spawningLocations.Count == 1)
                break;

            // choosing a random item to spawn
            randItem = (int)Random.Range(0, weightedItems.Count);

            // spawning the chosen item
            SpawnItem(weightedItems[randItem], spawningLocations);
        }
    }

    // filling the weightedItems list with items
    private void FillWeightedItems() {
        // placing each item into the same list, appearing as many times as specified by their ratios
        AddItem("hole", holeRatio);
        AddItem("map", mapRatio);
        AddItem("battery", batteryRatio);
        AddItem("trap", trapRatio);
        AddItem("speed", speedRatio);
    }

    // adding items to the weightedItems list
    private void AddItem(string name, int appearances) {
        for (int iteration = 0; iteration < appearances; iteration++) 
            weightedItems.Add(name);
    } 

    // this function will handle the spawning of items. The List<Vector2> spawnLocations must be passed in, rather than 
    // determining the location beforehand because some function will require more than one location. 
    // eg: the hole item will need an entrance and exit hole
    private void SpawnItem(string itemName, List<Vector2> spawningLocations) {
        // determining which item was chosen and executing the appropriate function
        if (itemName.Equals("hole") && spawningLocations.Count > 1)
            GenerateHole(spawningLocations);
        else if (itemName.Equals("map"))
            GenerateMap(spawningLocations);
        else if (itemName.Equals("battery"))
            GenerateBattery(spawningLocations);
        else if (itemName.Equals("trap"))
            GenerateTrap(spawningLocations);
        else if (itemName.Equals("speed"))
            GenerateSpeed(spawningLocations);
    }

    // this function will return a Vector2, containing a randomly selectecd spawning location
    private Vector2 GetRandomLocation(List<Vector2> spawningLocations) {
        int randLocation = Random.Range(0, spawningLocations.Count);

        return spawningLocations[randLocation];
    }




    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////// Functions to Generate Each Item /////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    // this function will generate hole objects (exit and entrance holes)
    private void GenerateHole(List<Vector2> spawningLocations) {
        Vector2 location1 = GetRandomLocation(spawningLocations);
        spawningLocations.Remove(location1);

        Vector2 location2 = GetRandomLocation(spawningLocations);
        spawningLocations.Remove(location2);

        GameObject holes = LoadPrefab("Holes", new Vector2(0, 0));

        // moving the children holes in the holes prefab
        holes.transform.GetComponentsInChildren<Transform>()[0].position = new Vector3(location1.x+0.5f, location1.y+0.5f, 0);
        holes.transform.GetComponentsInChildren<Transform>()[1].position = new Vector3(location2.x+0.5f, location2.y+0.5f, 0);
    }
    
    // this function will generate a map object
    private void GenerateMap(List<Vector2> spawningLocations) {
        Vector2 location = GetRandomLocation(spawningLocations);

        GameObject prefab = LoadPrefab("Map", location); 

        prefab.GetComponent<MapManager>().invManager = invManager;

        // indicating that an object has been spawned at the given location
        spawningLocations.Remove(location);
    }

    // this function will generate a battery object
    private void GenerateBattery(List<Vector2> spawningLocations) {
        Vector2 location = GetRandomLocation(spawningLocations);

        // creating the battery item
        LoadPrefab("Battery", location);

        // indicating that an object has been spawned at the given location
        spawningLocations.Remove(location);
    }

    // this function will generate a speed object
    private void GenerateSpeed(List<Vector2> spawningLocations) {
        Vector2 location = GetRandomLocation(spawningLocations);

        // creating the adrenaline item
        LoadPrefab("Speed", location);

        // indicating that an object has been spawned at the given location
        spawningLocations.Remove(location);
    }

    // this function will generate a trap object
    private void GenerateTrap(List<Vector2> spawningLocations) {
        Vector2 location = GetRandomLocation(spawningLocations);

        /*
        LoadPrefab("Trap", location);
*/
        // indicating that an object has been spawned at the given location
        spawningLocations.Remove(location);
    }

    /////////////////////////////////////////
    /////// Helper Functions ////////////////
    /////////////////////////////////////////
    /*
    private void SetupGenericObject(GameObject genericObject, Sprite genericSprite, Vector2 location, Transform parent) {
        // setting the parent of the item
        genericObject.transform.SetParent(parent);

        // adding components to the item
        genericObject.AddComponent<SpriteRenderer>();           
        genericObject.AddComponent<BoxCollider2D>();

        // editing the components on the item
        genericObject.GetComponent<SpriteRenderer>().sprite = genericSprite;
        genericObject.GetComponent<SpriteRenderer>().sortingOrder = 2;
        genericObject.GetComponent<BoxCollider2D>().isTrigger = true;
        genericObject.GetComponent<BoxCollider2D>().size = new Vector2(genericObject.GetComponent<SpriteRenderer>().bounds.size.x, genericObject.GetComponent<SpriteRenderer>().bounds.size.y);

        // moving the item
        genericObject.transform.position = new Vector3(location.x+0.5f, location.y+0.5f, transform.position.z);
    }*/

    // given the name of a prefab and a location to spawn the prefab, this function will load and place that prefab 
    // the loaded prefab will be returned
    private GameObject LoadPrefab(string prefabName, Vector2 location) {
        GameObject prefab = Resources.Load("Prefabs/" + prefabName) as GameObject;

        if (prefab != null) {
            GameObject item = Instantiate(prefab, new Vector3(location.x+0.5f, location.y+0.5f, 0), Quaternion.identity); 

            item.transform.SetParent(transform);

            return item;
        }

        return null;
    }
}
