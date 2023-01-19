using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class SoulSealItem : MonoBehaviour
{
    [SerializeField] private GameObject sealIcon;
    [SerializeField] private TooltipTrigger tooltip;

    internal void Activate(bool activateMode)
    {
        sealIcon.SetActive(activateMode);
        tooltip.content = (activateMode == true) ? "Soul Seal" : "This Cell is empty";
    }
}
