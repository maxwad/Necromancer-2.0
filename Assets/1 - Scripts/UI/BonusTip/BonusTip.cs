using System.Collections;
using UnityEngine;
using TMPro;

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

    public void Show(int counter, Sprite sprite, float quantity, string text = "")
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

        bonusText.text = "+" + quantity;
        bonusText.color = Color.white;
        if(quantity == 0) 
        {
            bonusText.color = Color.yellow;
            bonusText.text = text;
        }        
  
        transform.position += startOffset * 0.5f;
        transform.position += startOffset * (counter + 1);

        StartCoroutine(ShowBonus());
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
        BonusTipUIManager.HideVisualEffect();
        gameObject.SetActive(false);
    }
}
