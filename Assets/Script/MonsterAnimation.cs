using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAnimation : StateMachineBehaviour 
{

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
    private Vector3 pos;
    private MonsterData monsterdata;
    private Transform transform;
    private Monster monster;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monster = animator.GetComponent<Monster>();
        monsterdata = monster.data;
        transform = animator.transform;
        if (monsterdata.name == "Skeleton_Am" || monsterdata.name == "Skeleton_King")
        {
            pos = animator.transform.position + animator.transform.forward * 15f;
        }
        else
        {
            pos = Vector3.zero;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.DrawRay(transform.GetChild(2).position, transform.forward * 25f, Color.black, 5f);
        RaycastHit hit;
        Physics.Raycast(transform.GetChild(2).position, transform.GetChild(2).forward, out hit, 25f);
        if (monsterdata.name == "Skeleton_King" && monster.AttackSpeed != 0)
        {
            if (hit.transform == null || (hit.transform != null && (!hit.collider.CompareTag("Wall") || !hit.collider.CompareTag("InvisivleWall"))))
            {
                pos = animator.transform.position + animator.transform.forward * monster.AttackSpeed;
                transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
            }
        }
        else if (monsterdata.name == "Skeleton_Am")
        {
            if (animator.GetInteger("Condition") == 2 && hit.transform == null && pos != Vector3.zero)
            {
                transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monster.attackCount = 0;
        if (animator.GetInteger("Combo") == 0 && transform.CompareTag("Boss") && animator.GetInteger("Condition") != 4)
        {
            if (animator.GetInteger("Condition") == 5)
            {
                animator.SetInteger("Condition", 1);
            }
            else
            {
                monster.BossNavi();
                monster.bossCombo = 0;
                animator.SetBool("Attack", false);
                animator.SetInteger("Condition", 0);
            }
        }
        else if (transform.CompareTag("Monster"))
        {
            animator.gameObject.GetComponent<NavMeshAgent>().enabled = true;
            animator.SetBool("Attack", false);
        }
    }
}
