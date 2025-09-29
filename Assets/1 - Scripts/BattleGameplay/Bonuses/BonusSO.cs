using Enums;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BonusData", menuName = "BonusItem")]

public class BonusSO : ScriptableObject
{
    public BonusType bonusType;
    public float value;
    public List<Sprite> spritesSmall;
    public List<Sprite> spritesMiddle;
    public List<Sprite> spritesLarge;
}
