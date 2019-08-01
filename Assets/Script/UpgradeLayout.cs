using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeLayout : MonoBehaviour 
{
    public Text statusText;
    public Text valueText;
    public Image items;
    public ToggleGroup status;
    private Player player;
    private Image upgradeLayoutImage;

    private static UpgradeLayout _instance;

    public static UpgradeLayout instance
    {
        get
        {
            if (_instance == null) { _instance = FindObjectOfType(typeof(UpgradeLayout)) as UpgradeLayout; }

            return _instance;
        }
    }

    public bool open;

    public void Initialize(Player player)
    {
        this.player = player;
        upgradeLayoutImage = GetComponent<Image>();
        transform.position = Vector3.zero;
        open = false;

        OpenAndClose(open);

        SetSize();
    }

    public void ChangeStausText(Toggle toggle)
    {
        Text childText = toggle.GetComponentInChildren<Text>();

        statusText.text = childText.text;

        RefreshText(statusText);
    }

    int value = 0;
    Item curItem;
    public void SelectItem(bool isClick)
    {
        value = GetCurrentStatus();

        switch(curItem.data.itemClass)
        {
            case ItemClass.Lowly :
                valueText.text = value.ToString() + " -> " + (value + 1).ToString() + " ~ " + (value + 3).ToString();
            break;

            case ItemClass.Usable:
                valueText.text = value.ToString() + " -> " + (value + 4).ToString() + " ~ " + (value + 6).ToString();
            break;

            case ItemClass.Useful:
                valueText.text = value.ToString() + " -> " + (value + 7).ToString() + " ~ " + (value + 9).ToString();
            break;

            case ItemClass.Perfect:
                valueText.text = value.ToString() + " -> " + (value + 10).ToString();
            break;
        }
    }

    //현재 선택된 능력치에 대한 플레이어 스탯을 반환함
    Toggle curToggle;
    private int GetCurrentStatus()
    {
        curToggle = null;
        for (int i = 0; i < status.transform.childCount; i++)
        {
            Toggle childToggle = status.transform.GetChild(i).GetComponent<Toggle>();
            if (childToggle.isOn)
            {
                curToggle = childToggle;
                break;
            }
        }

        if (curToggle == null)
        {
            return 0;
        }
        Debug.Log(player.data.maxHp);
        switch (curToggle.name)
        {
            case "HP" :
                return player.data.maxHp;

            case "Stamina" :
                return player.data.maxStamina;
                
            case "Str" :
                return player.data.stat.str;

            case "Luk" :
                return player.data.stat.luk;
        }
        return 0;
    }

    private void SetCurrentStatus(int value)
    {
        player.PlayerParticle[3].Play();
        player.UpgradeSound.Play();
        switch (curToggle.name)
        {
            case "HP":
                player.maxHp += value;
                break;

            case "Stamina":
                player.maxStamina += value;
                break;

            case "Str":
                player.stats.str += value;
                break;

            case "Luk":
                player.stats.luk += value;
                break;
        }

        RefreshText();
    }

    public void Upgrade()
    {
        int random = 0;
        switch(curItem.data.itemClass)
        {
            case ItemClass.Lowly :
                random = Random.Range(1, 4);
                SetCurrentStatus(random);
            break;

            case ItemClass.Usable:
                random = Random.Range(4, 7);
                SetCurrentStatus(random);
            break;

            case ItemClass.Useful:
                random = Random.Range(7, 10);
                SetCurrentStatus(random);
            break;

            case ItemClass.Perfect:
                SetCurrentStatus(10);
            break;
        }
        
        DeleteItem();
    }

    private void DeleteItem()
    {
        for (int i = 0; i < Inventory.instance.transform.childCount; i++)
        {
            if (curItem == Inventory.instance.transform.GetChild(i))
            {
                Destroy(Inventory.instance.transform.GetChild(i).gameObject);
            }
        }

        DeleteItemGameObject();
    }

    private void RefreshText()
    {
        switch (curToggle.name)
        {
            case "HP":
                valueText.text = player.maxHp.ToString();
                break;

            case "Stamina":
                valueText.text = player.maxStamina.ToString();
                break;

            case "Str":
                valueText.text = player.stats.str.ToString();
                break;

            case "Luk":
                valueText.text = player.stats.luk.ToString();
                break;
        }
    }

    private void RefreshText(Text text)
    {
        switch (text.text)
        {
            case "생명력":
                valueText.text = player.maxHp.ToString();
                break;

            case "지구력":
                valueText.text = player.maxStamina.ToString();
                break;

            case "힘":
                valueText.text = player.stats.str.ToString();
                break;

            case "운":
                valueText.text = player.stats.luk.ToString();
                break;
        }
    }

    private void SetSize()
    {
        RectTransform rect = upgradeLayoutImage.rectTransform;

        rect.anchoredPosition3D = Vector3.zero;
    }

    public void DeleteItemGameObject()
    {
        for(int i = 0; i < curItems.Count; i ++)
        {
            if(curItems[i] == null) { continue; }
            if(curItems[i].GetComponent<Toggle>().isOn)
            {
                Destroy(curItems[i].gameObject);
                return;
            }
        }
    }

    //인벤토리에 있는 아이템들 Instantiate
    //Instantiate 할때 Script도 같이 적용된상태에서 생성되는지 몰라서 냅둠
    //만약 생성이 같이 안된다면 AddComponent<Item>().data = Inventory.instance.tranform.GetChild(i).GetComponent<Item>().data;
    List<GameObject> curItems = new List<GameObject>();
    public void OpenAndClose(bool isOn)
    {
        for (int i = 0; i < curItems.Count; i++)
        {
            Destroy(curItems[i].gameObject);
        }

        curItems = new List<GameObject>();

        upgradeLayoutImage.enabled = isOn;
        for(int i = 0; i < transform.childCount; i ++)
        {
            transform.GetChild(i).gameObject.SetActive(isOn);
        }

        if (isOn)
        {
            for(int i = 0; i < Inventory.instance.transform.childCount; i ++)
            {
                GameObject go = Instantiate(Inventory.instance.transform.GetChild(i).gameObject, items.transform);
                go.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/" + go.GetComponent<Item>().data.itemType + "_Box");
                Destroy(go.GetComponent<ItemClickEvent>());
                Toggle toggle = go.AddComponent<Toggle>();
                toggle.onValueChanged.AddListener(SelectItem);

                curItem = toggle.GetComponent<Item>();

                go.SetActive(true);

                curItems.Add(go);
            }
            for (int i = 0; i < 9 - Inventory.instance.transform.childCount; i++)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>(Define.PrefabFilePath + "GearBox"), items.transform);
                Destroy(go.GetComponent<ItemClickEvent>());

                go.SetActive(true);

                curItems.Add(go);
            }
            if (curItems.Count / 3 > 3)
                items.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, curItems.Count / 3 * 152);
        }
    }
}
