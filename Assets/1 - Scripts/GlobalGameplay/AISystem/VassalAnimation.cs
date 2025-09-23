using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static Enums;

public class VassalAnimation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text actionLabel;

    public void Init(Color castleColor)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = castleColor;
        Activate(false);
    }

    public void Activate(bool activeMode) => gameObject.SetActive(activeMode);

    public void SetFlipProperty(bool flipMode) => spriteRenderer.flipX = flipMode;

    public bool GetFlipProperty() => spriteRenderer.flipX;

    public void ShowAction(string action) => actionLabel.text = action;

    public void Fading(bool isFading) => StartCoroutine(Fade(isFading));

    private IEnumerator Fade(bool isFading)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(0.02f);
        Color currentColor = spriteRenderer.color;
        
        float alfaFrom = 0;
        float alfaTo = 1;
        float step = 0.05f;

        if(isFading == true)
        {
            alfaFrom = 1;
            alfaTo = 0;
            step = -step;
        }

        bool stop = false;

        while(stop == false)
        {
            alfaFrom += step;
            currentColor.a = alfaFrom;
            spriteRenderer.color = currentColor;

            if(step > 0)
            {
                if(alfaFrom >= alfaTo) stop = true;
            }
            else
            {
                if(alfaFrom <= alfaTo) stop = true;
            }

            yield return delay;
        }
    }
}
