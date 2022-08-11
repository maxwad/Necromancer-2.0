using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using static NameManager;

public class SpellButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SpellStat spell;
    [SerializeField] private Image veil;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;

    [SerializeField] private GameObject descriptionGO;
    [SerializeField] private TMP_Text description;

    [SerializeField] private TMP_Text numberOfSlotText;
    //private EventTrigger eventTrigger;

    private bool disabledBecauseMana = false;
    private bool disabledBecauseDelay = false;
    private bool isDisabled = false;

    private Coroutine coroutine;
    private float checkTimeMana = 0.2f;
    private float checkTimeDelay = 0.1f;
    private WaitForSeconds checkMana;
    private WaitForSeconds checkDelay;

    private Coroutine descriptionCoroutine;
    private float checkTimeDescription = 0.5f;
    private float timeStep = 0.1f;
    private WaitForSecondsRealtime descriptionDelay;

    private HeroController hero;
    private SpellLibrary spellLibrary;

    public void SetSpellOnButton(SpellStat newSpell)
    {
        spell = newSpell;
        description.text = newSpell.description + " (Cost: " + newSpell.manaCost + ")";
        descriptionGO.SetActive(false);
    }

    public void InitializeButton(int slot = -1)
    {
        hero = GlobalStorage.instance.hero;
        spellLibrary = GlobalStorage.instance.spellManager.GetComponent<SpellLibrary>();

        icon = GetComponent<Image>();
        icon.sprite = spell.icon;

        button = GetComponent<Button>();

        if(slot != -1) numberOfSlotText.text = slot.ToString();

        button.onClick.AddListener(ActivateSpell);

        if(coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(CheckDisabling());

    }

    public void ActivateSpell()
    {
        if(isDisabled == true) return;

        if(MenuManager.isGamePaused == true || MenuManager.isMiniPause == true) return;

        hero.SpendMana(spell.manaCost);
        spellLibrary.ActivateSpell(spell.spellType, true, spell.value, spell.actionTime);

        StartCoroutine(StartDelay());
    }

    private bool CheckMana()
    {
        float currentManaCount = hero.currentMana;
        return !(currentManaCount - spell.manaCost >= 0);
    }

    private IEnumerator CheckDisabling()
    {
        checkMana = new WaitForSeconds(checkTimeMana);

        while(true)
        {
            yield return checkMana;

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

    private IEnumerator ShowDescription()
    {
        descriptionDelay = new WaitForSecondsRealtime(timeStep);
        float currentWaitTime = 0;

        while(currentWaitTime < checkTimeDescription)
        {
            currentWaitTime += timeStep;
            yield return descriptionDelay;
        }

        descriptionGO.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionCoroutine = StartCoroutine(ShowDescription());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine(descriptionCoroutine);
        descriptionGO.SetActive(false);
    }
}
