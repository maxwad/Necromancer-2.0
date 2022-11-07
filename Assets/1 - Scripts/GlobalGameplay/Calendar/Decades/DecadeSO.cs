using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

[CreateAssetMenu(fileName = "DecadeData", menuName = "DecadeItem")]
public class DecadeSO : ScriptableObject
{
    public string decadeName;
    public RuneSO effect;
    public BoostEffect purpose;
    public bool isNegative;


    //public int identificator;
    ////$ - is the place for replace by value
    //[TextArea(2, 4)]
    //public string decadeDescription;

    //public BoostFromType boostType;
    //public float value;
    //public bool isPercentType = true;

    //public Sprite icon;
}
