using System.Collections;
using UnityEngine;
using static NameManager;

public class WeaponStorage : MonoBehaviour
{
    [HideInInspector] public bool isBibleWork = false;

    public void Attack(UnitController unitController)
    {
        switch(unitController.unitAbility)
        {
            case UnitsAbilities.Whip:
                WhipAction(unitController);
                break;

            case UnitsAbilities.Garlic:
                GarlicAction(unitController);
                
                break;

            case UnitsAbilities.Axe:
                AxeAction(unitController);
                break;

            case UnitsAbilities.Spear:
                SpearAction(unitController);
                break;

            case UnitsAbilities.Bible:
                if(isBibleWork == false) BibleAction(unitController);
                break;

            case UnitsAbilities.Bow:
                BowAction(unitController);
                break;

            case UnitsAbilities.Knife:
                KnifeAction(unitController);
                break;

            case UnitsAbilities.Bottle:
                BottleAction(unitController);
                break;

            default:
                break;
        }
    }

    #region Helpers

    private GameObject CreateWeapon(UnitController unitController)
    {
        GameObject weapon = Instantiate(unitController.attackTool);

        weapon.transform.position = transform.position + new Vector3 (0, transform.localScale.y / 2, 0);
        weapon.transform.localScale = new Vector3(unitController.size, unitController.size, unitController.size);
        weapon.transform.SetParent(transform);

        weapon.GetComponent<WeaponDamage>().SetSettings(unitController);
        weapon.GetComponent<WeaponMovement>().SetSettings(unitController, this);

        return weapon;
    }

    #endregion


    private void WhipAction(UnitController unitController) 
    {
        float normalYAngle = 0f;
        float flipYAngle = 180f;
        float zAngle = 25f;

        if(unitController.level == 1)
        {
            CreateConfiguredWeapon(flipYAngle, normalYAngle);           
        }

        if(unitController.level == 2)
        {
            CreateConfiguredWeapon(flipYAngle, normalYAngle);

            CreateConfiguredWeapon(normalYAngle, flipYAngle);            
        }

        if(unitController.level == 3)
        {
            CreateConfiguredWeapon(normalYAngle, flipYAngle, zAngle);
            CreateConfiguredWeapon(normalYAngle, flipYAngle, -zAngle);

            CreateConfiguredWeapon(flipYAngle, normalYAngle, zAngle);
            CreateConfiguredWeapon(flipYAngle, normalYAngle, -zAngle);            
        }

        void CreateConfiguredWeapon(float normalAngleY, float flipAngleY, float angleZ = 0)
        {
            GameObject itemWeapon = CreateWeapon(unitController);
            float yAngle = unitController.unitSprite.flipX == true ? normalAngleY : flipAngleY;
            itemWeapon.transform.eulerAngles = new Vector3(itemWeapon.transform.eulerAngles.x, yAngle, angleZ);
        }
    }


    private void GarlicAction(UnitController unitController) 
    {
        GameObject weapon = CreateWeapon(unitController);
        weapon.GetComponent<WeaponMovement>().ActivateWeapon(unitController);
    }


    private void AxeAction(UnitController unitController)
    {
        float axeAngleLvl1 = 20;
        float axeAngleLvl2 = 45;

        if(unitController.level == 1)
        {
            CreateConfiguredWeapon(1, 0);            
        }

        if(unitController.level == 2)
        {
            CreateConfiguredWeapon(1, axeAngleLvl1);

            CreateConfiguredWeapon(2, -axeAngleLvl1);
        }

        if(unitController.level == 3)
        {
            CreateConfiguredWeapon(1, axeAngleLvl2);

            CreateConfiguredWeapon(2, 0);

            CreateConfiguredWeapon(3, -axeAngleLvl2);
        }

        void CreateConfiguredWeapon(int index, float angleZ = 0)
        {
            GameObject weapon = CreateWeapon(unitController);
            weapon.transform.eulerAngles = new Vector3(weapon.transform.eulerAngles.x, weapon.transform.eulerAngles.y, angleZ);
            weapon.GetComponent<WeaponMovement>().ActivateWeapon(unitController, index);
        }
    }


