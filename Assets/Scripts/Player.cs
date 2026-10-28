using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb {get; private set;}
    public Animator anim {get; private set;}   
    public PlayerInputSet input  {get; private set;}
    private StateMachine _stateMachine;
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public playerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerBasicAttackState basicAttackState  { get; private set; }
    public PlayerJumpAttackState jumpAttackState { get; private set; }

    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = 0.1f;
    public float comboResetTime = 1f;
    private Coroutine _queuedAttackCo;
    
    [Header("Movement details")] 
    public float moveSpeed = 8;
    public float jumpForce = 12;
    [FormerlySerializedAs("wallJumpDir")] public Vector2 walJumpForce;
    public float inAirMoveMultiplier = 0.7f;
    public float wallSlideMultiplier = 0.4f;
    private bool _facingRight = true;
    public int facingDir { get; private set; } = 1;
    public Vector2 moveInput { get; private set; }
    public float dashDuration = 0.25f;
    public float dashSpeed = 20;

    [Header("Collision detection")] 
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform priamryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    
    
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        _stateMachine = new StateMachine();
        input = new PlayerInputSet();
        
        idleState = new Player_IdleState(this, _stateMachine, "idle");
        moveState = new Player_MoveState(this, _stateMachine, "move");
        jumpState = new PlayerJumpState(this, _stateMachine, "jumpFall");
        fallState = new PlayerFallState(this, _stateMachine, "jumpFall");
        wallSlideState = new playerWallSlideState(this, _stateMachine, "wallSlide");
        wallJumpState = new PlayerWallJumpState(this, _stateMachine, "jumpFall");
        dashState = new PlayerDashState(this, _stateMachine, "dash");
        basicAttackState = new PlayerBasicAttackState(this, _stateMachine, "basicAttack");
        jumpAttackState = new PlayerJumpAttackState(this, _stateMachine, "jumpAttack");
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        _stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        HandleCollisionDetection();
        _stateMachine.UpdateActiveState();
    }

    
    public void EnterAttackStateWithDelay()
    {
        if (_queuedAttackCo != null)
        {
            StopCoroutine(_queuedAttackCo);
        }

        _queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }
    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        _stateMachine.ChangeState(basicAttackState);
    }
    
    public void CallAnimationTrigger()
    {
        _stateMachine.currentState.CallAnimationTrigger();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    private void HandleFlip(float xVelocity)
    {
        if (!_facingRight && xVelocity > 0)
        {
            Flip();
        }
        else if (_facingRight && xVelocity < 0)
        {
            Flip();
        }
    }
    
    public void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        _facingRight = !_facingRight;
        facingDir *= -1;
    }

    private void HandleCollisionDetection()
    {
        
        // groundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        // wallDetected = Physics2D.Raycast(priamryWallCheck.position, Vector2.right * facingDir, wallCheckDistance,
        //     whatIsGround)
        // && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * facingDir, wallCheckDistance,
        //     whatIsGround);
        groundDetected = Physics2D.Raycast(
            groundCheck.position,
            Vector2.down,
            groundCheckDistance,
            whatIsGround
        );
        RaycastHit2D hit1 = Physics2D.Raycast(
            priamryWallCheck.position,
            Vector2.right * facingDir,
            wallCheckDistance,
            whatIsGround);
        
        RaycastHit2D hit2 = Physics2D.Raycast(
            secondaryWallCheck.position,
            Vector2.right * facingDir,
            wallCheckDistance,
            whatIsGround);
        
        wallDetected = hit1 && hit2;
        
        // Debug.DrawRay(
        //     priamryWallCheck.position,
        //     Vector2.right * facingDir * wallCheckDistance,
        //     hit1 ? Color.green : Color.red);
        //
        // Debug.DrawRay(
        //     secondaryWallCheck.position,
        //     Vector2.right * facingDir * wallCheckDistance,
        //     hit2 ? Color.green : Color.red);
    

    }

   private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(priamryWallCheck.position, priamryWallCheck.position + new Vector3(wallCheckDistance * facingDir,0));
        Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * facingDir,0));
    }
}
