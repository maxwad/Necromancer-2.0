using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutobattleUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    private Autobattle autobattle;
    private BattleManager battleManager;

    public Toggle[] attackToggles;
    public Toggle manaToggle;

    public void ShowWindow(Autobattle autobattleScript)
    {
        if(autobattle == null) 
        {
            autobattle = autobattleScript;
            battleManager = GlobalStorage.instance.battleManager;
        }


        uiPanel.SetActive(true);
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

    public void CloseWindow()
    {
        battleManager.ReopenPreBattleWindow();
        uiPanel.SetActive(false);
    }
}
