using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NameManager;

public class HealthObjectStats : MonoBehaviour
{
    private BattleBoostManager boostManager;

    public bool isFromPool = false;

    public bool isImmortal = false;
    public float health = 10f;
    private float currentHealth;

    private SpriteRenderer sprite;
    private Color normalColor;
    private Color damageColor = Color.gray;
    private float blinkTime = 0.1f;

    [SerializeField] private GameObject damageNote;
    private Color colorDamage = Color.white;

    [SerializeField] private GameObject deathPrefab;

    public ObstacleTypes typeOfObject;
    public BonusType[] bonusTypes;
    public int[] bonusesPropabilities;

    private void Start()
    {
        boostManager = GlobalStorage.instance.boostManager;
        currentHealth = health;
        sprite = GetComponent<SpriteRenderer>();
        if(sprite != null) normalColor = sprite.color;
    }

    public void TakeDamage(float physicalDamage, float magicDamage)
    {
        if(isImmortal == false)
        {
            sprite.color = damageColor;
            Invoke("ColorBack", blinkTime);

            float damage = physicalDamage + magicDamage;
            currentHealth -= damage;

            ShowDamage(damage, colorDamage);

            if(currentHealth <= 0) Dead();
        }
    }

    private void ColorBack()
    {
        sprite.color = normalColor;
    }

    private void ShowDamage(float damageValue, Color colorDamage)
    {
        GameObject damageObject = GlobalStorage.instance.objectsPoolManager.GetObject(ObjectPool.DamageText);
        damageObject.transform.position = transform.position;
        damageObject.SetActive(true);
        damageObject.GetComponent<DamageText>().Iniatilize(damageValue, colorDamage);
    }

    private void Dead()
    {
        GameObject death = Instantiate(deathPrefab, transform.position, Quaternion.identity);
        death.transform.SetParent(GlobalStorage.instance.effectsContainer.transform);

        CreateBonus();
        currentHealth = health;
        EventManager.OnObstacleDestroyedEvent(gameObject);

    }

    private void CreateBonus()
    {
        BonusType bonus = RandomBonus();
        if(bonus != BonusType.Nothing) GlobalStorage.instance.bonusManager.CreateBonus(false, bonus, transform.position);

    }

    private BonusType RandomBonus()
    {
        int randomNumb = Random.Range(0, 101);
        int checkSumm = 0;

        for(int i = 0; i < bonusTypes.Length; i++)
        {
            checkSumm += (bonusesPropabilities[i] + bonusesPropabilities[i] * (int)boostManager.GetBoost(BoostType.BonusOpportunity));

            if(randomNumb <= checkSumm) return bonusTypes[i];
        }

        return BonusType.Nothing;
    }

    public void DestroyMe()
    {
        if(isFromPool == true)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        if(typeOfObject == ObstacleTypes.Obstacle) GetComponent<Collider2D>().enabled = false;
    }

    private void OnBecameVisible()
    {
        if(typeOfObject == ObstacleTypes.Obstacle) GetComponent<Collider2D>().enabled = true;
    }

    private void OnEnable()
    {
        EventManager.EndOfBattle += DestroyMe;
    }

    private void OnDisable()
    {
        EventManager.EndOfBattle -= DestroyMe;
    }

}

