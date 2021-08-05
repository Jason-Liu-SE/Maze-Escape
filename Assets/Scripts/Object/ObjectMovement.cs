using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script will move an object based on a set of directions/instructions
// Eg: Instructions from the path finding algorithm
public class ObjectMovement : MonoBehaviour {
    // movement
    public float walkSpeed = 0f;

    // storing the velocity
    [HideInInspector] public float horizontalComponent = 0f;
    [HideInInspector] public float verticalComponent = 0f;

    public Animator animator;

    [SerializeField] private AManager aStarManager;

    private Queue<Node> movementQueue = new Queue<Node>();  // stores the movement instructions for the object
    private Node target;                                    // the location that the object is trying to reach   
    private float timer = 0f;                               // counting the amount of time that has passed since the last time it was reset              

    
    void Update() {
        timer += Time.deltaTime;

        // there are no more instructions, or the timer has reached half a second
        if (target == null || timer > 0.5f) {     
            // clearing the queue
            movementQueue.Clear();

            // resetting the timer
            timer = 0f;

            // checking if the object has stopped moving
            if (target == null) {
                // code to get a new path from the aStarManager
                aStarManager.GenerateAStarPath();               // generating a new path
                movementQueue = GetPathFindingInstructions();   // getting the path
                
                // if aStarManager returned a path, get the first node in that path
                if (movementQueue.Count > 0) target = movementQueue.Dequeue();
            }
        } else {      // there are actions/instructions to be carried out
            // move the object until it has reached its target destination
            if ((transform.position.x != target.x || transform.position.y != target.y) && target != null) {
                // moving the object towards the target
                MoveTo(new Vector2(target.x, target.y));
            }

            // once it has reached its target destination, get the next action/instruction
            else {
                // setting the target to null
                target = null;

                // checking if the queue has more instructions. If so, get the next action
                if (movementQueue.Count > 0) target = movementQueue.Dequeue();
            }
        }
    }

    // this will call the get path function in the AManager script
    // a set of directions to reach the target of the pathfinding algorithm will be returned
    public Queue<Node> GetPathFindingInstructions() {
        return ConvertNodeListToQueue(aStarManager.GetPath());
    }

    // giving a list containing nodes, this function will convert that list into a corresponding node queue
    private Queue<Node> ConvertNodeListToQueue(List<Node> list) {
        Queue<Node> queue = new Queue<Node>();

        foreach (Node element in list) queue.Enqueue(element);
        
        return queue;
    }

    // this functions moves the object to the target position
    private void MoveTo(Vector2 target) {
        // ensuring that the walk speed is greater than 0
        if (walkSpeed < 0) walkSpeed = Mathf.Abs(walkSpeed);

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.x, target.y, transform.position.z), walkSpeed*Time.deltaTime);
    }
}