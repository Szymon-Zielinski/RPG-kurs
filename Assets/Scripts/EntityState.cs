using UnityEngine;

public abstract class EntityState 
{
    protected StateMachine stateMachine;
    protected string AnimBoolName;
    
    protected Animator anim;
    protected Rigidbody2D rb;
    
    protected float stateTimer;
    protected bool triggerCalled;

    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.AnimBoolName = animBoolName;
    }
    public virtual void Enter()
    {
        anim.SetBool(AnimBoolName, true);
        triggerCalled = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }

    public virtual void Exit()
    {
        anim.SetBool(AnimBoolName, false);
    }

    public void CallAnimationTrigger()
    {
        triggerCalled = true;
    }
}
