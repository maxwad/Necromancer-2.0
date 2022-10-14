using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectTipPosition : MonoBehaviour
{
    private RectTransform tipRect;
    private float offset = 20f;
    private float height4K = 2000f;
    private float scaleFactor;
    private float scaleFactorNormal = 1f;
    private float scaleFactor4K = 2f;


    private void LateUpdate()
    {
        CorrectSquadtipPosition();

    }

    private void CorrectSquadtipPosition()
    {
        if(tipRect == null) tipRect = GetComponent<RectTransform>();

        scaleFactor = (Screen.height > height4K) ? scaleFactor4K : scaleFactorNormal;

        Vector2 position = Input.mousePosition;
        float pivotX = 0;
        float pivotY = 1;
        float offsetX = offset * scaleFactor;
        float offsetY = 0;


        if(Screen.width - position.x < tipRect.rect.width * scaleFactor)
        {
            pivotX = 1f;
            offsetX = -offsetX;
        }

        if(position.y - tipRect.rect.height * scaleFactor < 0)
        {
            pivotY = 0f;
        }

        if(position.y + tipRect.rect.height * scaleFactor > Screen.height)
        {
            pivotY = 1f;
        }

        tipRect.pivot = new Vector2(pivotX, pivotY);
        transform.position = position + new Vector2(offsetX, offsetY);
    }
}
