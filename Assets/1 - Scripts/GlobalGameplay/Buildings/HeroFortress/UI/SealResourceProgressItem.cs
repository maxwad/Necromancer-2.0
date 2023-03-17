using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SealResourceProgressItem : MonoBehaviour
{
    [SerializeField] private Toggle resToggle;
    [SerializeField] private Image resIcon;
    [SerializeField] private Image progressScale;
    [SerializeField] private TMP_Text progressText;

    private Sanctuary sanctuary;
    private int index = 0;
    private float maxAmount;
    private float currentAmount = 0;
    private bool isComplete = false;

    public void Init(Sanctuary sanct, int i, Cost cost, Color color, Sprite icon)
    {
        sanctuary = sanct;
        index = i;
        maxAmount = cost.amount;

        if(i == 0) resToggle.isOn = true;

        resIcon.sprite = icon;
        progressScale.color = color;

        UpdateScale();
    }

    //Toggle
    public void ChangeResource()
    {
        if(resToggle.isOn == true)
        {
            if(sanctuary != null)
            {
                if(isComplete == false)
                    sanctuary.SelectSlider(index, maxAmount - currentAmount);
                else
                    sanctuary.SelectAnoterSlider();
            }
        }
    }

    public float GetAmount()
    {
        return currentAmount;
    }

    public bool GetStatus()
    {
        return isComplete;
    }

    public void SetActive()
    {
        resToggle.isOn = true;
    }

    public void AddResource(float amount)
    {
        currentAmount += amount;
        UpdateScale();
        isComplete = currentAmount >= maxAmount;

        if(isComplete == true)
            resToggle.interactable = false;

        ChangeResource();
    }

    private void UpdateScale()
    {
        progressScale.fillAmount = currentAmount / maxAmount;
        progressText.text = currentAmount + "/" + maxAmount;
    }
}
