using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

public class AutobattleUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    private CanvasGroup canvas;
    private Autobattle autobattle;
    private BattleManager battleManager;
    private PlayersArmy playersArmyScript;

    public Toggle[] attackToggles;
    public Toggle manaToggle;

    public string victoryText = "Victory!";
    public string defeatText = "Defeat";

    public TMP_Text resultText;
    public TMP_Text lossesText;
    public TMP_Text healthText;
    public TMP_Text manaText;

    [Inject]
    public void Construct(BattleManager battleManager, PlayersArmy playersArmyScript)
    {
        this.battleManager = battleManager;
        this.playersArmyScript = playersArmyScript;

        canvas = uiPanel.GetComponentInChildren<CanvasGroup>();
    }

    public void ShowResult(Autobattle autobattleScript, ResultOfAutobattle result)
    {
        autobattle = autobattleScript;

        MenuManager.instance.MiniPause(true);

        playersArmyScript.StopControlUnitDeath(false);
        uiPanel.SetActive(true);
        Fading.instance.Fade(true, canvas);

        FillWindow(result);
    }

    public void FillWindow(ResultOfAutobattle result)
    {
        resultText.text = (result.result == true) ? victoryText : defeatText;

        if(result.minLosses != result.maxLosses)
        {
            lossesText.text = result.minLosses + "-" + result.maxLosses;
        }
        else
        {
            lossesText.text = result.losses.ToString();
        }

        healthText.text = (result.healthLosses != 0) ? "-" + result.healthLosses : "0";
        manaText.text = (result.manaLosses != 0) ? "-" + result.manaLosses : "0";
    }

    public void Recalculating()
    {
        autobattle.Recalculating();
    }

    public void Fight()
    {
        autobattle.AcceptResult();
        CloseWindow(false);
    }

    public bool GetAttackMode()
    {
        if(attackToggles[0].isOn == true) return true;

        return false;
    }

    public bool GetManaMode()
    {
        if(manaToggle.isOn == true) return true;

        return false;
    }

    public void CloseWindow(bool isCancel = true)
    {
        if(isCancel == true)
        {
            battleManager.ReopenPreBattleWindow();
            playersArmyScript.StopControlUnitDeath(true);
        }

        uiPanel.SetActive(false);

        MenuManager.instance.MiniPause(false);
    }
}
