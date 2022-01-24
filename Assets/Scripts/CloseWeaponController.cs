using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloseWeaponController : MonoBehaviour//미완성함수가 있으므로 abstract클래스
{
    [SerializeField]
    protected CloseWeapon currentCloseWeapon; //closeWeapon형 타입의 무언가

    //공격중인가
    protected bool isAttack = false;
    protected bool isSwing = false;

    protected RaycastHit hitInfo;
    [SerializeField]
    protected LayerMask layerMask;


    //추상 클래스는 컴포넌트로 객체에 넣을 수 없기 때문에update함수가 작동하지 않는다.
    

    protected void TryAttack()
    {
        if(!Inventory.inventoryActivated)
        {
            if (Input.GetButton("Fire1"))
            {
                if (!isAttack)
                {
                    StartCoroutine("AttackCoroutine");
                }
            }
        }
    }

    protected IEnumerator AttackCoroutine()
    {
        isAttack = true;
        currentCloseWeapon.anim.SetTrigger("Attack");
        yield return new WaitForSeconds(currentCloseWeapon.attackDelayA);
        isSwing = true;
        StartCoroutine("HitCoroutine");
        yield return new WaitForSeconds(currentCloseWeapon.attackDelayB);
        isSwing = false;
        yield return new WaitForSecondsRealtime(currentCloseWeapon.attackDelay - currentCloseWeapon.attackDelayA - currentCloseWeapon.attackDelayB);


        isAttack = false;

    }

    protected abstract IEnumerator HitCoroutine(); //추상적(미완성 함수, 자식에서 완성시켜야함)

    protected bool CheckObject()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range, layerMask))
        {
            return true;
        }

        return false;
    }

    //완성 함수 이지만 추가 편집이 가능한 virtual함수
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        if (WeaponManager.curretnWeapon != null)
        {
            WeaponManager.curretnWeapon.gameObject.SetActive(false);
        }

        currentCloseWeapon = _closeWeapon;
        WeaponManager.curretnWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;

        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);
    }
}
