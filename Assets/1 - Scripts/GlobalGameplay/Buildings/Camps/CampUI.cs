using UnityEngine;
using TMPro;

public class CampUI : MonoBehaviour
{
    private CanvasGroup canvas;
    private CampManager campManager;
    private CampGame campGame;

    [Header("Windows")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject mainWindow;
    [SerializeField] private GameObject miniWindow;
    [SerializeField] private GameObject emptyWindow;
    [SerializeField] private GameObject remoteWindow;
    [SerializeField] private GameObject confirmWindow;
    [SerializeField] private GameObject instructionLabel;
    [SerializeField] private GameObject resultLabel;

    [Header("Start Parameters")]
    [SerializeField] private TMP_Text totalCells;
    [SerializeField] private TMP_Text rewards;
    [SerializeField] private TMP_Text attempts;
    [SerializeField] private TMP_Text helpPoints;

    [SerializeField] private Sprite closedBonfire;

    private GameObject currentCamp;
    private CampGameParameters currentParameters;

    private void Start()
    {
        campManager = GlobalStorage.instance.campManager;
        campGame = GetComponent<CampGame>();

        canvas = uiPanel.GetComponent<CanvasGroup>();
    }

    public void Open(bool modeClick, GameObject camp)
    {
        MenuManager.instance.MiniPause(true);
        uiPanel.SetActive(true);

        currentCamp = camp;
        Init(modeClick);

        Fading.instance.FadeWhilePause(true, canvas);
    }

    //Button
    public void Close()
    {
        if(campGame.CheckGameStatus() == true)
            CloseCamp();

        campGame.CancelCorounite();

        mainWindow.SetActive(false);
        miniWindow.SetActive(false);

        MenuManager.instance.MiniPause(false);
        uiPanel.SetActive(false);
    }

    //Button
    public void TryToCloseGame()
    {
        if(campGame.CheckGameStatus() == true)
        {
            confirmWindow.SetActive(true);
        }
        else
        {
            Close();
        }
    }

    //Button
    public void CancelExit()
    {
        confirmWindow.SetActive(false);
    }

    public void Init(bool modeClick)
    {
        bool campStatus = campManager.GetCampStatus(currentCamp);
        Refactoring(modeClick, campStatus);

        if(campStatus == false) return;

        currentParameters = campManager.GetStartParameters();
        FillParameters();

        campGame.ResetCells();
        campGame.PreparedToGame(currentParameters);
    }

    private void Refactoring(bool modeClick, bool statusMode)
    {
        mainWindow.SetActive(false);
        miniWindow.SetActive(false);
        emptyWindow.SetActive(false);
        remoteWindow.SetActive(false);
        confirmWindow.SetActive(false);
        resultLabel.SetActive(false);
        instructionLabel.SetActive(true);

        if(modeClick == true)
        {
            miniWindow.SetActive(true);

            if(statusMode == true)
                remoteWindow.SetActive(true);
            else
                emptyWindow.SetActive(true);
        }
        else
        {
            if(statusMode == true)
            {
                mainWindow.SetActive(true);
            }
            else
            {
                miniWindow.SetActive(true);
                emptyWindow.SetActive(true);
            }
        }
    }

    public void FillParameters()
    {
        totalCells.text = currentParameters.cellsAmount.ToString();
        rewards.text = currentParameters.rewardsAmount.ToString();
        attempts.text = currentParameters.attempts.ToString();
        helpPoints.text = currentParameters.helps.ToString();
    }

    public void CloseCamp()
    {
        resultLabel.SetActive(true);
        instructionLabel.SetActive(false);
        SetCampEmpty(currentCamp);
        campManager.CloseCamp(currentCamp);
    }

    public void SetCampEmpty(GameObject camp)
    {
        SpriteRenderer campSprite = camp.GetComponent<SpriteRenderer>();
        campSprite.sprite = closedBonfire;
    }
}
