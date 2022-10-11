using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static NameManager;

public class ArmySlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TMP_Text quantity;
    public int index;
    public Image backlight;
    public UISlotTypes slotType;

    public Color visible;
    public Color unvisible;
    private Unit unitInSlot;

    private InfotipTrigger squadtipTrigger;

    private PlayersArmy playersArmy;

    private void OnEnable()
    {
        if(playersArmy == null) playersArmy = GlobalStorage.instance.player.GetComponent<PlayersArmy>();
        backlight.color = unvisible;
    }

    public void FillTheArmySlot(Unit unit)
    {
        if(squadtipTrigger == null) squadtipTrigger = GetComponent<InfotipTrigger>();

        if (unit != null && unit.quantity != 0)
        {
            unitInSlot = unit;
            icon.sprite = unit.unitIcon;
            quantity.text = unit.quantity.ToString();
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
        if(slotType == UISlotTypes.Army && eventData.button == PointerEventData.InputButton.Right)
        {
            backlight.color = backlight.color == unvisible ? visible : unvisible;
            playersArmy.UnitsReplacementUI(index);
        }

        if(eventData.button == PointerEventData.InputButton.Left && GlobalStorage.instance.isGlobalMode == true)
        {
            bool mode = slotType == UISlotTypes.Reserve ? true : false;
            playersArmy.SwitchUnit(mode, unitInSlot);
        }
    }

    //public void ResetSelecting()
    //{
    //    backlight.color = unvisible;
    //    playersArmy.ResetReplaceIndexes();
    //}
}
