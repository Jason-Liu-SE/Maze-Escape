using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotation : MonoBehaviour 
{
    public Transform target;
    public PlayerMovement playerMovement;

    private Vector3 orientation;

    void Update()
    {
        if (target == null) return;
        
        float x = playerMovement.horizontalComponent;
        float y = playerMovement.verticalComponent;

        // storing current orientation
        if (x!=0f || y != 0f) orientation = target.rotation.eulerAngles;

        // checking player orientation
        if (x < 0 && y < 0) orientation.z = 225;           // diagonal
        else if (x > 0 && y < 0) orientation.z = -135;
        else if (x < 0 && y > 0) orientation.z = -45;
        else if (x > 0 && y > 0) orientation.z = -45;

        else if (x < 0) orientation.z = -90;            // left, right, down, up
        else if (x > 0) orientation.z = -90;
        else if (y < 0) orientation.z = 180;
        else if (y > 0) orientation.z = 0;
        
        transform.rotation = Quaternion.Euler(orientation);
    }
}
