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
        
        Ctx.ChangeAnimationState("Idle");
        Ctx.DebugCurrentState = "Idle";

        Ctx.AccumulatedVelocityX -= 0.01f;
        Ctx.TargetDirection = 0;

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

        if (Ctx.LastDirection)
        {

            Ctx.CurrentMovementX = Ctx.CalculateHorizontalMovement(Ctx.AccumulatedVelocityX, 1.2f, 15f) * Ctx.DeltaTime;
            Ctx.AccumulatedVelocityX = Mathf.MoveTowards(Ctx.AccumulatedVelocityX, Ctx.TargetDirection, .1f);
        }
        else if (!Ctx.LastDirection)
        {

            Ctx.CurrentMovementX = Ctx.CalculateHorizontalMovement(Ctx.AccumulatedVelocityX, 1.2f, -15f) * Ctx.DeltaTime;
            Ctx.AccumulatedVelocityX = Mathf.MoveTowards(Ctx.AccumulatedVelocityX, Ctx.TargetDirection, .1f);
        }

        Ctx.AccumulatedVelocityX = Mathf.Max(Ctx.AccumulatedVelocityX, 0);



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
        // if movement is pressed, switch to walk state
        if (Ctx.IsMovementPressed)
            {
                SwitchState(Factory.Walk());
            }
    }
}