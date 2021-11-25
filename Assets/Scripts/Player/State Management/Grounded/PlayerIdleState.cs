using UnityEngine;
public class PlayerIdleState : PlayerBaseState
{
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    private float _targetVelocityX;
    
    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {
        // Set Idle Animation
        
        Debug.Log("attempted to change to idle");
        Ctx.ChangeAnimationState("Idle");
        Ctx.DebugCurrentState = "Idle";

    }

    // UpdateState(); is called everyframe inside of the Update(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStateLogic()
    {
        // Check to see if the current state should switch
        CheckSwitchStates();

        // set current state string



    }

    // UpdateState(); is called everyframe inside of the LateUpdate(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStatePhysics()
    {
        _targetVelocityX = Ctx.MoveInputVectorX * Ctx.HorizontalSpeed;

        Debug.Log(_targetVelocityX);
        Ctx.CurrentMovementX = Mathf.SmoothDamp(Ctx.CurrentMovementX, _targetVelocityX, ref Ctx.VelocityXSmoothing, Ctx.AccelerationTimeGrounded, Ctx.MaxHorizontalVelocity, Ctx.DeltaTime);
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
        // if player is moving and run is pressed, switch to run state
        if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
        // else if movement is pressed, switch to walk state
        else if (Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Walk());
        }
    }
}