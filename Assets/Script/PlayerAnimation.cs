using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : StateMachineBehaviour
{
    Player player;
    RaycastHit hit;
    Transform transform;
    Vector3 pos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.gameObject.GetComponent<Player>();
        player.Allmovement = false;
        player.SetActTrail(true);
        player.AttackNum = animator.GetInteger("AttackCombo");

        if (player.AttackNum == 0)
        {
            player.FollowCameraVec3();
        }
        transform = animator.transform;
        pos = transform.position + transform.forward * 10f;
        Debug.DrawRay(transform.GetChild(2).position, transform.forward * 1f, Color.black, 5f);
        Physics.Raycast(transform.GetChild(2).position, transform.GetChild(2).forward, out hit, 1f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hit.transform == null)
        {
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
        }
        player = animator.gameObject.GetComponent<Player>();
        player.ReduceStamina(5f);
        if (Input.GetMouseButtonDown(0) && player.AttackNum < player.AttackNum + 1 && player.AttackNum < 2)
        {
            animator.SetInteger("AttackCombo", player.AttackNum + 1);
        }

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetInteger("PlayerCondition") != 3 && animator.GetInteger("PlayerCondition") < 9)
        {
            player = animator.gameObject.GetComponent<Player>();
            player.Allmovement = true;
            player.SetActTrail(false);
        }
    }
}