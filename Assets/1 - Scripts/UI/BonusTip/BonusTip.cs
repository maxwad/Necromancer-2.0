using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class BonusTip : MonoBehaviour
{
    private RectTransform rectTransform;
    private TMP_Text bonusText;
    private float smallWaitTime = 0.01f;
    private WaitForSeconds smallWait;

    private float lifeTime = 3f;
    private float currentTime = 0f;
    private float heigthOffset = 0;
    private Vector3 startOffset;

    private SpriteRenderer spriteRenderer;
    private Coroutine coroutine;

    public void Show(int counter, Sprite sprite, float quantity, string text = "", string mark = "+")
    {
        if(spriteRenderer == null)
        {
            rectTransform = GetComponentInChildren<RectTransform>();
            heigthOffset = rectTransform.rect.height;
            startOffset = new Vector3(0, 1.2f * heigthOffset, 0);
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            bonusText = GetComponentInChildren<TMP_Text>();
        }

        spriteRenderer.sprite = sprite;

        bonusText.text = mark + quantity;
        bonusText.color = Color.white;
        if(quantity == 0) 
        {
            bonusText.color = Color.yellow;
            bonusText.text = text;
        }        
  
        transform.position += startOffset * 0.5f;

        if(GlobalStorage.instance.isGlobalMode == true)
            transform.position += startOffset * (counter + 1);

        coroutine = StartCoroutine(ShowBonus());
    }

    private IEnumerator ShowBonus()
    {
        smallWait = new WaitForSeconds(smallWaitTime);

        while(currentTime <= lifeTime)
        {
            currentTime += smallWaitTime;
            transform.position += new Vector3(0, smallWaitTime, 0);

            yield return smallWait;
        }

        currentTime = 0;
        spriteRenderer.sprite = null;

        if(GlobalStorage.instance.isGlobalMode == true)
            BonusTipUIManager.HideVisualEffect();

        gameObject.SetActive(false);
    }

    private void Deactivation(bool mode)
    {
        if(coroutine != null) StopCoroutine("ShowBonus");
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.SwitchPlayer += Deactivation;        
    }

    private void OnDisable()
    {
        EventManager.SwitchPlayer -= Deactivation;
    }
}
