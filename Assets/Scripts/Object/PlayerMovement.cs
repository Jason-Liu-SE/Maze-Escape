using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed;
    public Rigidbody2D rigidBody;
    public Animator animator;

    // storing the velocity
    public float horizontalComponent = 0f;
    public float verticalComponent = 0f;

    // Update is called once per frame
    void FixedUpdate()
    {   
        horizontalComponent = 0f;
        verticalComponent = 0f;

        // checking the direction of the velocity
        if (InputManager.GetKey("right")) horizontalComponent += 1;
        if (InputManager.GetKey("left")) horizontalComponent -= 1;

        if (InputManager.GetKey("up")) verticalComponent += 1;
        if (InputManager.GetKey("down")) verticalComponent -= 1;

        // checking if both the player is moving in the horizontal and vertical direction
        if (horizontalComponent != 0 && verticalComponent != 0) {
            horizontalComponent *= 0.7071f;
            verticalComponent *= 0.7071f;
        }

        // sending information to animator
        animator.SetFloat("verticalVelocity", verticalComponent);
        animator.SetFloat("horizontalVelocity", Mathf.Abs(horizontalComponent));

        // rotating player orientation
        if (horizontalComponent < 0) transform.localRotation = Quaternion.Euler(0, 180, 0);        // left
        else transform.localRotation = Quaternion.Euler(0, 0, 0);                                  // right

        // moving player
        rigidBody.velocity = new Vector2(horizontalComponent*walkSpeed, verticalComponent*walkSpeed);
    }
}