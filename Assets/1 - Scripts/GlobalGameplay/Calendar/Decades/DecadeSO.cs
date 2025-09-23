using UnityEngine;
using static Enums;

[CreateAssetMenu(fileName = "DecadeData", menuName = "DecadeItem")]
public class DecadeSO : ScriptableObject
{
    public string decadeName;
    public RuneSO effect;
    public BoostEffect purpose;
    public bool isNegative;
}
