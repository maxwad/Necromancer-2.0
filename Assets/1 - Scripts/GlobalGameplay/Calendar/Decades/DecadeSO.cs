using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

[CreateAssetMenu(fileName = "DecadeData", menuName = "DecadeItem")]
public class DecadeSO : ScriptableObject
{
    public string decadeName;
    //$ - is the place for replace by value
    [TextArea(2, 4)]
    public string decadeDescription;

    public BoostType boostType;
    public float value;
    public bool isPercentType = true;

    public Sprite icon;
}
