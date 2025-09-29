using Enums;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class SpellButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ResourcesManager resourcesManager;
    private BoostManager boostManager;
    private HeroController hero;
    private SpellLibrary spellLibrary;
    private BattleUISpellPart battleUISpellPart;
    private PreSpellLibrary preSpellLibrary;

    private SpellSO spell;
    [SerializeField] private Image veil;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;

    [SerializeField] private TMP_Text numberOfSlotText;

    [SerializeField] private Color emptyColor;
    [SerializeField] private Color activeColor;

    private bool isEmptyButton = true;
    private bool disabledBecauseMana = false;
    private bool disabledBecauseDelay = false;
    private bool disabledBecauseBattleIsOver = false;
    private bool isDisabled = false;

    private Coroutine coroutine;
    private float checkTimeMana = 0.2f;
    private float checkTimeDelay = 0.1f;
    private WaitForSeconds checkManaPeriod;
    private WaitForSeconds checkDelay;

    [SerializeField] private TooltipTrigger tooltipTrigger;
    [SerializeField] private InfotipTrigger infotip;

    [Inject]
    public void Construct(
            ResourcesManager resourcesManager,
            BoostManager boostManager,
            HeroController hero,
            SpellManager spellManager
        )
    {
        this.resourcesManager = resourcesManager;
        this.boostManager = boostManager;
        this.hero = hero;

        spellLibrary = spellManager.GetComponent<SpellLibrary>();
        preSpellLibrary = spellLibrary.GetComponent<PreSpellLibrary>();
    }

    public void PreInit(BattleUISpellPart uiPart, int numberOfSlot)
    {
        tooltipTrigger.enabled = true;
        tooltipTrigger.content = "This slot is empty";
        infotip.enabled = false;

        battleUISpellPart = uiPart;

        numberOfSlotText.text = numberOfSlot.ToString();

        isEmptyButton = true;
        icon.sprite = null;
        icon.color = emptyColor;
    }

    public void InitializeButton(SpellSO newSpell)
    {
        isEmptyButton = false;
        spell = newSpell;

        icon.sprite = spell.icon;
        icon.color = activeColor;

        button.onClick.AddListener(ActivateSpell);

        string description = spell.description.Replace("$V", spell.value.ToString());
        description = description.Replace("$T", spell.actionTime.ToString());
        description = description.Replace("$R", spell.radius.ToString());
        description += " (Cost: " + spell.manaCost;
        description += " Reload: " + spell.reloading + " sec.)";

        tooltipTrigger.enabled = false;
        infotip.enabled = true;
        infotip.SetSpell(newSpell);

        if(coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(CheckDisabling());

    }

    public void ActivateSpell()
    {
        if(isEmptyButton == true) return;

        if(isDisabled == true) return;

        if(MenuManager.instance.IsTherePauseOrMiniPause() == true) return;

        if(hero.isDead == true) return;

        ActivatePreSpell(false);

        resourcesManager.ChangeResource(ResourceType.Mana, -spell.manaCost);

        if(spell.actionTime != 0)
        {
            if(battleUISpellPart.CheckSpell(spell) == true)
            {
                battleUISpellPart.ProlongSpell(spell);
            }
            else
            {
                spellLibrary.ActivateSpell(spell, true);
                battleUISpellPart.AddUISpellEffect(spell);
            }
        }
        else
        {
            spellLibrary.ActivateSpell(spell, true);
        }

        StartCoroutine(StartDelay());
    }

    private bool CheckMana()
    {
        return !resourcesManager.CheckMinResource(ResourceType.Mana, spell.manaCost);
    }

    private bool CheckBattleOver()
    {
        return battleUISpellPart.CheckBattleOver();
    }

    private IEnumerator CheckDisabling()
    {
        checkManaPeriod = new WaitForSeconds(checkTimeMana);

        while(true)
        {
            yield return checkManaPeriod;

            disabledBecauseMana = CheckMana();
            disabledBecauseBattleIsOver = CheckBattleOver();

            if(disabledBecauseDelay == true || disabledBecauseMana == true || disabledBecauseBattleIsOver == true)
                isDisabled = true;
            else
                isDisabled = false;

            if(isDisabled == true)
                icon.color = Color.red;
            else
                icon.color = Color.white;
        }
    }

    private IEnumerator StartDelay()
    {
        disabledBecauseDelay = true;

        checkDelay = new WaitForSeconds(checkTimeDelay);
        float currentWaitingTime = 0;
        float reloadingTime = spell.reloading + spell.reloading * boostManager.GetBoost(BoostType.SpellReloading);

        veil.gameObject.SetActive(true);
        float veilStep = 1 / (reloadingTime / checkTimeDelay);

        while(currentWaitingTime <= reloadingTime)
        {
            currentWaitingTime += checkTimeDelay;
            yield return checkDelay;

            veil.fillAmount -= veilStep;
        }

        veil.gameObject.SetActive(false);
        veil.fillAmount = 1;
        disabledBecauseDelay = false;
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(ActivateSpell);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isDisabled == true) return;

        if(MenuManager.instance.IsTherePauseOrMiniPause() == true) return;

        if(hero.isDead == true) return;

        ActivatePreSpell(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ActivatePreSpell(false);
    }

    private void ActivatePreSpell(bool mode)
    {
        if(spell != null && spell.hasPreSpell == true)
        {
            preSpellLibrary.Activate(spell, mode);
        }
    }
}
