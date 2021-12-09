using UnityEngine;
using System.Collections;
public class EnemyPatrolState : EnemyBaseState
{

    // create a public constructor method with currentContext of type EnemyStateMachine, factory of type EnemyStateFactory
    // and pass this to the base state constructor
    public EnemyPatrolState(EnemyStateMachine currentContext, EnemyStateFactory EnemyStateFactory) : base(currentContext, EnemyStateFactory)
    {
        IsRootState = true;
    }

    // this method is called in SwitchState(); of the parent class after the last state's ExitState() function was called
    public override void EnterState()
    {


    }

    // UpdateState(); is called everyframe inside of the Update(); function of the currentContext (EnemyStateMachine.cs)
    public override void UpdateStateLogic()
    {
        // Check to see if the current state should switch
        CheckSwitchStates();

    }

    // UpdateState(); is called everyframe inside of the LateUpdate(); function of the currentContext (EnemyStateMachine.cs)
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

    }
}