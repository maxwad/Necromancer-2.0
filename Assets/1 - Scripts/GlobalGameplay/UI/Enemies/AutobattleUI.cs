using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutobattleUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    private CanvasGroup canvas;
    private Autobattle autobattle;
    private BattleManager battleManager;
    private BattleResult battleResult;

    public Toggle[] attackToggles;
    public Toggle manaToggle;

    public string victoryText = "Victory!";
    public string defeatText = "Defeat";

    public TMP_Text resultText;
    public TMP_Text lossesText;
    public TMP_Text healthText;
    public TMP_Text manaText;

    public void ShowResult(Autobattle autobattleScript, ResultOfAutobattle result)
    {
        if(autobattle == null) 
        {
            autobattle = autobattleScript;
            battleManager = GlobalStorage.instance.battleManager;
            battleResult = GetComponent<BattleResult>();
            canvas = uiPanel.GetComponentInChildren<CanvasGroup>();
        }

        MenuManager.instance.MiniPause(true);
        GlobalStorage.instance.ModalWindowOpen(true);

        battleResult.StartCheckingUnitDeath();
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
            battleResult.CloseCheckingUnitDeath();
        }

        uiPanel.SetActive(false);

        MenuManager.instance.MiniPause(false);
        GlobalStorage.instance.ModalWindowOpen(false);
    }
}
