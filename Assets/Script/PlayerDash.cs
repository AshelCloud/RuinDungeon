using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : StateMachineBehaviour {

    Player player;
    Transform transform;
    Vector3 Pos;
    RaycastHit hit;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.GetComponent<Player>();
        transform = animator.transform;
        Pos = transform.position + transform.forward * 20f;
        player.Allmovement = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.DrawRay(transform.GetChild(2).position, transform.forward * 1f, Color.white, 5f);
        Physics.Raycast(transform.GetChild(2).position, transform.GetChild(2).forward, out hit, 1f);
        if (hit.transform == null)
        {
            transform.position = Vector3.Lerp(transform.position, Pos, Time.deltaTime * 3);
        }
    }
}
