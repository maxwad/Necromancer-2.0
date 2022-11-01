using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RuneDestroyer : MonoBehaviour, IDropHandler
{
    private RunesManager runesManager;
    private RunesWindow runesWindow;
    private ResourcesManager resourcesManager;

    private void Start()
    {
        runesManager = GlobalStorage.instance.runesManager;
        runesWindow = GlobalStorage.instance.playerMilitaryWindow.GetComponentInChildren<RunesWindow>();
        resourcesManager = GlobalStorage.instance.resourcesManager;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(GlobalStorage.instance.IsGlobalMode() == false)
        {
            InfotipManager.ShowWarning("You cannot make any changes during the battle.");
            return;
        }

        RuneUIItem rune = eventData.pointerDrag.GetComponent<RuneUIItem>();
        if(rune == null) return;

        List<Cost> runeCompensation = rune.rune.realCost;

        foreach(var price in runeCompensation)
        {
            resourcesManager.ChangeResource(price.type, Mathf.RoundToInt(price.amount * 0.33f));
        }

        runesWindow.CutRuneFromList(rune);
        runesManager.FillCell(rune.rune);

        Destroy(eventData.pointerDrag);
        runesWindow.UpdateWindow();
    }
}
