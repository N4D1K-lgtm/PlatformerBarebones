using UnityEngine;
public class PlayerWalkState : PlayerBaseState
{
    private float _targetVelocityX;
    
    float _accumulatedVelocityX;
    
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) {
        IsRootState = false;
    }

    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState() {
        Ctx.ChangeAnimationState("Run");

        Ctx.DebugCurrentState = "Walk";
        Ctx.TargetDirection = 1;


    }

    // UpdateState(); is called everyframe inside of the Update(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStateLogic()
    {
        // Check to see if the current state should switch
        CheckSwitchStates();

    }

    // UpdateState(); is called everyframe inside of the LateUpdate(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStatePhysics() {

        if (Ctx.MoveInputVectorX > 0f) 
        {   
            if (!Ctx.LastDirection)
            {
                Ctx.AccumulatedVelocityX = 0;
                Ctx.LastDirection = true;
            }
            Ctx.CurrentMovementX = Ctx.CalculateHorizontalMovement(Ctx.AccumulatedVelocityX, 1.2f, 15f) * Ctx.DeltaTime;
            Ctx.AccumulatedVelocityX = Mathf.MoveTowards(Ctx.AccumulatedVelocityX, Ctx.TargetDirection, .1f);

            Debug.Log(Ctx.CurrentMovementX);
        } else if (Ctx.MoveInputVectorX < 0f )
        {
            if (Ctx.LastDirection)
            {
                Ctx.AccumulatedVelocityX = 0;
                Ctx.LastDirection = false ;
            }

            Ctx.CurrentMovementX = Ctx.CalculateHorizontalMovement(Ctx.AccumulatedVelocityX, 1.2f, -15f) * Ctx.DeltaTime;
            Ctx.AccumulatedVelocityX = Mathf.MoveTowards(Ctx.AccumulatedVelocityX, Ctx.TargetDirection, .1f);
        }
        
    }

    // this method is called in SwitchState(); of the parent class before the next state's EnterState() function is called
    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {

    }

    // called in the current state's UpdateState() method
    public override void CheckSwitchStates() {

        
        if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        // else if movement is still pressed and run is pressed, switch to run
        }
    }
}