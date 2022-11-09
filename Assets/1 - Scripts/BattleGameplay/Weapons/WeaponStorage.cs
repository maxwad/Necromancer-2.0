using System.Collections;
using UnityEngine;
using static NameManager;

public class WeaponStorage : MonoBehaviour
{
    [HideInInspector] public bool isBibleWork = false;
    private ObjectsPoolManager objectsPool;
    private BattleBoostManager boostManager;
    [SerializeField] private GameObject weaponContainer;

    public void Attack(Unit unit)
    {
        switch(unit.unitAbility)
        {
            case UnitsAbilities.Whip:
                WhipAction(unit);
                break;

            case UnitsAbilities.Garlic:
                GarlicAction(unit);                
                break;

            case UnitsAbilities.Axe:
                AxeAction(unit);
                break;

            case UnitsAbilities.Spear:
                SpearAction(unit);
                break;

            case UnitsAbilities.Bible:
                if(isBibleWork == false) BibleAction(unit);
                break;

            case UnitsAbilities.Bow:
                BowAction(unit);
                break;

            case UnitsAbilities.Knife:
                KnifeAction(unit);
                break;

            case UnitsAbilities.Bottle:
                BottleAction(unit);
                break;

            default:
                break;
        }
    }

    #region Helpers

    private GameObject CreateWeapon(Unit unit)
    {
        if(objectsPool == null) 
        { 
            objectsPool = GlobalStorage.instance.objectsPoolManager;
            boostManager = GlobalStorage.instance.boostManager;
        }

        if(unit.unitController == null) return null;

        GameObject weapon = objectsPool.GetWeapon(unit.unitAbility);

        weapon.transform.position = unit.unitController.gameObject.transform.position + new Vector3 (0, transform.localScale.y / 2, 0);
        float weaponSize = unit.size + unit.size * boostManager.GetBoost(BoostType.WeaponSize);

        weapon.transform.localScale = new Vector3(weaponSize, weaponSize, weaponSize);
        weapon.transform.SetParent(weaponContainer.transform);
        weapon.SetActive(true);

        weapon.GetComponent<WeaponDamage>().SetSettings(unit);
        weapon.GetComponent<WeaponMovement>().SetSettings(unit);

        return weapon;
    }

    #endregion


    private void WhipAction(Unit unit) 
    {
        float normalYAngle = 0f;
        float flipYAngle = 180f;
        float zAngle = 25f;

        if(unit.level == 1)
        {
            CreateConfiguredWeapon(flipYAngle, normalYAngle);           
        }

        if(unit.level == 2)
        {
            CreateConfiguredWeapon(flipYAngle, normalYAngle);

            CreateConfiguredWeapon(normalYAngle, flipYAngle);            
        }

        if(unit.level == 3)
        {
            CreateConfiguredWeapon(normalYAngle, flipYAngle, zAngle);
            CreateConfiguredWeapon(normalYAngle, flipYAngle, -zAngle);

            CreateConfiguredWeapon(flipYAngle, normalYAngle, zAngle);
            CreateConfiguredWeapon(flipYAngle, normalYAngle, -zAngle);            
        }

        void CreateConfiguredWeapon(float normalAngleY, float flipAngleY, float angleZ = 0)
        {
            GameObject itemWeapon = CreateWeapon(unit);
            if(itemWeapon == null) return;
            float yAngle = unit.unitController.unitSprite.flipX == true ? normalAngleY : flipAngleY;
            itemWeapon.transform.eulerAngles = new Vector3(itemWeapon.transform.eulerAngles.x, yAngle, angleZ);
        }
    }


    private void GarlicAction(Unit unit) 
    {
        GameObject weapon = CreateWeapon(unit);
        if(weapon == null) return;
        weapon.GetComponent<WeaponMovement>().ActivateWeapon(unit);
    }


    private void AxeAction(Unit unit)
    {
        float axeAngleLvl1 = 20;
        float axeAngleLvl2 = 45;

        if(unit.level == 1)
        {
            CreateConfiguredWeapon(1, 0);            
        }

        if(unit.level == 2)
        {
            CreateConfiguredWeapon(1, axeAngleLvl1);

            CreateConfiguredWeapon(2, -axeAngleLvl1);
        }

        if(unit.level == 3)
        {
            CreateConfiguredWeapon(1, axeAngleLvl2);

            CreateConfiguredWeapon(2, 0);

            CreateConfiguredWeapon(3, -axeAngleLvl2);
        }

        void CreateConfiguredWeapon(int index, float angleZ = 0)
        {
            GameObject weapon = CreateWeapon(unit);
            if(weapon == null) return;
            weapon.transform.eulerAngles = new Vector3(weapon.transform.eulerAngles.x, weapon.transform.eulerAngles.y, angleZ);
            weapon.GetComponent<WeaponMovement>().ActivateWeapon(unit, index);
        }
    }


