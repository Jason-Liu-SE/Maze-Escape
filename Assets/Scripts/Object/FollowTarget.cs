using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour {
    public Transform target;

    public Vector3 offset;

    [Range(0.0f, 1.0f)]
    public float lerpSpeed = 0f;

    public float cameraSpeed = 0f;

    public bool lerp = true;

    // Start is called before the first frame update
    void Start() {
        if (target == null) 
            return;

        // Moves to the player's location
        transform.position = new Vector3(target.position.x - offset.x, target.position.y + offset.y, transform.position.z);
    }

    // Update is called once per frame
    void Update() {
        if (target == null) 
            return;

        if (lerp)
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x - offset.x, target.position.y + offset.y, transform.position.z), lerpSpeed);
        else 
            transform.position = new Vector3(target.position.x - offset.x, target.position.y + offset.y, transform.position.z);
    }
}
