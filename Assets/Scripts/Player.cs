using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : Entity
{
    public PlayerInputSet input  {get; private set;}
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
    public Vector2 moveInput { get; private set; }
    public float dashDuration = 0.25f;
    public float dashSpeed = 20;

    protected override void Awake()
    {
        base.Awake();
        
        input = new PlayerInputSet();
        
        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        jumpState = new PlayerJumpState(this, stateMachine, "jumpFall");
        fallState = new PlayerFallState(this, stateMachine, "jumpFall");
        wallSlideState = new playerWallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "jumpFall");
        dashState = new PlayerDashState(this, stateMachine, "dash");
        basicAttackState = new PlayerBasicAttackState(this, stateMachine, "basicAttack");
        jumpAttackState = new PlayerJumpAttackState(this, stateMachine, "jumpAttack");
    }

    override protected void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
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
        stateMachine.ChangeState(basicAttackState);
    }
}
