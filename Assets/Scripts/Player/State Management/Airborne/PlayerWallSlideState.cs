﻿using UnityEngine;
public class PlayerWallSlideState : PlayerBaseState
{
    // create a public constructor method with currentContext of type PlayerStateMachine, factory of type PlayerStateFactory
    // and pass this to the base state constructor
    public PlayerWallSlideState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    {
    
    }

    private int _wallDirX;
    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {
        Ctx.DebugCurrentState = "WallSlide";

        _wallDirX = Ctx.Controller2D.collisions.left ? -1 : 1;

        // start playing animation
        Ctx.ChangeAnimationState("WallSlide");

        if (_wallDirX == 1)
        {
            // dCtx.SpriteRenderer.flipX = true;
            //Ctx.Transform.localScale = new Vector3(-1, 1, 1);

        }
        else if (_wallDirX == -1)
        {
            // Ctx.SpriteRenderer.flipX = false;
            //Ctx.Transform.localScale = new Vector3(1, 1, 1);

        }

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
        Ctx.CurrentMovementY = Ctx.WallSlideSpeed;
        Debug.Log(Ctx.CurrentMovementY);
        if (Ctx.TimeToWallUnstick > 0)
        {
            Ctx.VelocityXSmoothing = 0;
            Ctx.CurrentMovementX = -0.0001f * _wallDirX;

            if (Ctx.MoveInputVectorX != _wallDirX && Ctx.MoveInputVectorX != 0)
            {
                Ctx.TimeToWallUnstick -= Ctx.DeltaTime;

            }
            else
            {
                Ctx.TimeToWallUnstick = Ctx.WallStickTime;
            }
        }
        else
        {
            Ctx.TimeToWallUnstick = Ctx.WallStickTime;
        }

    }

    // this method is called in SwitchState(); of the parent class before the next state's EnterState() function is called
    public override void ExitState()
    {
        Ctx.VelocityY = 0;
        Ctx.OldVelocityY = 0;
        Ctx.CanWallJump = true;
    }

    public override void InitializeSubState()
    {

    }

    // called in the current state's UpdateState() method
    public override void CheckSwitchStates()
    {
        if (!Ctx.Controller2D.collisions.left && !Ctx.Controller2D.collisions.right)
        {
            SwitchState(Factory.Falling());
        } 
    }
}