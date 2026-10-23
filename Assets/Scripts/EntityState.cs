using Unity.VisualScripting.FullSerializer;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public abstract class EntityState
{
   protected Player player;
   protected StateMachine stateMachine;
   protected string AnimBoolName;
   protected Animator anim;
   protected Rigidbody2D rb;
   protected PlayerInputSet input;
   protected float stateTimer;
   protected bool triggerCalled;

   public EntityState(Player player, StateMachine stateMachine, string animBoolName)
   {
      this.player = player;
      this.stateMachine = stateMachine;
      this.AnimBoolName = animBoolName;
      anim = player.anim;
      rb = player.rb;
      input = player.input;
   }

   public virtual void Enter()
   {
      anim.SetBool(AnimBoolName, true);
      triggerCalled = false;
   }

   public virtual void Update()
   {
      stateTimer -= Time.deltaTime;
      anim.SetFloat("yVelocity", rb.linearVelocity.y);

      if (input.Player.Dash.WasPressedThisFrame() && CanDash())
      {
         stateMachine.ChangeState(player.dashState);
      }
   }

   public virtual void Exit()
   {
       anim.SetBool(AnimBoolName, false);
   }

   public void CallAnimationTrigger()
   {
      triggerCalled = true;
   }

   private bool CanDash()
   {
      if (player.wallDetected)
      {
         return false;
      }

      if (stateMachine.currentState == player.dashState)
      {
         return false;
      }
      return true;
   }
}
