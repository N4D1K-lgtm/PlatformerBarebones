using UnityEngine;
public class PlayerAirborneState : PlayerBaseState
{
    private float _targetPosX;
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerAirborneState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
        InitializeSubState();
        
    }

    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {
    }

    // UpdateState(); is called everyframe inside of the Update(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStateLogic()
    {
        // Check to see if the current state should switch
        CheckSwitchStates();

        
    }

    // UpdateState(); is called everyframe inside of the LateUpdate(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStatePhysics()
    {
        _targetPosX = Ctx.MoveInputVectorX * Ctx.HorizontalSpeed;

        Ctx.CurrentMovementX = Mathf.SmoothDamp(Ctx.CurrentMovementX, _targetPosX, ref Ctx.VelocityXSmoothing, Ctx.AccelerationTimeAirborne, Ctx.MaxHorizontalVelocity, Ctx.DeltaTime);

        if (Ctx.Controller2D.collisions.above)
        {
            //If player collides with ceiling, set velocity.y to 0 to stop player movement
            Ctx.OldVelocityY = 0;
            // Prevent the previous frames velocity being applied to the player (this is to prevent hang time on the ceiling if a jump is interrupted)
            Ctx.VelocityY = 0;

        }

        // Save last calculated _velocity to oldVelocity
        //Ctx.OldVelocityY = Ctx.VelocityY;
        // Apply average of OldVelocityY and new VelocityY * Timestep
        Ctx.CurrentMovementY = Ctx.VelocityY * Ctx.DeltaTime + .5f * Ctx.Gravity * Ctx.DeltaTime * Ctx.DeltaTime;
        // Calculate new _velocityY from gravity and timestep
        Ctx.VelocityY += (Ctx.Gravity * Ctx.DeltaTime);
        // Clamp vertical velocity in between +/- of maxVerticalVelocity;
        Ctx.CurrentMovementY = Mathf.Clamp(Ctx.CurrentMovementY, -Ctx.MaxVerticalVelocity, Ctx.MaxVerticalVelocity);

        /*if (Ctx.CurrentMovementX < 0)
        {
            Ctx.Transform.localScale = new Vector3(-1, 1, 1);


        }
        else if (Ctx.CurrentMovementX > 0)
        {
            Ctx.Transform.localScale = new Vector3(1, 1, 1);

        }*/

    }

    // this method is called in SwitchState(); of the parent class before the next state's EnterState() function is called
    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {
        if (Ctx.CurrentMovementY <= 0 && (Ctx.Controller2D.collisions.left || Ctx.Controller2D.collisions.left))
        {
            SetSubState(Factory.WallSlide());
        } 
        else if (Ctx.CurrentMovementY < 0 && (!Ctx.Controller2D.collisions.left || !Ctx.Controller2D.collisions.right)) {
            SetSubState(Factory.Falling());
        }
    }
   
    // called in the current state's UpdateState() method
    public override void CheckSwitchStates()
    {
        // if player is grounded and jump is pressed, switch to jump state
        if (Ctx.Controller2D.collisions.below)
        {
            SwitchState(Factory.Grounded());
        } else if (Ctx.IsJumpPressed && (Ctx.Controller2D.collisions.left || Ctx.Controller2D.collisions.right) && !Ctx.RequireJumpPressed && Ctx.CurrentMovementY <= 0)
        {
            SwitchState(Factory.Jump());
        }
    }
}
