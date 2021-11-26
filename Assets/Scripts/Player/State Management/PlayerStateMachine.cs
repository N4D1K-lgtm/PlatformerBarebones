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

    public readonly Vector2[] WallJumps = { new Vector2(0.05f, 0.01f), new Vector2(0.07f, 0.02f), new Vector2(0.09f, 0.03f) };

    // Animation Strings
    private string[] _animationStates = new string[] { "Idle", "Run", "Attack_1", "Jump", "Fall", "WallSlide" };
    private Dictionary<string, int> _animationStatesDict = new Dictionary<string, int>();
    
    // Jump height and force variables
    // To Do: Change jumpForce to depend on horizontal movement and jump height;
    private bool _isJumpPressed;
    private bool _isMovementPressed;
    private bool _isRunPressed;
    private bool _isJumpEnabled;
    private bool _requireJumpPressed;
    private bool _CanWallJump;
    private Vector3 _velocity;
    private Vector3 _oldVelocity;
    private Vector3 _currentMovement;
    private Vector2 _moveInputVector;
    private float _initialJumpForce;
    private float _timeToWallUnstick;
    private float _gravity;
    private float _timeScale;
    private float _deltaTime;
    private string _debugCurrentState;
    private int _currentAnimationStateHash;

    // State Variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // Getters and Setters

    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Controller2D Controller2D { get { return controller2D; } }
    public Transform Transform { get { return transform; } set { transform = value; } }
    // public Collider2D Collider2D { get { return collider2D; } }
    public Animator Animator { get { return animator; } }
    public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } }
    public Dictionary<string, int> AnimationStatesDict { get { return _animationStatesDict; } }
    public float VelocityY { get { return _velocity.y; } set { _velocity.y = value; } }
    public float VelocityX { get { return _velocity.x; } set { _velocity.x = value; } }
    public float OldVelocityY { get { return _oldVelocity.y; } set { _oldVelocity.y = value; } }
    public float OldVelocityX { get { return _oldVelocity.x; } set { _oldVelocity.x = value; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float CurrentMovementX { get { return _currentMovement.x; } set { _currentMovement.x = value; } }
    public float MoveInputVectorY { get { return _moveInputVector.y; } set { _moveInputVector.y = value; } } 
    public float MoveInputVectorX { get { return _moveInputVector.x; } set { _moveInputVector.x = value; } } 
    public float MaxVerticalVelocity { get { return _maxVerticalVelocity; } }
    public float MaxHorizontalVelocity { get { return _maxHorizontalVelocity; } }
    public float InitialJumpForce { get { return _initialJumpForce; } }
    public float WallSlideSpeed { get { return _wallSlideSpeed; } }
    public float TimeToWallUnstick { get { return _timeToWallUnstick; } set { _timeToWallUnstick = value;} }
    public float WallStickTime { get { return _wallStickTime; } }
    public float Gravity { get { return _gravity; } }
    public float HorizontalSpeed { get { return _horizontalSpeed; } }
    public float AccelerationTimeGrounded { get { return _accelerationTimeGrounded;} }
    public float AccelerationTimeAirborne { get { return _accelerationTimeAirborne;} }
    public float TimeScale { get { return _timeScale; } set { _timeScale = value; } }
    public float DeltaTime { get { return _deltaTime; } }
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsRunPressed { get { return _isRunPressed; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool RequireJumpPressed { get { return _requireJumpPressed; } set { _requireJumpPressed = value; } }
    public bool CanWallJump { get { return _CanWallJump; } set { _CanWallJump = value; } }
    public string DebugCurrentState { get { return _debugCurrentState; } set { _debugCurrentState = value; } }

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
        playerActionControls.Gameplay.Run.performed += context => OnRun(context);
        playerActionControls.Gameplay.Run.canceled += context => OnRun(context);

        // misc variables
        _initialJumpForce = 2 * _jumpHeight / _timeToJumpApex;
        _gravity = -2 * _jumpHeight / (_timeToJumpApex * _timeToJumpApex);
        _timeScale = 1;

        for (int i = 0; i <_animationStates.Length; i++)
        {
            int hash = Animator.StringToHash("Base Layer." + _animationStates[i]);
            _animationStatesDict.Add(_animationStates[i], hash);
        }
    }

    void Start()
    {
        /*for (int i = 0; i < _animationStates.Length; i++)
        {
            Debug.Log(_animationStatesDict[_animationStates[i]] + _animationStates[i]);
        }*/
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
    
    public void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

}
