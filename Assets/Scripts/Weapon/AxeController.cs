using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : CloseWeaponController
{
    // 활성화 여부
    public static bool isActivate = false;
    [SerializeField]
    private float weaponDamage = 1;


    protected void Update() {
        if (isActivate)
        {
            TryAttack();
        }
        //EquipItem();
    }


    protected override IEnumerator HitCoroutine()
    {
        while (isSwing)
        {
            if(CheckObject())
            {
                if(hitInfo.transform.tag == "Tree")
                {
                    hitInfo.transform.GetComponent<ChoppableTree>().GetHit(currentCloseWeapon.damage);
                }
                isSwing = false;
            }
            yield return null;
        }
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;

    }
}
