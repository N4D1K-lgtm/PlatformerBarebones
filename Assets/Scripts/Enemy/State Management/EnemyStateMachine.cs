using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    // State Variables
    EnemyBaseState _currentState;
    EnemyStateFactory _states;


    public EnemyBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }


    void Awake()
    {
        // setup state
        _states = new EnemyStateFactory(this);
        _currentState = _states.Idle();
        _currentState.EnterState();

    }
    // Start is called before the first frame update
    void Start()
    {
        _currentState.UpdateStatesLogic();
        _currentState.UpdateStatesPhysics();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
