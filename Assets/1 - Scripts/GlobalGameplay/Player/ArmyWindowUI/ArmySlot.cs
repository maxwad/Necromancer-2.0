using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static NameManager;

public class ArmySlot : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public Image icon;
    public TMP_Text quantity;
    public int index;
    public Image backlight;
    public UnitStatus slotType;

    public Color visible;
    public Color unvisible;
    public Unit unitInSlot;

    private InfotipTrigger squadtipTrigger;
    private CanvasGroup canvasGroup;

    private PlayersArmy playersArmy;
    private PlayersArmyPart playersArmyUI;


    private Transform currentParent;
    private Transform parentStorage;
    private Canvas dragdropZone;

    private void OnEnable()
    {
        if(playersArmy == null) 
        { 
            playersArmy = GlobalStorage.instance.playersArmy;
            playersArmyUI = GlobalStorage.instance.playerMilitaryWindow.GetComponent<PlayersArmyPart>();

            canvasGroup = GetComponent<CanvasGroup>();
            Canvas[] group = GlobalStorage.instance.playerMilitaryWindow.GetComponentsInChildren<Canvas>();
            dragdropZone = group[group.Length - 1];
        }

        if(unitInSlot != null) quantity.text = unitInSlot.unitController.quantity.ToString();
    }

    public void Init(Unit unit)
    {
        if(squadtipTrigger == null) squadtipTrigger = GetComponent<InfotipTrigger>();
        //if(unitInSlot != null)
        //{
        //    Debug.Log("ArmySlot Level was = " + unitInSlot.level + " and became " + unit.level);
        //}
        
        unitInSlot = unit;
        icon.sprite = unit.unitIcon;
        quantity.text = unit.unitController.quantity.ToString();
        squadtipTrigger.SetUnit(unit);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(unitInSlot.status == UnitStatus.Army)
            {
                if(GlobalStorage.instance.isGlobalMode == true)
                    playersArmyUI.ForceClearCell(this);
                else
                {
                    InfotipManager.ShowWarning("You can not delete any squads from hero's army during the battle.");
                    return;
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {        
        if(unitInSlot == null || unitInSlot.isUnitActive == false)
        {
            Debug.Log("Take: slot = " + unitInSlot.isUnitActive);
            return;
        }

        if(unitInSlot.status == UnitStatus.Store && GlobalStorage.instance.isGlobalMode == false)
        {
            InfotipManager.ShowWarning("You can not add any squads to your hero during the battle.");
            return;
        }

        currentParent = transform.parent;
        parentStorage = transform.parent;

        transform.SetParent(dragdropZone.transform, false);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(unitInSlot == null || unitInSlot.isUnitActive == false)
        {
            Debug.Log("drag: slot = " + unitInSlot.isUnitActive);
            return;
        }
        else
        {
            if(unitInSlot.status == UnitStatus.Store && GlobalStorage.instance.isGlobalMode == false)
            {
                InfotipManager.ShowWarning("You can not add any squads to your hero during the battle.");
                return;
            }

            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(unitInSlot.status == UnitStatus.Store && GlobalStorage.instance.isGlobalMode == false)
        {
            InfotipManager.ShowWarning("You can not add any squads to your hero during the battle.");
            return;
        }

        if(transform.parent != dragdropZone.transform)
        {
            if(transform.parent != currentParent)
            {
                currentParent = transform.parent;
            }
        }
        else
        {
            transform.SetParent(parentStorage);
            transform.localPosition = Vector3.zero;
            currentParent = transform.parent;
        }

        canvasGroup.blocksRaycasts = true;

        //playersArmy.UpdateSquads();
    }

    public int GetPositionIndex() => index;
}
