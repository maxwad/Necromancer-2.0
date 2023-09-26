using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;
using Zenject;

public class BattleUIInfirmaryPart : MonoBehaviour
{
    private BattleUIManager battleUIManager;
    private PlayerStats playerStats;
    private InfirmaryManager infirmaryManager;

    [Header("Infirmary")]
    [SerializeField] private Image infirmaryScale;
    [SerializeField] private TMP_Text infirmaryInfo;
    private float currentMaxInfirmaryCount;
    private float currentInfirmaryCount;
    [SerializeField] private Color infirmaryUpColor;
    [SerializeField] private Color infirmaryDownColor;
    [SerializeField] private Color normalInfirmaryColor;

    [Inject]
    public void Construct(PlayerStats playerStats, InfirmaryManager infirmaryManager)
    {
        this.playerStats = playerStats;
        this.infirmaryManager = infirmaryManager;
    }

    public void Init(BattleUIManager manager)
    {
        battleUIManager = manager;
    }

    public void FillInfirmary(float max = 0, float current = 0)
    {
        Color blinkColor = infirmaryUpColor;
        if(max == 0)
        {
            currentMaxInfirmaryCount = playerStats.GetCurrentParameter(PlayersStats.Infirmary);
            currentInfirmaryCount = infirmaryManager.GetInjuredCount();
        }
        else
        {
            if(currentInfirmaryCount > current) blinkColor = infirmaryDownColor;
            currentMaxInfirmaryCount = max;
            currentInfirmaryCount = current;
        }

        float widthInfirmary = currentInfirmaryCount / currentMaxInfirmaryCount;

        infirmaryScale.fillAmount = widthInfirmary;
        infirmaryInfo.text = currentInfirmaryCount.ToString() + "/" + currentMaxInfirmaryCount.ToString();
        battleUIManager.Blink(infirmaryScale, blinkColor, normalInfirmaryColor);
    }

    private void UpdateInfirmaryUI(float quantity, float capacity)
    {
        FillInfirmary(capacity, quantity);
    }

    private void OnEnable()
    {
        EventManager.UpdateInfirmaryUI += UpdateInfirmaryUI;
    }

    private void OnDisable()
    {
        EventManager.UpdateInfirmaryUI -= UpdateInfirmaryUI;
    }

}
