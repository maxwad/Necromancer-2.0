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
            playersArmy = GlobalStorage.instance.player.GetComponent<PlayersArmy>();
            playersArmyUI = GlobalStorage.instance.playerMilitaryWindow.GetComponent<PlayersArmyPart>();

            canvasGroup = GetComponent<CanvasGroup>();
            Canvas[] group = GlobalStorage.instance.playerMilitaryWindow.GetComponentsInChildren<Canvas>();
            dragdropZone = group[group.Length - 1];

        }
        backlight.color = unvisible;
    }

    public void Init(Unit unit)
    {
        if(squadtipTrigger == null) squadtipTrigger = GetComponent<InfotipTrigger>();

        if (unit.unitController != null && unit.unitController.quantity != 0)
        {
            unitInSlot = unit;
            icon.sprite = unit.unitIcon;
            quantity.text = unit.unitController.quantity.ToString();
            squadtipTrigger.SetUnit(unit);
        }
        else
        {
            unitInSlot = null;
            icon.sprite = null;
            quantity.text = null;
            squadtipTrigger.SetUnit(null);
        }

        backlight.color = unvisible;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if(slotType == UISlotTypes.Army && eventData.button == PointerEventData.InputButton.Right)
        //{
        //    backlight.color = backlight.color == unvisible ? visible : unvisible;
        //    playersArmy.UnitsReplacementUI(index);
        //}

        //if(eventData.button == PointerEventData.InputButton.Left && GlobalStorage.instance.isGlobalMode == true)
        //{
        //    bool mode = slotType == UISlotTypes.Reserve ? true : false;
        //    playersArmy.SwitchUnit(mode, unitInSlot);
        //}
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(unitInSlot == null || unitInSlot.isUnitActive == false)
        {
            return;
        }

        currentParent = transform.parent;
        SquadSlotPlacing parentPlace = currentParent.GetComponent<SquadSlotPlacing>();
        if(parentPlace != null) parentPlace.ClearSlot();

        parentStorage = transform.parent;

        transform.SetParent(dragdropZone.transform, false);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(unitInSlot == null || unitInSlot.isUnitActive == false)
        {
            return;
        }
        else
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(transform.parent != dragdropZone.transform)
        {
            if(transform.parent != currentParent)
            {
                currentParent = transform.parent;
            }
        }
        else
        {
            transform.parent = parentStorage;
            transform.localPosition = Vector3.zero;
            currentParent = transform.parent;
        }

        canvasGroup.blocksRaycasts = true;

        playersArmy.UpdateSquads();
    }


    //public void ResetSelecting()
    //{
    //    backlight.color = unvisible;
    //    playersArmy.ResetReplaceIndexes();
    //}
}
