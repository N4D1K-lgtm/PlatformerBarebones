using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Require Controller2D script on gameobject
[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour
{   
    // Final velocity(really this is the calculated movement not velocity) passed to Move() function in the character controller
    Vector3 velocity;

    // The velocity calculated w/ timestep and gravity
    float _velocity;

    // The previous frame's velocity
    float oldVelocity;

    Controller2D controller;

    // Handled by smooth damp funtion
    float velocityXSmoothing;

    // Horizontal acceleration time, lower is faster.
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;

    // Move speed, total height of player jump and total time to jump
    public float moveSpeed = 6;
    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    
    public float maxVerticalVelocity = .1f;
    public float maxHorizontalVelocity = 20f;
    float verticalVelocity;

    // Calculated from jump apex, height and movespeed
    private float gravity;
    private float jumpForce => 2 * jumpHeight / timeToJumpApex;

    bool grounded;
    
    

    // Start is called before the first frame update
    void Start() {
        
        // Access 2DController script as controller
        controller = GetComponent<Controller2D> ();

    }

    // Update is called once per frame
    void Update() {
        
        // Initialize input vector for player movement
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Target horizontal velocity for Smooth Damping
        float targetVelocityX = input.x * moveSpeed;
        
        // SmoothDamp(current pos, target pos, current velocity, smooth time, maxSpeed, deltaTime)
        velocity.x = targetVelocityX * Time.deltaTime;
        //Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne, maxHorizontalVelocity, Time.deltaTime);
        

        // If space is pressed and player is touching ground apply jump velocity
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below) {
            _velocity = jumpForce;
        }
        
        // Save last calculated _velocity to oldVelocity
        oldVelocity = _velocity;
        // Calculate new _velocity from gravity and timestep
        _velocity += gravity * Time.deltaTime;
        // Apply Average oldVelocity and new _velocity * timestep
        velocity.y = (oldVelocity + _velocity) * .5f * Time.deltaTime; 
        
        // An attempt to limit vertical velocity but doesnt seem to work
        Mathf.Clamp(velocity.y, -maxVerticalVelocity, maxVerticalVelocity);
        
        // Call move function in 2DController
        controller.Move(velocity);

        // Print Velocity
        Debug.Log(velocity);
        
        // Print if player is on slope
        Debug.Log(controller.collisions.climbingSlope);

        //If 2DController detects a collision above or below player
        if (controller.collisions.below) {
            // Stop the player from moving
            velocity.y = 0;
            // Prevent gravity from being applied to _velocity
            gravity = 0;
        } else {

            // If in the air apply gravity
            gravity = -2 * jumpHeight / (timeToJumpApex * timeToJumpApex);
        }

        if(controller.collisions.above) {
            // If player collides with ceiling, set velocity.y to 0 to stop player movement
            velocity.y = 0;
            // Prevent the previous frames velocity being applied to the player (this is to prevent hang time on the ceiling if a jump is interrupted)
            _velocity = 0;
        }
    }
}
