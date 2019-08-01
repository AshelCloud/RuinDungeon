using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class ItemClickEvent : MouseEvent
{
    public override void OnPointerEnter(PointerEventData eventData)
    {
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        InvokeRepeating("MouseTracking", Time.deltaTime, Time.deltaTime);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        CancelInvoke("MouseTracking");
        WearingEquipment();
        Check();
        Item[] items = Inventory.instance.GetComponentsInChildren<Item>();
        int w = 0;
        for (int i = 0; i < items.Length; i++)
        {
            w += items[i].data.weight;
        }
        Inventory.instance.data.weight = w;

        Weight.instance.SetWeight();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
    }

    //이미지가 마우스 따라가는 함수
    private void MouseTracking()
    {
        Canvas myCanvas = FindObjectOfType<Canvas>() as Canvas;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);

        Vector2 inventorySizeDelta = Inventory.instance.GetComponent<RectTransform>().sizeDelta;
        Bounds inventoryBounds = Inventory.instance.GetComponent<BoxCollider>().bounds;

        gameObject.transform.position = myCanvas.transform.TransformPoint(pos);

        bool inventoryResult = inventoryBounds.Contains(gameObject.transform.position);
        bool equipmentsResult = Equipments.instance.GetComponent<BoxCollider>().bounds.Contains(gameObject.transform.position);
        bool chestResult = Chest_UI.instance.GetComponent<BoxCollider>().bounds.Contains(gameObject.transform.position);

        if (inventoryResult)
        {
            gameObject.transform.SetParent(Inventory.instance.transform);
        }

        if (equipmentsResult)
        {
            gameObject.transform.SetParent(Equipments.instance.transform);
        }

        if (chestResult)
        {
            gameObject.transform.SetParent(Chest_UI.instance.transform);
        }
        ////성모왈 : Clamp로 하면 모든게 쉬워질지어니...
        ////성모왈 : 아님 말고
        //Vector3 playerPos = gameObject.transform.localPosition;
        //if (gameObject.transform.localPosition.y > inventoryBounds.max.y)
        //{
        //    playerPos.y = inventoryBounds.max.y;
        //}
        //if (inventoryBounds.min.y > gameObject.transform.localPosition.y)
        //{
        //    playerPos.y = inventoryBounds.min.y;
        //}
        ////if(inventoryBounds.min.x * 2f > gameObject.transform.localPosition.x)
        ////{
        ////    playerPos.x = inventoryBounds.min.x * 2f;
        ////}
        //if (gameObject.transform.localPosition.x > inventoryBounds.center.x)
        //{
        //    playerPos.x = inventoryBounds.center.x;
        //}

        //gameObject.transform.localPosition = playerPos;
    }

    private void WearingEquipment()
    {
        if (Chest_UI.instance.chestImage.enabled) { return; }
        if (!Equipments.instance.IsWearing(gameObject.GetComponent<Item>().data.itemType) && Equipments.instance.GetComponent<BoxCollider>().bounds.Contains(gameObject.transform.position))
        {
            gameObject.transform.SetParent(Inventory.instance.transform);

            SetPosition();
            return;
        }

        Canvas myCanvas = FindObjectOfType<Canvas>() as Canvas;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);

        Vector2 equipmentsSizeDelta = Equipments.instance.GetComponent<RectTransform>().sizeDelta;
        Bounds equipmentsBounds = Equipments.instance.GetComponent<BoxCollider>().bounds;

        bool result = equipmentsBounds.Contains(gameObject.transform.position);

        if (result)
        {
            Equipments.instance.Wearing(gameObject.GetComponent<Item>());
        }
    }

    private void Check()
    {
        Bounds inventoryBounds = Inventory.instance.GetComponent<BoxCollider>().bounds;

        bool inventoryResult = inventoryBounds.Contains(gameObject.transform.position);
        bool equipmentsResult = Equipments.instance.GetComponent<BoxCollider>().bounds.Contains(gameObject.transform.position);
        bool chestResult = Chest_UI.instance.GetComponent<BoxCollider>().bounds.Contains(gameObject.transform.position);

        if (!inventoryResult && !equipmentsResult && !chestResult)
        {
            gameObject.transform.SetParent(Inventory.instance.transform);

            gameObject.GetComponent<Item>().data.wearing = false;
            SetPosition();
        }
    }

    private void SetPosition()
    {
        gameObject.GetComponent<Image>().rectTransform.anchoredPosition3D = Vector3.zero;
    }
}

public class Item : MonoBehaviour
{
    public ItemData data;
}
