using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NameManager;

public class ObjectOwner : MonoBehaviour
{
    public TypeOfObjectsOwner owner;
    public bool isGuardNeeded = true;
    public float probabilityGuard = 100;

    private Color currentColor;
    public Color neutralColor;
    public Color enemyColor;
    public Color playerColor;

    public SpriteRenderer flagSpite;

    [SerializeField] private GameObject siegeBlock;
    [SerializeField] private TMP_Text term;

    public void ChangeOwner(TypeOfObjectsOwner newOwner)
    {
        if(newOwner != owner)
        {
            owner = newOwner;

            switch(owner)
            {
                case TypeOfObjectsOwner.Player:
                    currentColor = playerColor;
                    break;

                case TypeOfObjectsOwner.Enemy:
                    currentColor = enemyColor;
                    break;

                case TypeOfObjectsOwner.Nobody:
                    currentColor = neutralColor;
                    break;

                default:
                    break;
            }

            flagSpite.color = currentColor;
        }
    }


    public void StartSiege(bool siegeMode)
    {
        siegeBlock.SetActive(siegeMode);
    }

    public void UpdateSiegeTerm(string termStr)
    {
        term.text = termStr;
    }
}
