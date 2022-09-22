using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text amount;
    private SquadtipTrigger squadtipTrigger;

    public void Initialize(EnemyController enemy, int count)
    {
        if(squadtipTrigger == null) squadtipTrigger = GetComponent<SquadtipTrigger>();
        squadtipTrigger.SetEnemy(enemy);

        icon.sprite = enemy.icon;
        amount.text = count.ToString();
    }
}
