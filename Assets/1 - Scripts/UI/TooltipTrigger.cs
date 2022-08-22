using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string content;
    public string visitedStatus;
    public string notVisitedStatus;

    private string status;

    private float timeDelay = 1f;
    private float currentWaitTime = 0;
    private bool isWaiting = false;
    private bool isTooltipOpen = false;

    private GlobalMapTileManager gmManager;

    private void Start()
    {
        SetStatus(false);
        gmManager = GlobalStorage.instance.gmManager;
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
        status = (mode == false) ? notVisitedStatus : visitedStatus;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isWaiting = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isWaiting == false) TooltipManager.Hide();

        isTooltipOpen = false;
        isWaiting = false;
        currentWaitTime = 0;
    }

    private void OnMouseEnter()
    {
        if(EventSystem.current.IsPointerOverGameObject()) return;
        if(CheckTheFog() == true) return;  

        isWaiting = true;
    }

    private bool CheckTheFog()
    {
        Vector3Int checkPosition = gmManager.fogMap.WorldToCell(transform.position);
        return gmManager.fogMap.HasTile(checkPosition);        
    }

    private void OnMouseExit()
    {
        if(isWaiting == false) TooltipManager.Hide();

        isTooltipOpen = false;
        isWaiting = false;
        currentWaitTime = 0;
    }

    private void OnDisable()
    {
        if (isTooltipOpen == true) TooltipManager.Hide();
    }    
}
