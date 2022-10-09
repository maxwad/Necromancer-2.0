using UnityEngine;
using static NameManager;

[CreateAssetMenu(fileName = "MacroAbilityData", menuName = "MacroAbilityItem")]

public class MacroAbilitySO : ScriptableObject
{
    public string abilityName;
    public PlayersStats abilitySeries;
    public PlayersStats ability;
    public Sprite activeIcon;
    public Sprite inActiveIcon;
    public int level;
    public float value;
    public StatBoostType valueType;
    public string fakeDescription;
    public string schemeDescription;
    public string realDescription;
    public bool luckDepending;

    private void OnEnable()
    {
        realDescription = schemeDescription.Replace("$", value.ToString());
    }
}