    private void SpearAction(UnitController unitController) 
    {
        StartCoroutine(CreateSpears(unitController.level));

        IEnumerator CreateSpears(int count)
        {
            for(int i = 0; i < count; i++)
            {
                GameObject itemWeapon = CreateWeapon(unitController);
                itemWeapon.GetComponent<WeaponMovement>().ActivateWeapon(unitController);
                yield return new WaitForSeconds(0.2f);
            }
        }        
    }


    private void BibleAction(UnitController unitController)
    {
        isBibleWork = true;

        float bibleAngleLvl2_2 = 180;

        float bibleAngleLvl3_2 = 120;
        float bibleAngleLvl3_3 = 240;

        if(unitController.level == 1)
        {
            CreateConfiguredWeapon();
        }

        if(unitController.level == 2)
        {
            CreateConfiguredWeapon();

            CreateConfiguredWeapon(bibleAngleLvl2_2);
        }

        if(unitController.level == 3)
        {
            CreateConfiguredWeapon();

            CreateConfiguredWeapon(bibleAngleLvl3_2);

            CreateConfiguredWeapon(bibleAngleLvl3_3);
        }

        void CreateConfiguredWeapon(float angleZ = 0)
        {
            GameObject weapon = CreateWeapon(unitController);
            weapon.transform.eulerAngles = new Vector3(0, 0, angleZ);

            GameObject weaponInner = weapon.transform.GetChild(0).gameObject;
            weaponInner.transform.Rotate(0, 0, -angleZ);

            weapon.GetComponent<WeaponMovement>().ActivateWeapon(unitController);
        }
    }


    private void BowAction(UnitController unitController)
    {
        if(unitController.level == 1) StartCoroutine(CreateBow(new float[] { 90 }));

        if(unitController.level == 2) StartCoroutine(CreateBow(new float[] { 90, 270 }));

        if(unitController.level == 3) StartCoroutine(CreateBow(new float[] { 0, 90, 180, 270 }));

        IEnumerator CreateBow(float[] angles)
        {
            for(int i = 0; i < angles.Length; i++)
            {
                GameObject itemWeapon = CreateWeapon(unitController);                
                itemWeapon.transform.eulerAngles = new Vector3(0, 0, angles[i]);
                itemWeapon.GetComponent<WeaponMovement>().ActivateWeapon(unitController);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }


    private void KnifeAction(UnitController unitController)
    {
        if(unitController.level == 1) CreateKnife(new float[] { 0 });

        if(unitController.level == 2) CreateKnife(new float[] { 10, -10 });

        if(unitController.level == 3) CreateKnife(new float[] { 15, 0, -15 });

        void CreateKnife(float[] offsetAngles)
        {
            for(int i = 0; i < offsetAngles.Length; i++)
            {
                GameObject itemWeapon = CreateWeapon(unitController);
                itemWeapon.transform.eulerAngles = new Vector3(0, 0, GetAngleY(itemWeapon) + offsetAngles[i]);
                itemWeapon.GetComponent<WeaponMovement>().ActivateWeapon(unitController);                
            }
        }

        float GetAngleY(GameObject weapon)
        {
            float angleZ = 0;

            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            if(x == 0 )
            {
                if(y == 0) angleZ = unitController.unitSprite.flipX == true ? -180 : 0;

                if(y == 0 && unitController.unitSprite.flipX == true) weapon.GetComponent<SpriteRenderer>().flipY = true;

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


    private void BottleAction(UnitController unitController)
    {
        StartCoroutine(CreateBottle());

        IEnumerator CreateBottle()
        {
            for(int i = 0; i < unitController.level; i++)
            {
                GameObject itemWeapon = CreateWeapon(unitController);
                itemWeapon.transform.eulerAngles = new Vector3(0, 0, 0);
                itemWeapon.GetComponent<WeaponMovement>().ActivateWeapon(unitController);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
