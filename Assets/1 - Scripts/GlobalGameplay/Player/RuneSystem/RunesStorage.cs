using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunesStorage : MonoBehaviour
{
    [SerializeField] private List<RuneSO> runes;
    [HideInInspector] public List<RuneSO> availableRunes = new List<RuneSO>();

    public void Init()
    {
        availableRunes = runes;
    }

    public List<RuneSO> GetAvailableRunes()
    {
        return availableRunes;
    }
}
