using Unity.VisualScripting.FullSerializer;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class EntityState
{
   protected StateMachine stateMachine;
   protected string stateName;

   public EntityState(StateMachine stateMachine, string stateName)
   {
      this.stateMachine = stateMachine;
      this.stateName = stateName;
   }

   public virtual void Enter()
   {
      Debug.Log("I enter " + stateName);
   }

   public virtual void Update()
   {
      Debug.Log("I run update of  " + stateName);
   }

   public virtual void Exit()
   {
      Debug.Log("I exit " + stateName);
   }
}
