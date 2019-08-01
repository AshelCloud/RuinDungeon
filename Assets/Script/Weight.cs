using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weight : MonoBehaviour {

    private static Weight _instance;

    public static Weight instance
    {
        get
        {
            if (_instance == null) { _instance = FindObjectOfType(typeof(Weight)) as Weight; }

            return _instance;
        }
    }

    private Text text;
    private Image image;

    private bool isUpdate;

    public void Initialize()
    {
        text = GetComponent<Text>();
        text.rectTransform.anchoredPosition = Vector2.zero;

        image = GetComponentInChildren<Image>();

        text.rectTransform.anchoredPosition3D -= new Vector3(0, 55, 0);

        SetWeight();

        isUpdate = true;
    }

    public void SetWeight()
    {
        string weight = string.Format("{0:D2} / 50", (int)Inventory.instance.data.weight);
        text.rectTransform.sizeDelta = new Vector2(text.fontSize * weight.Length / 1.5f, text.fontSize * 1.2f);
        text.text = weight;
    }
}
