using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class Decade 
{
    public string decadeName;
    public string decadeDescription;

    public BoostFromType boostType;
    public float value;
    public bool isPercentType;
    public Sprite icon;

    public int identificator;

    public Decade(DecadeSO decadeSO, int number)
    {
        decadeName = decadeSO.decadeName;

        boostType = decadeSO.boostType;
        isPercentType = decadeSO.isPercentType;

        value = (isPercentType == true) ? (decadeSO.value / 100) : decadeSO.value;
        icon = decadeSO.icon;

        identificator = number;
        decadeDescription = GetDescription(decadeSO.decadeDescription, decadeSO.value);
    }

    private string GetDescription(string oldString, float val)
    {
        return oldString.Replace("$", Mathf.Abs(val).ToString());
    }
}
