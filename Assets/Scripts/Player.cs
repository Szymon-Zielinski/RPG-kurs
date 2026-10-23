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
    private StateMachine stateMachine;
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public playerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerBasicAttackState BasicAttackState  { get; private set; }

    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public float attackVelocityDuration = 0.1f;
    public float comboResetTime = 1f;
    private Coroutine queuedAttackCo;
    
    
    [Header("Movement details")] 
    public float moveSpeed = 8;
    public float jumpForce = 12;
    [FormerlySerializedAs("wallJumpDir")] public Vector2 walJumpForce;
    public float inAirMoveMultiplier = 0.7f;
    public float wallSlideMultiplier = 0.4f;
    private bool facingRight = true;
    public int facingDir { get; private set; } = 1;
    public Vector2 moveInput { get; private set; }
    public float dashDuration = 0.25f;
    public float dashSpeed = 20;
    
    

    [Header("Collision detection")] 
    [SerializeField] private float groundCheckDistance;

    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }
    
    
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        stateMachine = new StateMachine();
        input = new PlayerInputSet();
        
        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new PlayerJumpState(this, stateMachine, "jumpFall");
        fallState = new PlayerFallState(this, stateMachine, "jumpFall");
        wallSlideState = new playerWallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "jumpFall");
        dashState = new PlayerDashState(this, stateMachine, "dash");
        BasicAttackState = new PlayerBasicAttackState(this, stateMachine, "basicAttack");
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
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
        {
            StopCoroutine(queuedAttackCo);
        }

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }
    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(BasicAttackState);
    }
    
    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    private void HandleFlip(float xVelocity)
    {
        if (!facingRight && xVelocity > 0)
        {
            Flip();
        }
        else if (facingRight && xVelocity < 0)
        {
            Flip();
        }
    }
    
    public void Flip()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
        facingDir *= -1;
    }

    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(transform.position, Vector2.right *facingDir, wallCheckDistance, whatIsGround);
    }

   private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(wallCheckDistance * facingDir,0));
    }
}
