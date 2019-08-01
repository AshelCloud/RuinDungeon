using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    Vector3 curRotation;
    Coroutine curCoroutine;
    int curCount;
    public bool isCorrect;

    public void Initialize()
    {
        curRotation = transform.rotation.eulerAngles;
        curCount = 0;
        curCoroutine = null;
        PuzzleManager.instnace.Add(gameObject);
    }

    private AudioSource MoveSound;
    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Player>())
        {
            if(Input.GetKeyDown(KeyCode.F) && !PuzzleManager.instnace.correct)
            {
                if(curCoroutine == null)
                {
                    if (MoveSound == null)
                        MoveSound = Instantiate(Resources.Load<AudioSource>(Define.PrefabFilePath + "D1_TriangleMove"));
                    StopAllCoroutines();

                    MoveSound.Play();

                    curRotation = transform.rotation.eulerAngles;
                    
                    curCoroutine = StartCoroutine(Rotate());

                    curCount++;

                    isCorrect = PuzzleManager.instnace.Check(curCount);

                    PuzzleManager.instnace.CorrectCheck();
                }

            }
        }
    }
    
    private IEnumerator Rotate()
    {
        while(true)
        {
            if(transform.rotation.eulerAngles.y < (curRotation.y + 115f))
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(curRotation.x, curRotation.y + 120f, curRotation.z), 1f * Time.deltaTime);
            }
            else
            {
                curCoroutine = null;
            }
            
            yield return new WaitForEndOfFrame();
        }
    }
}
