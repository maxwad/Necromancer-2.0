using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class GMInterfaceTurn : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private Image messageImage;
    [SerializeField] private TMP_Text message;

    public void ActivateTurnBlock(bool activateMode)
    {
        container.SetActive(activateMode);
    }

    public void FillMessage(string enemyName, Color enemyColor)
    {
        message.text = enemyName + " is moving";
        messageImage.color = enemyColor;
    }
}
