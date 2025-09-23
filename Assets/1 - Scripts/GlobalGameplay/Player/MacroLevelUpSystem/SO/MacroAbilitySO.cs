using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "MacroAbilityData", menuName = "MacroAbilityItem")]

public class MacroAbilitySO : ScriptableObject
{
    public string abilityName;
    public string serieName;
    public PlayersStats abilitySeries;
    public PlayersStats ability;
    public Sprite activeIcon;
    public Sprite inActiveIcon;
    public int level;
    public float value;
    public StatBoostType valueType;
    public int cost;
    public string description;
    public string fakeDescription;
    public bool luckDepending;
}
