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

   public EntityState(Player player, StateMachine stateMachine, string animBoolName)
   {
      this.player = player;
      this.stateMachine = stateMachine;
      this.AnimBoolName = animBoolName;
      anim = player.anim;
      rb = player.rb;
   }

   public virtual void Enter()
   {
      anim.SetBool(AnimBoolName, true);
   }

   public virtual void Update()
   {
      Debug.Log("I run update of  " + AnimBoolName);
   }

   public virtual void Exit()
   {
       anim.SetBool(AnimBoolName, false);
   }
}
