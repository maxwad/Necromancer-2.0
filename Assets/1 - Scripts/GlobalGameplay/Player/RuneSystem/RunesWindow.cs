using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunesWindow : MonoBehaviour
{
    [SerializeField] private RuneLevelWrapper levelRow;
    [SerializeField] private RunesRowWrapper firstRuneRow;
    [SerializeField] private RunesRowWrapper negativeRuneRow;
    [SerializeField] private RunesRowWrapper bonusRuneRowRow;

    private bool isNegativeRowUnlocked = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            UpdateWindow();
        }
    }

    public void UpdateWindow()
    {
        levelRow.Init();
        firstRuneRow.Init();
        negativeRuneRow.Init();
        bonusRuneRowRow.Init();
    }
}
