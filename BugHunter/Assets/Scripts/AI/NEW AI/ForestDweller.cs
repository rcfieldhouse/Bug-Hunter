using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestDweller : AI
{

   public Rigidbody Rigidbody;
    private bool _CanJump = false,RoarPossible=false,HasFrontAttacked=false,_CanRun=false;

    public override void AttackPlayer(GameObject Target)
    {
        AI_Animator.SetBool("_IsMoving", false);
        if (CanAttack == true && HasAttacked == false)
        {
            if (HasFrontAttacked == true)
                IsSecondaryAttack = true;
            if (IsSecondaryAttack == true)
            {
                SecondaryAttack(Target);
                return;
            }
               //play Dante.sound.ogg torterra attack
                AI_Animator.SetBool("_IsAttacking", true);
                Target.GetComponent<HealthSystem>().ModifyHealth(Attack_1_Damage);
                HasAttacked = true;
                CanAttack = false;
            HasFrontAttacked = true;
        }
        if (HasAttacked == true)
        {
            HasAttacked = false;
           Invoke(nameof(ResetAttack), Attack_1_Delay);
            Invoke(nameof(StopAnim), 1.0f);
        }
    }
    void SecondaryAttack(GameObject Target)
    {
        AI_Animator.SetBool("_IsAttacking2", true);
        HasAttacked = true;
        CanAttack = false;
        HasFrontAttacked = false;
        Target.GetComponent<HealthSystem>().ModifyHealth(Attack_2_Damage);
        Invoke(nameof(ResetAttack), Attack_2_Delay);
        Invoke(nameof(StopAnim), 1.0f);
    }
    public override void Patroling()
    {
        RoarPossible = true;
        Debug.Log("apt");
       NavAgent.enabled = false;
       //do nothing bc this man is a boss and he doesn't need to patrol
       //he is the one who knocks
    }
    public override void ChasePlayer()
    {
        if (RoarPossible)
            DoTheRoar();
        if (_CanRun == false)
            return;

     
      
        base.ChasePlayer();
       // transform.LookAt(Target.transform);
    }
    public override void Update()
    {
        NavAgent.enabled = true;
        base.Update();
     
    }
    void DoTheRoar()
    {
        RoarPossible = false;   
        AI_Animator.SetBool("_Roar", true);
        Invoke(nameof(StopAnim), 1.0f);
        Invoke(nameof(RoarComplete), 1.0f);
    }
    public void StopAnim()
    {
        AI_Animator.SetBool("_IsAttacking2", false);
        AI_Animator.SetBool("_IsAttacking", false);
        AI_Animator.SetBool("_Roar", false);
        AI_Animator.SetBool("_IsMoving", false);
    }
    public void RoarComplete()
    {
        _CanRun = true;
    }
}