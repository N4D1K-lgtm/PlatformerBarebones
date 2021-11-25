using UnityEngine;
public class PlayerGroundedState : PlayerBaseState
{
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { 
        
        IsRootState = true;
        InitializeSubState();
    }
    
    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {
        
        // Set animation

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
        Ctx.OldVelocityY = 0;
        Ctx.VelocityY = 0;
        Ctx.CurrentMovementY = -.05f;
    }

    // this method is called in SwitchState(); of the parent class before the next state's EnterState() function is called
    public override void ExitState()
    {
        
    }

    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        } else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        } else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SetSubState(Factory.Run());
        }
    }
    
    // called in the current state's UpdateState() method
    public override void CheckSwitchStates()
    {
        // if player is grounded and jump is pressed, switch to jump state
        if (Ctx.IsJumpPressed && !Ctx.RequireJumpPressed)
        {
            SwitchState(Factory.Jump());
        } else if (!Ctx.Controller2D.collisions.below)
        {
            SwitchState(Factory.Airborne());
        }
    }
}
