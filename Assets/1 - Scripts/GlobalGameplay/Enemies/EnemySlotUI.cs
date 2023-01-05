using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text amount;
    private InfotipTrigger squadtipTrigger;

    public void Initialize(EnemySO enemy, int count)
    {
        if(squadtipTrigger == null) squadtipTrigger = GetComponent<InfotipTrigger>();
        squadtipTrigger.SetEnemy(enemy);

        icon.sprite = enemy.enemyIcon;
        amount.text = count.ToString();
    }
}
