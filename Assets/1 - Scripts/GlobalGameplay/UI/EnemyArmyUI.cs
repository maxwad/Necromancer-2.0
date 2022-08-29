using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static NameManager;

public class EnemyArmyUI : MonoBehaviour
{
    public GameObject uiPanel;
    private GameObject currentEnemyArmy;
    private bool isEnemyArmysWindowOpened = true;

    public TMP_Text caption;
    public Image resourceImage;
    public TMP_Text count;

    public void OpenWindow(bool modeClick, ClickableObject enemyArmy)
    {
        GlobalStorage.instance.isModalWindowOpen = true;
        isEnemyArmysWindowOpened = true;

        currentEnemyArmy = (enemyArmy == null) ? null : enemyArmy.gameObject;

        uiPanel.SetActive(true);

        Initialize();
    }

    private void Initialize()
    {
        EnemyArmyOnTheMap obj = currentEnemyArmy.GetComponent<EnemyArmyOnTheMap>();
        if(obj != null)
        {
            caption.text = obj.gameObject.name;
            if(GlobalStorage.instance.playerStats.GetCurrentParameter(PlayersStats.Curiosity) > 1)
                count.text = obj.commonCount.ToString();
            else
                count.text = "Some";

            resourceImage.sprite = obj.GetComponent<SpriteRenderer>().sprite;
            resourceImage.color = obj.GetComponent<SpriteRenderer>().color;

        }
    }


    public void CloseWindow()
    {
        GlobalStorage.instance.isModalWindowOpen = false;
        isEnemyArmysWindowOpened = false;

        uiPanel.SetActive(false);
    }
}
