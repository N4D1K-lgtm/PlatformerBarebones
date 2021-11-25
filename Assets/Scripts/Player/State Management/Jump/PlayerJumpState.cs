using UnityEngine;
public class PlayerJumpState : PlayerBaseState
{
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { 
    IsRootState = true;
    }

    private float _targetPosX;

    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {
        // start animation

        // require new jump press to jump again
        Ctx.RequireJumpPressed = true;

        // apply jump velocity
        Ctx.VelocityY = Ctx.InitialJumpForce;


        // set current state string
        Ctx.DebugCurrentState = "Jump";
    }

    // UpdateState(); is called everyframe inside of the Update(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStateLogic()
    {
        // check to see if the current state should switch
        CheckSwitchStates();

    }

    // UpdateState(); is called everyframe inside of the LateUpdate(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStatePhysics()
    {
        _targetPosX = Ctx.MoveInputVectorX * Ctx.HorizontalSpeed;

        Ctx.CurrentMovementX = Mathf.SmoothDamp(Ctx.CurrentMovementX, _targetPosX, ref Ctx.VelocityXSmoothing, Ctx.AccelerationTimeAirborne, Ctx.MaxHorizontalVelocity, Ctx.DeltaTime);

        // Save last calculated _velocity to oldVelocity
        Ctx.OldVelocityY = Ctx.VelocityY;
        // Calculate new _velocityY from gravity and timestep
        Ctx.VelocityY += (Ctx.Gravity * Ctx.DeltaTime);        
        // Apply average of OldVelocityY and new VelocityY * Timestep
        Ctx.CurrentMovementY = (Ctx.OldVelocityY + Ctx.VelocityY) * .5f;
        // Clamp vertical velocity in between +/- of maxVerticalVelocity;
        Ctx.CurrentMovementY = Mathf.Clamp(Ctx.CurrentMovementY, -Ctx.MaxVerticalVelocity, Ctx.MaxVerticalVelocity);

    }

    // this method is called in SwitchState(); of the parent class before the next state's EnterState() function is called
    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {

    }

    // called in the current state's UpdateState() method
    public override void CheckSwitchStates()
    {
        if (Ctx.Controller2D.collisions.below)
        {
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.CurrentMovementY <= 0 || Ctx.Controller2D.collisions.above) 
        {
            SwitchState(Factory.Airborne());
        } 
    }
}