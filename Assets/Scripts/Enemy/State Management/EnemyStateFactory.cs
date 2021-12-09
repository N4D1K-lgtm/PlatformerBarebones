public class EnemyStateFactory
{
    // Set context to type EnemyStateMachine
    EnemyStateMachine _context;

    // constructor method with currentContext of type EnemyStateMachine
    public EnemyStateFactory(EnemyStateMachine currentContext)
    {
        _context = currentContext;
    }

    // call constructor methods of the following concrete states, passing in EnemyStateMachine as context and this EnemyStateFactory class
    // To Do: find a way to do this without repeating code and creating a new instance of each concrete class everytime the state is switchsed (avoid garbage collection)
    public EnemyBaseState Idle()
    {
        return new EnemyIdleState(_context, this);
    }

    public EnemyBaseState Patrol()
    {
        return new EnemyPatrolState(_context, this);
    }

    public EnemyBaseState Attack()
    {
        return new EnemyAttackState(_context, this);
    }

}
