using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Enums;

public class GMInterfaceTurn : MonoBehaviour
{
    [SerializeField] private CanvasGroup container;
    [SerializeField] private Image messageImage;
    [SerializeField] private TMP_Text message;
    [SerializeField] private TMP_Text title;

    private string enemyTitle = "Enemy's turn:";
    private string heroTitle = "Hero's turn";

    public void ActivateTurnBlock(bool activateMode)
    {
        if(activateMode == true)
        {
            //container.gameObject.SetActive(activateMode);
            Fading.instance.Fade(true, container);
        }
        else
        {
            Fading.instance.Fade(false, container, activeMode: false);
        }
    }

    public void FillMessage(bool enemyMode, string enemyName, Color enemyColor)
    {
        if(enemyMode == true)
        {
            title.text = enemyTitle;
            message.text = enemyName + " is moving";
        }
        else
        {
            title.text = heroTitle;
            message.text = "";
        }

        messageImage.color = enemyColor;
    }
}
