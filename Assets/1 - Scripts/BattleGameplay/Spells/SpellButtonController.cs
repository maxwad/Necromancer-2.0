using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NameManager;

public class SpellButtonController : MonoBehaviour
{
    private ResourcesManager resourcesManager;

    [SerializeField] private SpellStat spell;
    [SerializeField] private Image veil;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;

    [SerializeField] private TMP_Text numberOfSlotText;

    private bool disabledBecauseMana = false;
    private bool disabledBecauseDelay = false;
    private bool isDisabled = false;

    private Coroutine coroutine;
    private float checkTimeMana = 0.2f;
    private float checkTimeDelay = 0.1f;
    private WaitForSeconds checkManaPeriod;
    private WaitForSeconds checkDelay;

    private HeroController hero;
    private SpellLibrary spellLibrary;

    private TooltipTrigger tooltipTrigger;

    public void InitializeButton(SpellStat newSpell, int slot = -1)
    {
        hero = GlobalStorage.instance.hero;
        spellLibrary = GlobalStorage.instance.spellManager.GetComponent<SpellLibrary>();
        resourcesManager = GlobalStorage.instance.resourcesManager;

        spell = newSpell;

        icon = GetComponent<Image>();
        icon.sprite = spell.icon;

        button = GetComponent<Button>();

        if(slot != -1) numberOfSlotText.text = slot.ToString();

        button.onClick.AddListener(ActivateSpell);

        tooltipTrigger = GetComponent<TooltipTrigger>();
        if(tooltipTrigger != null) tooltipTrigger.content = spell.description + " (Cost: " + spell.manaCost + ")";

        if(coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(CheckDisabling());

    }

    public void ActivateSpell()
    {
        if(isDisabled == true) return;

        if(MenuManager.instance.IsTherePauseOrMiniPause() == true) return;

        if(hero.isDead == true) return;

        resourcesManager.ChangeResource(ResourceType.Mana, -spell.manaCost);
        spellLibrary.ActivateSpell(spell.spell, true, spell.value, spell.actionTime);

        StartCoroutine(StartDelay());
    }

    private bool CheckMana()
    {
        //float currentManaCount = resourcesManager.GetResource(ResourceType.Mana);
        //return !(currentManaCount - spell.manaCost >= 0);
        return !resourcesManager.CheckMinResource(ResourceType.Mana, spell.manaCost);
    }

    private IEnumerator CheckDisabling()
    {
        checkManaPeriod = new WaitForSeconds(checkTimeMana);

        while(true)
        {
            yield return checkManaPeriod;

            disabledBecauseMana = CheckMana();

            if(disabledBecauseDelay == true || disabledBecauseMana == true)
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

        veil.gameObject.SetActive(true);
        float veilStep = 1 / (spell.reloading / checkTimeDelay);

        while(currentWaitingTime <= spell.reloading)
        {
            currentWaitingTime += checkTimeDelay;
            yield return checkDelay;

            veil.fillAmount -= veilStep;
        }

        veil.gameObject.SetActive(false);
        veil.fillAmount = 1;
        disabledBecauseDelay = false;
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(ActivateSpell);
    }
}
