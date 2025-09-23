using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Enums;

public class BattleBossUI : MonoBehaviour
{
    [SerializeField] private Image bossIcon;
    [SerializeField] private Image runeIcon;
    [SerializeField] private TooltipTrigger tooltip;
    [SerializeField] private Image healthScale;
    [SerializeField] private TMP_Text healthCount;
    private float maxHealth;

    public void Init(Sprite bossPict, Sprite runePict, string tip, float health)
    {
        bossIcon.sprite = bossPict;
        runeIcon.sprite = runePict;
        tooltip.content = "Current effect (kill this boss to get reverse effect): " + tip;
        maxHealth = Mathf.Round(health);
        healthCount.text = maxHealth + "/" + maxHealth;
    }

    public void UpdateHealth(float currentH)
    {
        float value = (currentH > 0 && currentH < 1) ? 1 : Mathf.Round(currentH);
        
        healthScale.fillAmount = value / maxHealth;
        healthCount.text = value + "/" + maxHealth;
    }
}
