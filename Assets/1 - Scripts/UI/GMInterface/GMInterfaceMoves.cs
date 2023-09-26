using UnityEngine;
using TMPro;
using Zenject;

public class GMInterfaceMoves : MonoBehaviour
{
    private CalendarManager calendarManager;

    [Header("Moves")]
    [SerializeField] private GameObject movesContainer;
    [SerializeField] private TMP_Text currentMovesCount;
    [SerializeField] private TMP_Text leftDaysCount;
    [SerializeField] private Animator nextDayButton;

    [Inject]
    public void Construct(CalendarManager calendarManager)
    {
        this.calendarManager = calendarManager;
    }

    public void UpdateCurrentMoves(float currentValue)
    {
        currentMovesCount.text = currentValue.ToString();

        bool mode = (currentValue == 0) ? true : false;

        nextDayButton.SetBool(TagManager.A_BLINK, mode);
    }

    public void DaysLeft(int days)
    {
        leftDaysCount.text = days.ToString();
    }

    public void ShowBlock(bool mode)
    {
        movesContainer.SetActive(mode);
    }

    //BUTTON
    public void NextDay()
    {
        calendarManager.NextDay();
    }
}
