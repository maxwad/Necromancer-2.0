using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager instance;

    public Tooltip tooltip;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public static void Show(string content, string header = "", string status = "")
    {
        instance.tooltip.SetText(content, header, status);
        instance.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        if(instance.tooltip != null) instance.tooltip.gameObject.SetActive(false);
    }
}
