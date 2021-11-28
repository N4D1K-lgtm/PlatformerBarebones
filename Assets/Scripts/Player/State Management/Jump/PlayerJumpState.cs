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
        Ctx.ChangeAnimationState("Jump");

        // require new jump press to jump again
        Ctx.RequireJumpPressed = true;

        if (!Ctx.CanWallJump)
        {
            // apply jump velocity
            Ctx.VelocityY = Ctx.InitialJumpVelocity;
        } else
        {
            int _wallDirX = Ctx.Controller2D.collisions.left ? -1 : 1;

            if (_wallDirX == Ctx.MoveInputVectorX)
            {
                Ctx.CurrentMovementX = -_wallDirX * Ctx.WallJumps[2].x;
                Ctx.VelocityY = Ctx.WallJumps[2].y;
                Debug.Log("wallClimb");
            }
            else if (Ctx.MoveInputVectorX == 0)
            {
                Ctx.CurrentMovementX = -_wallDirX * Ctx.WallJumps[1].x;
                Ctx.VelocityY = Ctx.WallJumps[1].y;
                Debug.Log("wallJumpMed");
            }
            else
            {
                Ctx.CurrentMovementX = -_wallDirX * Ctx.WallJumps[0].x;
                Ctx.VelocityY = Ctx.WallJumps[0].y;
                Debug.Log("wallJumpBig");
            }
            Ctx.TimeToWallUnstick = 0;
            Ctx.CanWallJump = false;
        }



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
        //Ctx.OldVelocityY = Ctx.VelocityY;
        // Apply average of OldVelocityY and new VelocityY * Timestep
        Ctx.CurrentMovementY = Ctx.VelocityY * Ctx.DeltaTime + .5f * Ctx.Gravity * Ctx.DeltaTime * Ctx.DeltaTime;
        // Calculate new _velocityY from gravity and timestep
        Ctx.VelocityY += (Ctx.Gravity * Ctx.DeltaTime);
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
