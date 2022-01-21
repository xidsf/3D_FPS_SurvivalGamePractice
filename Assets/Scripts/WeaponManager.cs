using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(GunController))] 컴포넌트 안넣으면 오류가 뜸
public class WeaponManager : MonoBehaviour
{
    public static bool isChangeWeapon = false; //정적 변수

    [SerializeField]
    private string currentWeaponType; //현재 무기 타입
    public static Transform curretnWeapon; //현재 무기
    public static Animator currentWeaponAnim; //현재 무기의 애니메이션
    

    [SerializeField]
    private float changeWeaponDelayTime; //무기 교체 딜레이 타임
    [SerializeField]
    private float changeWeaponEndDelayTime;

    //무기 종류 관리
    [SerializeField]
    private Gun[] guns;
    [SerializeField]
    private CloseWeapon[] closeWeapons;

    //관리 차원에서 무기 접근이 용이하도록 만듦
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, CloseWeapon> closeWeaponDictionary = new Dictionary<string, CloseWeapon>();

    //필요한 컴포넌트
    [Header("Need Components")]
    [SerializeField]
    private GunController theGunController; //활성화/비활성화를 위한 컴포넌트들
    [SerializeField]
    private HandController theHandController;
    [SerializeField]
    private AxeController theAxeController;
    [SerializeField]
    private PickaxeController thePickaxeController;
    

    void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);
        }
        for (int i = 0; i < closeWeapons.Length; i++)
        {
            closeWeaponDictionary.Add(closeWeapons[i].closeWeaponName, closeWeapons[i]);
        }
    }

    void Update()
    {
        if (!isChangeWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))//무기 교체 실행 (맨손)
            {
                StartCoroutine(ChangeWeaponCoroutine("HAND", "Hand"));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))//서브머신건
            {
                StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMachineGun1"));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))//도끼
            {
                StartCoroutine(ChangeWeaponCoroutine("AXE", "Axe"));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))//도끼
            {
                StartCoroutine(ChangeWeaponCoroutine("PICKAXE", "Pickaxe"));
            }
        }
    }

    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");
        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction();
        WeaponChange(_type, _name);
        yield return new WaitForSeconds(changeWeaponEndDelayTime);
        isChangeWeapon = false;
        currentWeaponType = _type;

    }

    private void CancelPreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "GUN":
                theGunController.CancelFineSight();
                theGunController.CancelReload();
                GunController.isActivate = false;
                break;
            case "HAND":
                HandController.isActivate = false;
                break;
            case "AXE":
                AxeController.isActivate = false;
                break;
            case "PICKAXE":
                PickaxeController.isActivate = false;
                break;
        }
    }

    private void WeaponChange(string _type, string _name)
    {
        if(_type == "GUN")
        {
            theGunController.GunChange(gunDictionary[_name]);
        }
        else if(_type == "HAND")
        {
            theHandController.CloseWeaponChange(closeWeaponDictionary[_name]);
        }
        else if (_type == "AXE")
        {
            theAxeController.CloseWeaponChange(closeWeaponDictionary[_name]);
        }
        else if (_type == "PICKAXE")
        {
            theAxeController.CloseWeaponChange(closeWeaponDictionary[_name]);
        }
    }

}
