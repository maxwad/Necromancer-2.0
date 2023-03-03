using UnityEngine;
using UnityEngine.EventSystems;
using static NameManager;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string header;
    public string content;
    public string visitedStatus;
    public string notVisitedStatus;

    public string status;

    private float timeDelay = 1f;
    private float currentWaitTime = 0;
    private bool isWaiting = false;
    private bool isTooltipOpen = false;
    private bool canIShowTipAgain = true;

    private GlobalMapTileManager gmManager;

    private void Start()
    {
        //SetStatus(false);
        gmManager = GlobalStorage.instance.gmManager;
    }


    private void Update()
    {
        if(isWaiting == true)
        {
            currentWaitTime += Time.unscaledDeltaTime;

            if(currentWaitTime >= timeDelay && canIShowTipAgain == true)
            {
                InfotipManager.Show(content, header, status);
                isTooltipOpen = true;
                isWaiting = false;
                canIShowTipAgain = false;
            }
        }
    }

    public void SetStatus(bool mode)
    {
        status = (mode == false) ? notVisitedStatus : visitedStatus;
    }

    public void SetOwner(string text)
    {
        status = "Owner: " + text;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isWaiting = true;
    }


    private void OnMouseEnter()
    {
        if(EventSystem.current.IsPointerOverGameObject()) return;

        if(CheckTheFog() == true) return;

        isWaiting = true;
    }

    private bool CheckTheFog()
    {
        if(gmManager.fogMap.gameObject.activeInHierarchy == false) return false;

        Vector3Int checkPosition = gmManager.fogMap.WorldToCell(transform.position);
        return gmManager.fogMap.HasTile(checkPosition);        
    }

    private void CloseTip()
    {
        if(isWaiting == false) InfotipManager.Hide(TipsType.Tool);

        isTooltipOpen = false;
        isWaiting = false;
        currentWaitTime = 0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canIShowTipAgain = true;
        CloseTip();
    }

    private void OnMouseExit()
    {
        canIShowTipAgain = true;
        CloseTip();
    }

    private void OnMouseDown()
    {
        CloseTip();
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1)) CloseTip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CloseTip();
    }

    private void OnDisable()
    {
        if(isTooltipOpen == true) CloseTip();
    }
}
