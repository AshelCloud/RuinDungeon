using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDoor : MonoBehaviour
{
    public bool isLeft;
    Animation myAnimation;

    public void Initialize()
    {
        myAnimation = GetComponent<Animation>();

        PuzzleManager.instnace.BossDoorAdd(gameObject);
    }

    public void Open()
    {
        if (isLeft)
        {
            myAnimation["Open_Boss_2"].speed = 0.1f;
            myAnimation.Play("Open_Boss_2");
        }
        else
        {
            myAnimation["Open_Boss"].speed = 0.1f;
            myAnimation.Play("Open_Boss");
        }
    }
}
