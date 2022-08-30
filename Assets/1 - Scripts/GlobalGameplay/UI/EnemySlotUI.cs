using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;
using UnityEngine.EventSystems;

public class EnemySlotUI : MonoBehaviour, IPointerEnterHandler
{
    public int index;
    public Image icon;
    public TMP_Text amount;
    public EnemyCardUI enemyCard;
    public EnemiesTypes enemyType;

    public void Initialize(int ind, Sprite pict, EnemyCardUI card, EnemiesTypes type)
    {
        index = ind;
        icon.sprite = pict;
        enemyCard = card;
        enemyType = type;
    }

    public void SetAmount(int count)
    {
        amount.text = count.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GlobalStorage.instance.enemyArmyUI.ShowDetails(index);
    }
}
