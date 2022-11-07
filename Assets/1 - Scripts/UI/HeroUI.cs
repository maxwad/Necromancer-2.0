using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;


public class HeroUI : MonoBehaviour
{
    private CanvasGroup canvas;

    private PlayerStats playerStats;
    private MacroLevelUpManager macroLevelUpManager;

    public TMP_Text level;
    public TMP_Text health;
    public TMP_Text healthRegeneration;
    public TMP_Text mana;
    public TMP_Text manaRegeneration;
    public TMP_Text defence;
    //public TMP_Text speed;
    public TMP_Text movementPoints;
    public TMP_Text luck;

    public void ShowTip()
    {
        if(canvas == null) canvas = GetComponent<CanvasGroup>();
        Fading.instance.FadeWhilePause(true, canvas);
    }

    public void Init()
    {
        if(playerStats == null)
        {
            playerStats = GlobalStorage.instance.playerStats;
            macroLevelUpManager = GlobalStorage.instance.macroLevelUpManager;
        }

        level.text = macroLevelUpManager.GetCurrentLevel().ToString();
        health.text = playerStats.GetCurrentParameter(PlayersStats.Health).ToString();
        mana.text = playerStats.GetCurrentParameter(PlayersStats.Mana).ToString();
        defence.text = playerStats.GetCurrentParameter(PlayersStats.Defence).ToString();
        //speed.text = playerStats.GetCurrentParameter(PlayersStats.Speed).ToString();
        movementPoints.text = (Mathf.Round(playerStats.GetCurrentParameter(PlayersStats.MovementDistance))).ToString();
        luck.text = playerStats.GetCurrentParameter(PlayersStats.Luck).ToString();

        float hReg = playerStats.GetCurrentParameter(PlayersStats.HealthRegeneration);
        float mReg = playerStats.GetCurrentParameter(PlayersStats.ManaRegeneration);

        if(hReg == 0)
        {
            healthRegeneration.gameObject.SetActive(false);
        }
        else
        {
            healthRegeneration.gameObject.SetActive(true);
            healthRegeneration.text = "+" + hReg;
        }

        if(mReg == 0)
        {
            manaRegeneration.gameObject.SetActive(false);
        }
        else
        {
            manaRegeneration.gameObject.SetActive(true);
            manaRegeneration.text = "+" + mReg;
        }
    }


    
    private void OnEnable()
    {
        Init();
    }
}
