using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Require Controller2D script on gameobject
[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {   
    
    Controller2D controller;
    
    // Final velocity(really this is the calculated movement not velocity) passed to Move() function in the character controller
    Vector3 velocity;
    // The velocity calculated w/ timestep and gravity
    float _velocityY;
    // The previous frame's velocity
    float oldVelocityY;

    // Handled by SmoothDampFunction
    float velocityXSmoothing;

    // Calculated from jump apex, height and horizontal movespeed
    private float gravity;
    // To Do: Change jumpforce to depend on horizontal movement and jump height;
    private float jumpForce => 2 * jumpHeight / timeToJumpApex;

    // Horizontal acceleration time
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;
    
    // Horizontal movement variables
    public float maxHorizontalSpeed = 3f;
    public float horizontalSpeed = .04f;

    public float jumpHeight = 4f;
    public float timeToJumpApex = .225f;
    public float maxVerticalVelocity = 20f;

    public float wallSlideSpeedMax = 7f;
    public float wallStickTime = .25f;
    float timeToWallUnstick;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    
    
    // Start is called before the first frame update
    void Start() {
        // Access 2DController class as controller
        controller = GetComponent<Controller2D> ();
    }

    // Update is called once per frame
    void Update() {
        // Initialize input vector for player movement
        Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (controller.collisions.left) ? -1 : 1;
        
        float targetVelocityX = input.x * horizontalSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne, maxHorizontalSpeed, Time.deltaTime);
        
        bool wallSliding = false; 
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
            
            wallSliding = true;
            
            _velocityY = Mathf.Max(_velocityY, -wallSlideSpeedMax);
            
            if (timeToWallUnstick > 0) {
                velocityXSmoothing = 0;
                velocity.x = 0;
                
                if (input.x != wallDirX && input.x != 0) {
                    timeToWallUnstick -= wallSlideSpeedMax;
                
                } else {
                    timeToWallUnstick = wallStickTime;
                }
            } else {
                timeToWallUnstick = wallStickTime;
            }
        }

        // If space is pressed and player is touching ground apply jump velocity
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (wallSliding) {
                if (wallDirX == input.x) {
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    _velocityY = wallJumpClimb.y;
                    Debug.Log("wallClimb");
                } else if (input.x == 0) {
                    velocity.x = -wallDirX * wallJumpOff.x;
                    _velocityY = wallJumpOff.y;
                    Debug.Log("wallJumpOff");
                } else {
                    velocity.x = -wallDirX* wallLeap.x;
                    _velocityY = wallLeap.y;
                    Debug.Log("wallLeap");
                }
            }

            if (controller.collisions.below) {
                _velocityY = jumpForce;
            }
        }
        
        // Save last calculated _velocity to oldVelocity
        oldVelocityY = _velocityY;
        // Calculate new _velocityY from gravity and timestep
        _velocityY += gravity * Time.deltaTime;
        
        // Apply average oldVelocityY and new _velocityY * timestep
        velocity.y = (oldVelocityY + _velocityY) * .5f * Time.deltaTime;

        // Clamp vertical velocity in between +/- of maxVerticalVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -maxVerticalVelocity, maxVerticalVelocity);

        // Call move function in 2DController
        controller.Move(velocity);

        //If 2DController detects a collision above or below player
        if (controller.collisions.below) {
            // Stop the player from moving
            velocity.y = 0;
            // Prevent gravity from being applied to _velocityY
            gravity = 0;
        } else {

            // If in the air apply gravity
            gravity = -2 * jumpHeight / (timeToJumpApex * timeToJumpApex);
        }

        if (controller.collisions.above) {
            // If player collides with ceiling, set velocity.y to 0 to stop player movement
            velocity.y = 0;
            // Prevent the previous frames velocity being applied to the player (this is to prevent hang time on the ceiling if a jump is interrupted)
            _velocityY = 0;
        }
    }
}
