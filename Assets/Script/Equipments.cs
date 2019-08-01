using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipments : MonoBehaviour 
{
    private static Equipments _instance;

    public static Equipments instance
    {
        get
        {
            if(_instance == null) { _instance = FindObjectOfType(typeof(Equipments)) as Equipments; }
            
            return _instance;
        }
    }

    Player player;
    public EquipmentsData data;
    public Image helmet;
    public Image top;
    public Image pants;
    public Image weapon;
    public Image item_1;
    public Image item_2;
    public Image item_3;
    private Image equipmentsImage;
    private float widthSize = 100f; // default value
    private bool open;
    private bool isUpdate = false;

    public void Initialize(Player player)
    {
        //Save();
        widthSize = 960f;
        this.player = player;
        equipmentsImage = GetComponent<Image>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(false);
        }

        List<ItemData> items = DataManager.BinaryDeserialize<List<ItemData>>("ItemListData" + "_" + 1); // ItemList_레벨

        GameObject go = new GameObject();
        Item item = go.AddComponent<Item>();
        item.data = items[1];

        Image goImage = go.AddComponent<Image>();
        goImage.sprite = Resources.Load<Sprite>(Define.TexturePath + item.data.spritePath);
        goImage.SetNativeSize();

        RectTransform itemRectTransform = goImage.rectTransform;
        Vector2 halfVec = new Vector2(0.5f, 0.5f);

        itemRectTransform.anchorMin = halfVec;
        itemRectTransform.anchorMax = halfVec;
        itemRectTransform.pivot = halfVec;

        itemRectTransform.localPosition = Vector3.zero;

        go.name = item.data.name;

        ItemClickEvent goClick = go.AddComponent<ItemClickEvent>();

        Wearing(item);

        SetSize();
        Load();

        open = false;
        equipmentsImage.enabled = false;
        isUpdate = true;
    }

    public void Equipmentset(Player player)
    {
        this.player = player;
    }

    private void SetSize()
    {
        RectTransform rect = equipmentsImage.rectTransform;
        equipmentsImage.SetNativeSize();

        Invoke("SetPos", Time.deltaTime);
        
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.anchorMin = new Vector2(0f, 0.5f);

        BoxCollider myCollider = GetComponent<BoxCollider>();
        myCollider.size = rect.sizeDelta;
        myCollider.center = new Vector3(myCollider.size.x * 0.5f, 0f, 0f);
    }

    private void Save()
    {
        DataManager.BinarySerialize(data, Define.EquipmentsDataPath);
    }

    private void Load()
    {
        data = DataManager.BinaryDeserialize<EquipmentsData>(Define.EquipmentsDataPath);
    }

    private void SetPos()
    {
        RectTransform rect = equipmentsImage.rectTransform;
        rect.anchoredPosition3D = new Vector3(210f, 0f, 0f);
    }

    public void Wearing(Item item)
    {
        switch(item.data.itemType)
        {
            case ItemType.Helmet:
            {
                if(0 >= helmet.transform.childCount)
                {
                    item.transform.SetParent(helmet.transform);
                    item.transform.position = helmet.transform.position;
                    item.data.wearing = true;
                }
                break;
            }

            case ItemType.Top:
            {
                if(0 >= top.transform.childCount)
                {
                    item.transform.SetParent(top.transform);
                    item.transform.position = top.transform.position;
                    item.data.wearing = true;
                }
                break;
            }

            case ItemType.Pants:
            {
                if(0 >= pants.transform.childCount)
                {
                    item.transform.SetParent(pants.transform);
                    item.transform.position = pants.transform.position;
                    item.data.wearing = true;
                }
                break;
            }
            
            case ItemType.Potion:
            {
                if(0 >= item_1.transform.childCount)
                {
                    player.PotionImage.gameObject.SetActive(true);
                    item.transform.SetParent(item_1.transform);
                    item.transform.position = item_1.transform.position;
                    item.data.wearing = true;
                }

                else if(0 >= item_2.transform.childCount)
                {
                    player.PotionImage.gameObject.SetActive(true);
                    item.transform.SetParent(item_2.transform);
                    item.transform.position = item_2.transform.position;
                    item.data.wearing = true;
                }

                else if(0 >= item_3.transform.childCount)
                {
                    player.PotionImage.gameObject.SetActive(true);
                    item.transform.SetParent(item_3.transform);
                    item.transform.position = item_3.transform.position;
                    item.data.wearing = true;
                }
                break;
            }

            case ItemType.Knife:
            {
                if(0 >= weapon.transform.childCount)
                {
                    player.ChangeWeapon(3);
                    item.transform.SetParent(weapon.transform);
                    item.transform.position = weapon.transform.position;
                    item.data.wearing = true;
                }
                break;
            }

            case ItemType.SwordAndShiled:
            {
                if (0 >= weapon.transform.childCount)
                {
                    player.ChangeWeapon(1);
                    item.transform.SetParent(weapon.transform);
                    item.transform.position = weapon.transform.position;
                    item.data.wearing = true;
                }
                break;
            }

            case ItemType.TwoHandSword:
            {
                if (0 >= weapon.transform.childCount)
                {
                    player.ChangeWeapon(2);
                    item.transform.SetParent(weapon.transform);
                    item.transform.position = weapon.transform.position;
                    item.data.wearing = true;
                }
                break;
            }
        }

        RefreshStats();
    }

    public void RefreshStats()
    {
        //스탯 새로고침
    }


    public void OpenAndClose()
    {
        open = !open;

        equipmentsImage.enabled = open;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.SetActive(open);
        }
    }

    public bool IsWearing(ItemType type)
    {
        switch(type)
        {
            case ItemType.Helmet:
            {
                if(helmet.transform.childCount > 0)
                {
                    return false;
                }
                
                return true;
            }
            case ItemType.Top:
            {
                if (top.transform.childCount > 0)
                {
                    return false;
                }

                return true;
            }
            case ItemType.Pants:
            {
                if (pants.transform.childCount > 0)
                {
                    return false;
                }

                return true;
            }
            case ItemType.Potion:
            {
                if (item_1.transform.childCount > 0 && item_2.transform.childCount > 0 && item_3.transform.childCount > 0)
                {
                    return false;   
                }
                    
                return true;
            }
            case ItemType.Knife:
            {
                if (weapon.transform.childCount > 0)
                {
                    return false;
                }

                return true;
            }
            case ItemType.SwordAndShiled:
            {
                if (weapon.transform.childCount > 0)
                {
                    return false;
                }

                return true;
            }
            case ItemType.TwoHandSword:
            {
                if (weapon.transform.childCount > 0)
                {
                    return false;
                }

                return true;
            }
        }

        return false;
    }
}
