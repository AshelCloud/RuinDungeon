using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour 
{
    private static PuzzleManager _instance;

    public static PuzzleManager instnace
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType(typeof(PuzzleManager)) as PuzzleManager;
            }
            return _instance;
        }
    }

    public List<GameObject> triangles = new List<GameObject>();
    public void Add(GameObject triangle)
    {
        triangles.Add(triangle);
    }

    public List<GameObject> bossDoor = new List<GameObject>();
    public void BossDoorAdd(GameObject door)
    {
        bossDoor.Add(door);
    }

    public void CorrectCheck()
    {
        int cnt = 0;
        for (int i = 0; i < triangles.Count; i ++)
        {
            if (triangles[i].GetComponent<Triangle>().isCorrect)
            {
                cnt ++;
            }
            Debug.Log(cnt);
            if(cnt == 3)
            {
                Invoke("Correct", 1f);
            }
        }
    }

    public bool correct;

    //여기 문열리는거랑 소리
    private void Correct()
    {
        for (int i = 0; i < bossDoor.Count; i++)
        {
            bossDoor[i].GetComponent<BossDoor>().Open();
        }
        //소리 넣으삼
        transform.GetChild(0).GetComponent<AudioSource>().Play();
        transform.GetChild(1).GetComponent<AudioSource>().Play();
        correct = true;
    }

    public bool Check(int count)
    {
        if(count % 3 == 1)
        {
            return true;
        }

        return false;
    }
}
