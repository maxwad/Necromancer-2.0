using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private TMP_Text damageText;
    private float smallWaitTime = 0.01f;
    private float bigWaitTime = 0.5f;
    private WaitForSeconds smallWait;
    private WaitForSeconds bigWait;
    private Vector3 damageNoteStartOffsetStandart = new Vector3(0, 0.7f, 0);
    private Vector3 damageNoteStartOffset;
    private Vector3 damageNoteScaleOffset;
    private Vector3 damageNotePositionOffset;

    private float damageValue;
    private Color textColor;

    public void Iniatilize(float damage, Color color)
    {        
        damageNoteScaleOffset = new Vector3(smallWaitTime, smallWaitTime, smallWaitTime);
        damageNotePositionOffset = new Vector3(0, smallWaitTime, 0);

        damageText = GetComponent<TMP_Text>();
        damageValue = damage;
        textColor = color;

        float randomOffset = Random.Range(-40, 30) * 0.01f;
        damageNoteStartOffset = damageNoteStartOffsetStandart + new Vector3(0, randomOffset, 0);

        transform.position += damageNoteStartOffset;
        transform.localScale = Vector3.one;

        StartCoroutine(ShowDamage(damageValue, textColor));
    }

    private IEnumerator ShowDamage(float damage, Color color)
    {
        smallWait = new WaitForSeconds(smallWaitTime);
        bigWait = new WaitForSeconds(bigWaitTime);

        damageText.color = color;
        damageText.text = damage.ToString();

        float heigth = 1;
        float minScale = 0f;
        float maxScale = 1.25f;
        float currentScale = 1f;

        while (heigth > 0 && currentScale < maxScale)
        {
            heigth -= 0.01f;
            transform.position += damageNotePositionOffset;

            currentScale += smallWaitTime;
            transform.localScale += damageNoteScaleOffset;

            yield return smallWait;
        }

        yield return bigWait;

        while (currentScale > minScale)
        {
            currentScale -= smallWaitTime * 2;
            transform.localScale -= damageNoteScaleOffset * 2;

            yield return smallWait;
        }

        //reset offset
        damageNoteStartOffset = damageNoteStartOffsetStandart;
        gameObject.SetActive(false);
        //TODO: change to set gameObject false
    }
}
