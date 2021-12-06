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

        // set current state string
        Ctx.DebugCurrentState = "Jump";

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
                Ctx.VelocityX = -_wallDirX * Ctx.WallJumps[0].x;
                Ctx.VelocityY = Ctx.WallJumps[0].y;
                
            }
            else if (Ctx.MoveInputVectorX == 0)
            {
                Ctx.VelocityX = -_wallDirX * Ctx.WallJumps[1].x;
                Ctx.VelocityY = Ctx.WallJumps[1].y;
                
            }
            else
            {
                Ctx.VelocityX = -_wallDirX * Ctx.WallJumps[2].x;
                Ctx.VelocityY = Ctx.WallJumps[2].y;
                
            }
            Ctx.TimeToWallUnstick = 0;
            Ctx.CanWallJump = false;
        }

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
        /*if (Ctx.MoveInputVectorX > 0f)
        {
            if (!Ctx.LastDirection)
            {
                Ctx.AccumulatedVelocityX = 0;
                Ctx.LastDirection = true;
            }
            Ctx.CurrentMovementX = Ctx.CalculateHorizontalMovement(Ctx.AccumulatedVelocityX, 10f, 0.025f);
            Ctx.AccumulatedVelocityX += 0.01f;

            Debug.Log(Ctx.CurrentMovementX);
        }
        else if (Ctx.MoveInputVectorX < 0f)
        {
            if (Ctx.LastDirection)
            {
                Ctx.AccumulatedVelocityX = 0;
                Ctx.LastDirection = false;
            }

            Ctx.CurrentMovementX = Ctx.CalculateHorizontalMovement(Ctx.AccumulatedVelocityX, 10f, -0.025f);
            Ctx.AccumulatedVelocityX += 0.01f;
        }
        else if (Ctx.MoveInputVectorX == 0)
        {
            if (Ctx.LastDirection)
            {
                Ctx.CurrentMovementX = Ctx.CalculateHorizontalMovement(Ctx.AccumulatedVelocityX, 10f, 0.025f);
                Ctx.AccumulatedVelocityX -= 0.01f;
                Ctx.AccumulatedVelocityX = Mathf.Max(Ctx.AccumulatedVelocityX, 0);
            }
            else if (!Ctx.LastDirection)
            {
                Ctx.CurrentMovementX = Ctx.CalculateHorizontalMovement(Ctx.AccumulatedVelocityX, 10f, -0.025f);
                Ctx.AccumulatedVelocityX -= 0.01f;
                Ctx.AccumulatedVelocityX = Mathf.Max(Ctx.AccumulatedVelocityX, 0);
            }
        }*/

        Ctx.CurrentMovementY = Ctx.VelocityY * Ctx.DeltaTime + .5f * Ctx.Gravity * Ctx.DeltaTime * Ctx.DeltaTime;
        Ctx.CurrentMovementX = Ctx.VelocityX * Ctx.DeltaTime + .5f * Ctx.DeltaTime * Ctx.DeltaTime;

        // Calculate new _velocityY from gravity and timestep
        Ctx.VelocityY += (Ctx.Gravity * Ctx.DeltaTime);
        Ctx.VelocityX += (Ctx.Acceleration * Ctx.DeltaTime);

        // Clamp velocity in between +/- of maxVelocity;
        Ctx.CurrentMovementY = Mathf.Clamp(Ctx.CurrentMovementY, -Ctx.MaxVerticalVelocity, Ctx.MaxVerticalVelocity);
        Ctx.CurrentMovementX = Mathf.Clamp(Ctx.CurrentMovementX, -Ctx.MaxHorizontalVelocity, Ctx.MaxHorizontalVelocity);

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
