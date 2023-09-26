using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InfirmarySlot : MonoBehaviour
{
    private PlayerStats playerStats;

    [SerializeField] public GameObject content;
    [SerializeField] public Image icon;
    [SerializeField] public Image timerVeil;
    [SerializeField] public TMP_Text quantity;

    public Color grey;
    public Color red;

    [SerializeField] private InfotipTrigger squadtipTrigger;
    [SerializeField] private TooltipTrigger tooltipTrigger;

    [Inject]
    public void Construct(PlayerStats playerStats)
    {
        this.playerStats = playerStats;
    }

    public void ResetSlot()
    {
        content.SetActive(false);
        squadtipTrigger.SetUnit(null);
    }

    public void FillTheInfarmarySlot(Sprite pict, int count, float days)
    {
        float infirmaryCapacity = playerStats.GetCurrentParameter(NameManager.PlayersStats.InfirmaryTime);

        content.SetActive(true);
        
        tooltipTrigger.content = "Next unit from this squad will die in " + days + " day(s)";
        icon.sprite = pict;
        timerVeil.fillAmount = 1 - (days / infirmaryCapacity);
        quantity.text = count.ToString();        
    }
}
