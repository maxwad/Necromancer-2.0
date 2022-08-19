using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string content;
    public string positiveStatus;
    public string negativeStatus;

    private string status;

    private float timeDelay = 0.1f;
    private float currentWaitTime = 0;
    private bool isWaiting = false;
    private bool isTooltipOpen = false;

    private void Awake()
    {
        SetStatus(false);
    }

    private void Update()
    {
        if(isWaiting == true)
        {
            currentWaitTime += Time.unscaledDeltaTime;

            if(currentWaitTime >= timeDelay)
            {
                TooltipManager.Show(content, header, status);
                isTooltipOpen = true;
                isWaiting = false;
            }
        }
    }

    public void SetStatus(bool mode)
    {
        status = (mode == false) ? negativeStatus : positiveStatus;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isWaiting = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isTooltipOpen = false;
        if(isWaiting == false) TooltipManager.Hide();

        currentWaitTime = 0;
    }

    private void OnMouseEnter()
    {
        if(EventSystem.current.IsPointerOverGameObject()) return;

        isWaiting = true;
    }

    private void OnMouseExit()
    {
        isTooltipOpen = false;
        if(isWaiting == false) TooltipManager.Hide();

        currentWaitTime = 0;
    }

    private void OnDisable()
    {
        if (isTooltipOpen == true) TooltipManager.Hide();
    }    
}
