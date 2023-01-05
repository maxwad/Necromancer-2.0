using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class WarningTip : MonoBehaviour
{
    private CanvasGroup canvas;
    public TMP_Text caption;

    public string warningText = "Warning:";
    public string messageText = "Message:";

    public TMP_Text contentText;

    private bool isOpen = false;
    public float lifeTime = 3f;
    private float currentTime = 0;

    private Image bg;
    public Color warningColor;
    public Color messageColor;    

    public void Show(string content, bool messageMode)
    {
        if(canvas == null)
        {
            canvas = GetComponent<CanvasGroup>();
            bg = GetComponent<Image>();
        }

        bg.color = (messageMode == false) ? warningColor : messageColor;
        caption.text = (messageMode == false) ? warningText : messageText;

        contentText.text = content;
        currentTime = lifeTime;

        if(isOpen == false) Fading.instance.FadeWhilePause(true, canvas);

        isOpen = true;
    }

    private void Update()
    {
        if(isOpen == true)
        {
            currentTime -= Time.unscaledDeltaTime;
            if(currentTime <= 0)
            {
                isOpen = false;
                Fading.instance.FadeWhilePause(false, canvas, 0.02f, activeMode: false);
            }
        }
    }
}
