using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BonusTip : MonoBehaviour
{
    private TMP_Text bonusText;
    private float smallWaitTime = 0.01f;
    private float bigWaitTime = 0.5f;
    private WaitForSeconds smallWait;
    private WaitForSeconds bigWait;
    private Vector3 bonusNoteStartOffsetStandart = new Vector3(0, 0.7f, 0);
    private Vector3 bonusNoteStartOffset;
    private Vector3 bonusNoteScaleOffset;
    private Vector3 bonusNotePositionOffset;

    private float bonusValue;
    private Color textColor;

    private SpriteRenderer spriteRenderer;

    public void Iniatilize(float bonus, Color color)
    {
        bonusNoteScaleOffset = new Vector3(smallWaitTime, smallWaitTime, smallWaitTime);
        bonusNotePositionOffset = new Vector3(0, smallWaitTime, 0);

        bonusText = GetComponent<TMP_Text>();
        bonusValue = bonus;
        textColor = color;

        float randomOffset = Random.Range(-40, 30) * 0.01f;
        bonusNoteStartOffset = bonusNoteStartOffsetStandart + new Vector3(0, randomOffset, 0);

        transform.position += bonusNoteStartOffset;
        transform.localScale = Vector3.one;

        StartCoroutine(Showbonus(bonusValue, textColor));
    }

    private IEnumerator Showbonus(float bonus, Color color)
    {
        smallWait = new WaitForSeconds(smallWaitTime);
        bigWait = new WaitForSeconds(bigWaitTime);

        bonusText.color = color;
        bonusText.text = bonus.ToString();

        float heigth = 1;
        float minScale = 0f;
        float maxScale = 1.25f;
        float currentScale = 1f;

        while(heigth > 0 && currentScale < maxScale)
        {
            heigth -= 0.01f;
            transform.position += bonusNotePositionOffset;

            currentScale += smallWaitTime;
            transform.localScale += bonusNoteScaleOffset;

            yield return smallWait;
        }

        yield return bigWait;

        while(currentScale > minScale)
        {
            currentScale -= smallWaitTime * 2;
            transform.localScale -= bonusNoteScaleOffset * 2;

            yield return smallWait;
        }

        //reset offset
        bonusNoteStartOffset = bonusNoteStartOffsetStandart;
        spriteRenderer.sprite = null;
        gameObject.SetActive(false);
        //TODO: change to set gameObject false
    }

    public void SetIcon(Sprite sprite)
    {
        if(spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}
