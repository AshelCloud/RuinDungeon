using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour 
{
    public InventoryData data;

    private static Inventory _instance;

    public static Inventory instance
    {
        get
        {
            if(_instance == null) { _instance = FindObjectOfType(typeof(Inventory)) as Inventory; }

            return _instance;
        }
    }
    
    private float widthSize = 100f; // Default;
    public Image inventoryImage;
    public bool open;
    public Image back;

    public void Initialize()
    {
        //Save();
        widthSize = 960f;
        inventoryImage = GetComponent<Image>();
        SetSize();
        Load();
        
        open = false;
        inventoryImage.enabled = false;
    }

    private void SetSize()
    {
        RectTransform rect = inventoryImage.rectTransform;
        inventoryImage.SetNativeSize();

        rect.pivot = new Vector2(1f, 0.5f);
        rect.anchorMax = new Vector2(1f, 0.5f);
        rect.anchorMin = new Vector2(1f, 0.5f);

        Invoke("SetPos", Time.deltaTime);

        BoxCollider myCollider = GetComponent<BoxCollider>();
        myCollider.size = rect.sizeDelta;
        myCollider.center = new Vector3(-myCollider.size.x * 0.5f, 0f, 0f);
    }

    public void AddItem(Item item)
    {
        item.data.wearing = false;

        bool result = Overlap(item);
        if(!result)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(transform);

            Image goImage = go.AddComponent<Image>();
            goImage.sprite = Resources.Load<Sprite>(Define.TexturePath + item.data.spritePath);
            goImage.SetNativeSize();

            itemRectTransform = goImage.rectTransform;

            goImage.rectTransform.anchoredPosition3D = Vector3.zero;

            //Invoke("SetPosition", Time.deltaTime);

            Item goItem = go.AddComponent<Item>();
            goItem.data = item.data;

            go.name = goItem.data.name;

            ItemClickEvent goClick = go.AddComponent<ItemClickEvent>();

            data.weight += goItem.data.weight;
            Weight.instance.SetWeight();

            go.SetActive(open);
        }
    }
    RectTransform itemRectTransform;

    private void SetPosition()
    {
        Vector2 halfVec = new Vector2(0.5f, 0.5f);

        itemRectTransform.anchorMin = halfVec;
        itemRectTransform.anchorMax = halfVec;
        itemRectTransform.pivot = halfVec;

        itemRectTransform.localPosition = Vector3.zero;
    }

    private bool Overlap(Item item)
    {
        if(data.items == null) { return false; }

        for (int i = 0; i < data.items.Count; i++)
        {
            Transform _childItem = transform.GetChild(i);

            Item childItem = _childItem.GetComponent<Item>();

            if(childItem == null) { continue; }
            
            if (childItem.data.name == item.data.name && childItem.data.itemType == item.data.itemType)
            {   
                if(!childItem.transform.Find("CountText"))
                {
                    GameObject go = new GameObject();
                    go.name = "CountText";
                    
                    Text goText = go.AddComponent<Text>();
                    goText.transform.SetParent(childItem.transform);
                }

                return true;
            }
        }

        return false;
    }

    Image BG = null;
    public void OpenAndClose(bool open)
    {
        inventoryImage.enabled = open;
        if (BG == null)
        {
            BG = Instantiate(back);
            BG.transform.SetParent(GameObject.Find("Canvas").transform);
            BG.SetNativeSize();
            BG.transform.SetAsFirstSibling();
            BG.transform.localPosition = new Vector3(0, 0, 0);
        }
        BG.gameObject.SetActive(open);
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(open);
        }
    }

    public void Save()
    {
        DataManager.BinarySerialize(data, Define.InventoryDataPath);
    }

    public void Load()
    {
        InventoryData fileData = DataManager.BinaryDeserialize<InventoryData>(Define.InventoryDataPath); //파일 불러오기
        if (fileData.items == null) { return; }
        foreach (ItemData itemData in fileData.items)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = itemData.name;
            gameObject.transform.SetParent(transform);
            gameObject.transform.localPosition = itemData.position;

            Image itemImage = gameObject.AddComponent<Image>();
            itemImage.sprite = Resources.Load<Sprite>(itemData.spritePath);

            Item item = gameObject.AddComponent<Item>();
            item.data = itemData;
        }
    }

    private void SetPos()
    {
        RectTransform rect = inventoryImage.rectTransform;
        rect.anchoredPosition3D = new Vector3(-210f, 0f, 0f);
    }
}
