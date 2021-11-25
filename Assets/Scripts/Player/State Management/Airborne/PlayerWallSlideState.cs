public class PlayerWallSlideState : PlayerBaseState
{
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerWallSlideState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    
    }

    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {
        // start playing animation



    }

    // UpdateState(); is called everyframe inside of the Update(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStateLogic()
    {
        // Check to see if the current state should switch
        CheckSwitchStates();


        Ctx.DebugCurrentState = "WallSlide";
    }

    // UpdateState(); is called everyframe inside of the LateUpdate(); function of the currentContext (PlayerStateMachine.cs)
    public override void UpdateStatePhysics()
    {

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
        if (Ctx.CurrentMovementY < 0 && !(Ctx.Controller2D.collisions.left || Ctx.Controller2D.collisions.right))
        {
            SetSubState(Factory.Falling());
        } 
    }
}