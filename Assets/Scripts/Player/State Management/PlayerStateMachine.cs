using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class PlayerStateMachine : MonoBehaviour
{
    private Controller2D controller2D;
    private Collider2D collider2D;
    private Animator animator;
    private PlayerActionControls playerActionControls;
    private SpriteRenderer spriteRenderer;
    private Transform transform;

    public float VelocityXSmoothing;

    [SerializeField]
    private float _accelerationTimeAirborne = .2f;
    [SerializeField]
    private float _accelerationTimeGrounded = .1f;
    [SerializeField]
    private float _maxHorizontalVelocity = 3f;
    [SerializeField]
    private float _horizontalSpeed = 0.025f;
    [SerializeField]
    private float _jumpHeight = 0.01f;
    [SerializeField]
    private float _timeToJumpApex = .4f;
    [SerializeField]
    private float _maxVerticalVelocity = 20f;
    [SerializeField]
    private float _wallSlideSpeed = -.005f;
    [SerializeField]
    private float _wallStickTime = .25f;
    [SerializeField]
    private float _dashTime = .25f;
    [SerializeField]
    private float _dashDistance = .25f;
    [SerializeField]
    private float _rollTime = .5f;
    [SerializeField]
    private float _rollDistance = .01f;

    public readonly Vector2[] WallJumps = { new Vector2(0.03f, 10f), new Vector2(0.035f, 13.5f), new Vector2(0.04f, 17f) };

    // Animation Strings
    private string[] _animationStates = new string[] { "Idle", "Run", "Jump", "Fall", "Roll", "Dash", "Wall_Grab", "Wall_Slide_Right", "Wall_Slide_Left", "Attack_1", "Attack_2", "Attack_3", "Special_Attack", };
    private Dictionary<string, int> _animationStatesDict = new Dictionary<string, int>();

   
    // Jump height and force variables
    // To Do: Change jumpForce to depend on horizontal movement and jump height;
    private bool _isJumpPressed;
    private bool _isMovementPressed;
    private bool _isRollDashPressed;
    private bool _isDashFinished;
    private bool _isRollFinished;
    private bool _isJumpEnabled;
    private bool _requireJumpPressed;
    private bool _requireRollDashPressed;
    private bool _canWallJump;
    private bool _canDash;
    private bool _lastDirection;
    private Vector3 _velocity;
    private Vector3 _currentMovement;
    private Vector2 _moveInputVector;
    private float _initialJumpVelocity;
    private float _accumulatedVelocityX;
    private float _timeToWallUnstick;
    private float _rollSpeed;
    private float _rollFrameTime;
    private float _dashSpeed;
    private float _gravity;
    private float _timeScale;
    private float _deltaTime;
    private string _debugCurrentState;
    private int _currentAnimationStateHash;

    // Nates Variables
    float jumpHeight;
    float v_x;
    float x_h;

    // State Variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // Getters and Setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Controller2D Controller2D { get { return controller2D; } }
    public Transform Transform { get { return transform; } set { transform = value; } }
    public Animator Animator { get { return animator; } }
    public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } }
    
    public float VelocityY { get { return _velocity.y; } set { _velocity.y = value; } }
    public float VelocityX { get { return _velocity.x; } set { _velocity.x = value; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float CurrentMovementX { get { return _currentMovement.x; } set { _currentMovement.x = value; } }
    public float MoveInputVectorY { get { return _moveInputVector.y; } set { _moveInputVector.y = value; } }
    public float MoveInputVectorX { get { return _moveInputVector.x; } set { _moveInputVector.x = value; } }
    public float MaxVerticalVelocity { get { return _maxVerticalVelocity; } }
    public float MaxHorizontalVelocity { get { return _maxHorizontalVelocity; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
    public float WallSlideSpeed { get { return _wallSlideSpeed; } }
    public float TimeToWallUnstick { get { return _timeToWallUnstick; } set { _timeToWallUnstick = value;} }
    public float WallStickTime { get { return _wallStickTime; } }
    public float Gravity { get { return _gravity; } }
    public float HorizontalSpeed { get { return _horizontalSpeed; } }
    public float AccumulatedVelocityX { get { return _accumulatedVelocityX; } set { _accumulatedVelocityX = value; } }
    public float AccelerationTimeGrounded { get { return _accelerationTimeGrounded;} }
    public float AccelerationTimeAirborne { get { return _accelerationTimeAirborne;} }
    public float DashTime { get { return _dashTime; } }
    public float RollTime { get { return _rollTime; } }
    public float DashSpeed { get { return _dashSpeed; } }
    public float RollSpeed { get { return _rollSpeed; } }
    public float RollFrameTime { get { return _rollFrameTime; } }
    public float TimeScale { get { return _timeScale; } set { _timeScale = value; } }
    public float DeltaTime { get { return _deltaTime; } }
    
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsRollDashPressed { get { return _isRollDashPressed; } }
    public bool IsRollFinished { get { return _isRollFinished; } set { _isRollFinished = value; } }
    public bool IsDashFinished { get { return _isDashFinished; } set { _isDashFinished = value; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool RequireJumpPressed { get { return _requireJumpPressed; } set { _requireJumpPressed = value; } }
    public bool RequireRollDashPressed { get { return _requireRollDashPressed; } set { _requireRollDashPressed = value; } }
    public bool CanWallJump { get { return _canWallJump; } set { _canWallJump = value; } }
    public bool CanDash { get { return _canDash; } set { _canDash = value; } }
    public bool LastDirection { get { return _lastDirection; } set { _lastDirection = value; } }
    
    public string DebugCurrentState { get { return _debugCurrentState; } set { _debugCurrentState = value; } }
    
    public Dictionary<string, int> AnimationStatesDict { get { return _animationStatesDict; } }


    void Awake()
    {
        // set initial references
        playerActionControls = new PlayerActionControls();
        controller2D = GetComponent<Controller2D>();
        collider2D = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform = GetComponent<Transform>();

        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();


        // initialize input action callbacks
        playerActionControls.Gameplay.Jump.performed += context => OnJump(context);
        playerActionControls.Gameplay.Jump.canceled += context => OnJump(context);
        playerActionControls.Gameplay.Move.performed += context => OnMove(context);
        playerActionControls.Gameplay.Move.canceled += context => OnMove(context);
        playerActionControls.Gameplay.RollDash.performed += context => OnRollDash(context);
        playerActionControls.Gameplay.RollDash.canceled += context => OnRollDash(context);

        // misc variables
        _jumpHeight = 2.5f;
        v_x = 1f;
        x_h = .25f;
        _initialJumpVelocity = (2f * _jumpHeight * v_x) / x_h;
        _gravity = (-2f * _jumpHeight * v_x * v_x) / (x_h * x_h);
        _timeScale = 1;


        _dashSpeed = _dashDistance / _dashTime;
        _rollSpeed = _rollDistance / _rollTime;

        // there are 20 animation frames for the roll
        _rollFrameTime = 20 / (_rollTime * 60);

        // true = right,  false = left
        _lastDirection = true;
        for (int i = 0; i <_animationStates.Length; i++)
        {
            int hash = Animator.StringToHash("Base Layer." + _animationStates[i]);
            _animationStatesDict.Add(_animationStates[i], hash);
        }
    }

    void Start()
    {
        for (int i = 0; i < _animationStates.Length; i++)
        {
            Debug.Log(_animationStatesDict[_animationStates[i]] + _animationStates[i]);
        }
    }

    // Update() is called once per frame
    void Update()
    {
        _deltaTime = Time.deltaTime * _timeScale;
        _currentState.UpdateStatesLogic();
        _currentState.UpdateStatesPhysics();
        controller2D.Move(_currentMovement);

    }

    void OnEnable()
    {
        playerActionControls.Enable();
    }

    void OnDisable()
    {
        playerActionControls.Disable();
    }

    public void ChangeAnimationState(string newAnimationState)
    {
        int newAnimationStateHash = _animationStatesDict[newAnimationState];

        if (newAnimationStateHash == _currentAnimationStateHash) return;

        if (animator != null)
        {
            animator.Play(newAnimationStateHash);
        }
        _currentAnimationStateHash = newAnimationStateHash;

    }


    public IEnumerator WaitCoroutine(float waitTime, System.Action<bool> callback)
    {
        //yield on a new YieldInstruction that waits for x seconds.
        yield return new WaitForSeconds(waitTime);

        //After we have waited x seconds set _finished to true
        bool isFinished = true;

        if (callback != null) callback(isFinished);
    }

    public float CalculateHorizontalMovement (float value, float acceleration, float maxspeed)
    {   
        // f(x) = y(1 - a^-x)

        float result = maxspeed * (1 - Mathf.Pow(acceleration, -value));
      
       
        return result;
    }


    // response methods to input actions
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isJumpPressed = true;

        } else if (context.canceled)
        {
            _isJumpPressed = false;
            _requireJumpPressed = false;
        }

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            _isMovementPressed = true;
            _moveInputVector = playerActionControls.Gameplay.Move.ReadValue<Vector2>();

        } else if (context.canceled) {
            _isMovementPressed = false;
            _moveInputVector = playerActionControls.Gameplay.Move.ReadValue<Vector2>();
        }
    }

    public void OnRollDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isRollDashPressed = true;
        } else if (context.canceled) {
            _isRollDashPressed = false;
            _requireRollDashPressed = false;
        }
    }
}
