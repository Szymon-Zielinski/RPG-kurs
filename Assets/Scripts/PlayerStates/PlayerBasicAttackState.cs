using UnityEngine;

public class PlayerBasicAttackState : PlayerState
{

    private float _attackVelocityTimer;
    private float _lastTimeAttacked;
    
    private bool _comboAttackQueue;
    private int _attackDir;
    private int _comboIndex = 1;
    private readonly int comboLimit = 3;
    private const int FirstComboIndex = 1;

    
    public PlayerBasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (_comboIndex != player.attackVelocity.Length)
        {
            comboLimit = player.attackVelocity.Length;
        }
    }

    
    
    
    
    public override void Enter()
    {
        base.Enter();
        _comboAttackQueue = false;
        ResetComboIndexIfNeeded();

        _attackDir = player.moveInput.x !=0 ? ((int)player.moveInput.x) : player.facingDir;
        
        anim.SetInteger("basicAttackIndex", _comboIndex);
        ApplyAttackVelocity();

    }
    
    override public void Update()
    {
        base.Update();
        HandleAttackVelocity();

        if (input.Player.Attack.WasPressedThisFrame())
        {
            QueueNextAttack();
        }
        HandleStateExit();

    }

    public override void Exit()
    {
        base.Exit();
        
        _comboIndex++;
        _lastTimeAttacked = Time.time;
    }

    
    
    
    
    
    private void HandleStateExit()
    {
        if (triggerCalled)
        {
            if (_comboAttackQueue)
            {
                anim.SetBool(AnimBoolName, false);
                player.EnterAttackStateWithDelay();
            }
            stateMachine.ChangeState(player.idleState);
        }
    }
    private void QueueNextAttack()
    {
        if (_comboIndex < comboLimit)
        {
            _comboAttackQueue = true;
        }
    }
    private void HandleAttackVelocity()
    {
        _attackVelocityTimer -= Time.deltaTime;
        if (_attackVelocityTimer < 0)
        {
            player.SetVelocity(0, rb.linearVelocity.y);
        }
    }

    private void ApplyAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[_comboIndex -1];
        _attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * _attackDir, attackVelocity.y);
    }
    private void ResetComboIndexIfNeeded()
    {
        if (Time.time > _lastTimeAttacked + player.comboResetTime)
        {
            _comboIndex = FirstComboIndex;
        }
        if (_comboIndex > comboLimit)
        {
            _comboIndex = FirstComboIndex;
        }
    }
}
