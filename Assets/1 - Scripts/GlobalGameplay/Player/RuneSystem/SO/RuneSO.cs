using UnityEngine;
using static NameManager;

[CreateAssetMenu(fileName = "RuneData", menuName = "RuneItem")]

public class RuneSO : ScriptableObject
{
    public string runeName;
    public string serieName;
    public RunesType rune;
    public Sprite activeIcon;
    public Sprite inActiveIcon;
    public int level;
    public float value;
    public StatBoostType valueType;
    public int cost;
    public string positiveDescription;
    public string negativeDescription;
}
