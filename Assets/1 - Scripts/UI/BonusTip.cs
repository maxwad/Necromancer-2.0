using System.Collections;
using UnityEngine;
using TMPro;

public class BonusTip : MonoBehaviour
{
    private TMP_Text bonusText;
    private float smallWaitTime = 0.01f;
    private WaitForSeconds smallWait;

    private float lifeTime = 3f;
    private float currentTime = 0f;
    private Vector3 startOffset = new Vector3(0, 1.5f, 0);

    private SpriteRenderer spriteRenderer;
    private string luckText = "x2";

    public void Iniatilize(int counter, Sprite sprite, float quantity)
    {
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            bonusText = GetComponentInChildren<TMP_Text>();
        }

        spriteRenderer.sprite = sprite;

        bonusText.text = quantity.ToString();
        bonusText.color = Color.white;
        if(quantity == 0) 
        {
            bonusText.color = Color.yellow;
            bonusText.text = luckText;
        }        
  
        transform.position += startOffset * 2;
        transform.position += startOffset * counter;

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
        gameObject.SetActive(false);
    }
}