    private void SpearAction(Unit unit) 
    {
        StartCoroutine(CreateSpears(unit.level));

        IEnumerator CreateSpears(int count)
        {
            for(int i = 0; i < count; i++)
            {
                GameObject itemWeapon = CreateWeapon(unit);
                if(itemWeapon == null) break;
                itemWeapon.GetComponent<WeaponMovement>().ActivateWeapon(unit);
                yield return new WaitForSeconds(0.2f);
            }
        }        
    }


    private void BibleAction(Unit unit)
    {
        isBibleWork = true;

        float bibleAngleLvl2_2 = 180;

        float bibleAngleLvl3_2 = 120;
        float bibleAngleLvl3_3 = 240;

        if(unit.level == 1)
        {
            CreateConfiguredWeapon();
        }

        if(unit.level == 2)
        {
            CreateConfiguredWeapon();

            CreateConfiguredWeapon(bibleAngleLvl2_2);
        }

        if(unit.level == 3)
        {
            CreateConfiguredWeapon();

            CreateConfiguredWeapon(bibleAngleLvl3_2);

            CreateConfiguredWeapon(bibleAngleLvl3_3);
        }

        void CreateConfiguredWeapon(float angleZ = 0)
        {
            GameObject weapon = CreateWeapon(unit);
            if(weapon == null) return;
            weapon.transform.eulerAngles = new Vector3(0, 0, angleZ);

            GameObject weaponInner = weapon.transform.GetChild(0).gameObject;
            //weaponInner.transform.Rotate(0, 0, -angleZ);
            weaponInner.transform.localRotation = Quaternion.Euler(0, 0, -angleZ);

            weapon.GetComponent<WeaponMovement>().ActivateWeapon(unit);
        }
    }


    private void BowAction(Unit unit)
    {
        StartCoroutine(CreateBow());

        IEnumerator CreateBow()
        {
            for(int i = 0; i < unit.level; i++)
            {
                GameObject itemWeapon = CreateWeapon(unit);
                if(itemWeapon == null) break;
                Vector2 mouseOnScreen = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 weaponPosition = unit.unitController.gameObject.transform.position;                
                float angle = Mathf.Atan2(weaponPosition.y - mouseOnScreen.y, weaponPosition.x - mouseOnScreen.x) * Mathf.Rad2Deg;
                
                itemWeapon.transform.eulerAngles = new Vector3(0, 0, angle);
                itemWeapon.GetComponent<WeaponMovement>().ActivateWeapon(unit);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }


    private void KnifeAction(Unit unit)
    {
        if(unit.level == 1) CreateKnife(new float[] { 0 });

        if(unit.level == 2) CreateKnife(new float[] { 10, -10 });

        if(unit.level == 3) CreateKnife(new float[] { 15, 0, -15 });

        void CreateKnife(float[] offsetAngles)
        {
            for(int i = 0; i < offsetAngles.Length; i++)
            {
                GameObject itemWeapon = CreateWeapon(unit);
                if(itemWeapon == null) break;
                itemWeapon.transform.eulerAngles = new Vector3(0, 0, GetAngleY(itemWeapon) + offsetAngles[i]);
                itemWeapon.GetComponent<WeaponMovement>().ActivateWeapon(unit);                
            }
        }

        float GetAngleY(GameObject weapon)
        {
            float angleZ = 0;

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            if(x == 0 )
            {
                if(y == 0) angleZ = unit.unitController.unitSprite.flipX == true ? -180 : 0;

                if(y == 0 && unit.unitController.unitSprite.flipX == true) weapon.GetComponent<SpriteRenderer>().flipY = true;

                if(y > 0) angleZ = -90;

                if(y < 0) angleZ = 90;                                
            }

            if(x > 0)
            {                
                if(y == 0) angleZ = 180;

                if(y > 0) angleZ = 225;

                if(y < 0) angleZ = 135;

                weapon.GetComponent<SpriteRenderer>().flipY = true;
            }

            if(x < 0)
            {
                if(y == 0) angleZ = 0;

                if(y > 0) angleZ = -45;

                if(y < 0) angleZ = 45;
            }

            return angleZ;
        }
    }


    private void BottleAction(Unit unit)
    {
        StartCoroutine(CreateBottle());

        IEnumerator CreateBottle()
        {
            for(int i = 0; i < unit.level; i++)
            {
                GameObject itemWeapon = CreateWeapon(unit);
                if(itemWeapon == null) break;
                itemWeapon.transform.eulerAngles = new Vector3(0, 0, 0);
                itemWeapon.GetComponent<WeaponMovement>().ActivateWeapon(unit);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
