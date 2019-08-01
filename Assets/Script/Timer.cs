using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float timesec;
    private int timemin;

    private Text text;
    private Image image;

    private bool isUpdate;

    public void Initialize()
    {
        text = GetComponent<Text>();
        text.rectTransform.anchoredPosition = Vector2.zero;

        image = GetComponentInChildren<Image>();

        isUpdate = true;
    }

    private void Update()
    {
        if (!isUpdate) { return; }
        Changetime();
        string clock = string.Format("{0:D2} : {1:D2}", timemin, (int)timesec);
        text.rectTransform.sizeDelta = new Vector2(text.fontSize * clock.Length / 1.7f, text.fontSize * 1.2f);
        text.text = clock;
        image.rectTransform.sizeDelta = new Vector2(text.fontSize, text.fontSize);
    }

    private void Changetime()
    {
        timesec += Time.deltaTime;

        if (timesec >= 60)
        {
            timesec = 0;
            timemin += 1;
        }
    }
}
