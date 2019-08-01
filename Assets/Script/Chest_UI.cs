using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest_UI : MonoBehaviour 
{
    public ChestData data;

    private static Chest_UI _instance;

    public static Chest_UI instance
    {
        get
        {
            if(_instance == null) { _instance = FindObjectOfType(typeof(Chest_UI)) as Chest_UI; }

            return _instance;
        }
    }

    public Image chestImage;
    float widthSize, heightSize;
    public bool open;

    public void Initialize()
    {
        chestImage = GetComponent<Image>();
        widthSize = 1000f;
        heightSize = 500f;

        open = true;

        OpenAndClose();

        SetSize();

        Load();
    }

    private void SetSize()
    {
        chestImage.SetNativeSize();
        
        Invoke("SetPosition", Time.deltaTime);
    }

    private void SetPosition()
    {
        RectTransform rect = chestImage.rectTransform;
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchoredPosition3D = new Vector3(210f, 0f, 0f);


        BoxCollider myCollider = GetComponent<BoxCollider>();
        myCollider.size = rect.sizeDelta;
        myCollider.center = new Vector3(myCollider.size.x * 0.5f, 0f, 0f);
    }


    public void Save()
    {
        DataManager.BinarySerialize(data, Define.ChestDataPath + ".txt");
    }

    public void Load()
    {
        data = DataManager.BinaryDeserialize<ChestData>(Define.ChestDataPath);

        for(int i = 0; i < data.items.Count; i ++)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(transform);

            Image goImage = go.AddComponent<Image>();
            goImage.sprite = Resources.Load<Sprite>(Define.TexturePath + data.items[i].spritePath);
            goImage.SetNativeSize();

            RectTransform itemRectTransform = goImage.rectTransform;
            Vector2 halfVec = new Vector2(0.5f, 0.5f);

            itemRectTransform.anchorMin = halfVec;
            itemRectTransform.anchorMax = halfVec;
            itemRectTransform.pivot = halfVec;

            itemRectTransform.localPosition = Vector3.zero;

            Item goItem = go.AddComponent<Item>();
            goItem.data = data.items[i];

            go.name = goItem.data.name;

            ItemClickEvent goClick = go.AddComponent<ItemClickEvent>();

            go.SetActive(chestImage.enabled);
        }
    }

    public void OpenAndClose()
    {
        open = !open;

        chestImage.enabled = open;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(open);
        }

        Inventory.instance.OpenAndClose(open);
    }
}
 