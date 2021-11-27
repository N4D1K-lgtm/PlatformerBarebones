using UnityEngine;
public class PlayerRunState : PlayerBaseState
{
    private float _targetVelocityX;
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {
        Ctx.ChangeAnimationState("Run");
    }

    // UpdateState(); is called everyframe inside of the Update(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStateLogic()
    {
        // Check to see if the current state should switch
        CheckSwitchStates();

        // set current state string
        Ctx.DebugCurrentState = "Run";

        

    }

    // UpdateState(); is called everyframe inside of the LateUpdate(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStatePhysics()
    {
        _targetVelocityX = Ctx.MoveInputVectorX * Ctx.HorizontalSpeed;

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
        // if movement is not pressed, switch to Idle state
        if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        // else if movement is pressed and run is not pressed, switch to Walk state
        } else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
    }
}