using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMapUI : MonoBehaviour
{
    private Text[] texts;
    private Text curText;

    private static SelectMapUI _instance;

    public static SelectMapUI instance
    {
        get
        {
            if (_instance == null) { _instance = FindObjectOfType(typeof(SelectMapUI)) as SelectMapUI; }

            return _instance;
        }
    }

    private int _count;

    private int count
    {
        get
        {
            return _count;
        }

        set
        {
            if (texts.Length > value && value >= 0)
            {
                _count = value;

                curText = texts[_count];
            }
        }
    }

    Image image;
    public void Initialize()
    {
        texts = GetComponentsInChildren<Text>();

        count = 0;

        ChangeImage();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(false);
        }

        image = GetComponent<Image>();
        image.rectTransform.anchoredPosition = Vector3.zero;
        image.enabled = false;
    }

    private void Update()
    {
        if (open)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                count--;

                ChangeImage();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                count++;

                ChangeImage();
            }

            if (Input.GetKeyDown(KeyCode.Return) && open)
            {
                //로드하는 부분
                Debug.Log("load");
                GameSystem.instance.Load("Map_2", "UserData_2", "InventoryData", "EquipmentsData");
                Destroy(gameObject);

                //---- 예시

                //---1번
                //GameSystem.instance.Load(curText.name);

                //---2번
                //GameSystem.instance.Load(curText.name, "앙 유저데이터띠(웬만하면 안바뀔 이름이니 안넣어도됨)", "앙 인벤토리 데이터띠(웬만하면 안바뀔 이름이니 안넣어도됨)", "앙 장비창 데이터 띠(웬만하면 안바뀔 이름이니 안넣어도됨)");
            }
        }

    }

    private void ChangeImage()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].GetComponentInChildren<Image>().enabled = false;
        }
        curText.GetComponentInChildren<Image>().enabled = true;
    }

    private bool open = false;
    public void Open()
    {
        open = !open;
        image.enabled = open;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(open);
        }
    }
}
