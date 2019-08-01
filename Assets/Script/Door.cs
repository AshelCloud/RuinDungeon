using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour 
{
    public bool isOpen;
    public bool left;

    private Animation myAnimation;
    

    private void Start()
    {
        myAnimation = GetComponent<Animation>();
    }

    public void PlayAnimation_1()
    {
            
        if (left)
        {
            myAnimation["Open_3"].speed = 1f;
            myAnimation.Play("Open_3");
            Debug.Log(gameObject.GetComponentInChildren<AudioSource>().gameObject.name);
            gameObject.GetComponentInChildren<AudioSource>().Play();
        }
        else
        {
            myAnimation["Open_4"].speed = 1f;
            myAnimation.Play("Open_4");
        }

        isOpen = true;
    }

    public void PlayAnimation_2()
    {
        gameObject.GetComponentInChildren<AudioSource>().Play();
        myAnimation.Play("Open_2");
        isOpen = true;
    }
}
