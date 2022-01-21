﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public static bool isActivate = true;

    [SerializeField]
    private Gun currentGun; //현제 장착된 총

    private float currentFireRate;//연사속도

    private RaycastHit hitInfo;//hit한 raycast정보

    //상태 변수
    private bool isReload = false;
    [HideInInspector]
    public bool isFineSightMode = false;

    private Vector3 originPos; //정조준 이후 본래 위치로 이동하기 위한 벡터값

    //효과음 재생
    private AudioSource audioSource;

    //필요한 컴포넌트
    [SerializeField]
    private Camera theCam; //카메라
    private CrossHair theCrossHair;
    [SerializeField]
    GameObject hitEffectPrefab; //피격 이펙트

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        originPos = Vector3.zero;
        theCrossHair = FindObjectOfType<CrossHair>();
        WeaponManager.curretnWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.GetComponent<Animator>();
    }

    void Update()
    {
        if(isActivate)
        {
            GunFireRateCalc();
            TryFire();
            TryReload();
            TryFineSightMode();
        }
    }


    private void GunFireRateCalc() //연사속도 계산
    {
        if(currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime; //60프레임이면 1/60
        }

    }

    private void TryFire() //발사 시도
    {
        if(Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            Fire();
        }
    }

    private void Fire() //발사 전 계산
    {
        if (!isReload)
        { 
            if (currentGun.currentBulletCount > 0)
            {
                Shoot();
            }
            else
            {
                CancelFineSight();
                StartCoroutine("ReloadCoroutine");
            }
        }
    }

    private void Shoot() //발사 후 계산
    {
        theCrossHair.FireAnimation();
        currentGun.currentBulletCount--;
        currentFireRate = currentGun.fireRate; //연사 속도 재계산
        PlaySE(currentGun.fire_sound);
        currentGun.muzzleFlash.Play();
        Hit();
        StopAllCoroutines();
        StartCoroutine("RetroActionCoroutine"); //총기반동 코루틴 실행
    }

    private void Hit()
    {
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward +
                new Vector3(Random.Range(-theCrossHair.GetAccuracy() - currentGun.accuracy, theCrossHair.GetAccuracy() + currentGun.accuracy),
                            Random.Range(-theCrossHair.GetAccuracy() - currentGun.accuracy, theCrossHair.GetAccuracy() + currentGun.accuracy), 0), 
                out hitInfo, currentGun.range))
        {
            GameObject clone = Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 1.5f);

        }
    }

    private void TryReload() //재장전 시도
    {
        if(Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount )
        {
            CancelFineSight();
            StartCoroutine("ReloadCoroutine");
        }
    }

    IEnumerator ReloadCoroutine() //재장전 코루틴
    {
        if(currentGun.carryBulletCount > 0)
        {
            isReload = true;
            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;


            yield return new WaitForSeconds(currentGun.reloadTime);

            if(currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }

            isReload = false;
        }
        else
        {
            Debug.Log("No Bullet");
        }
    }

    public void CancelReload()
    {
        if(isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
    }

    private void TryFineSightMode() //정조준 시도
    {
        if(Input.GetButtonDown("Fire2") && !isReload)
        {
            FineSight();
        }
    }

    public void CancelFineSight() //정조준 취소
    {
        if (isFineSightMode)
            FineSight();
    }

    private void FineSight() //정조준 로직 시작
    {
        isFineSightMode = !isFineSightMode;
        theCrossHair.FineSightAnimation(isFineSightMode);
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);

        if(isFineSightMode)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());
        }

    }

    IEnumerator FineSightActivateCoroutine() //정조준 활성화
    {
        while(currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            
            yield return null;
        }
    }

    IEnumerator FineSightDeactivateCoroutine()//정조준 비활성화
    {
        while (currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

    IEnumerator RetroActionCoroutine() //반동 코루틴
    {
        Vector3 recoilBack = new Vector3(currentGun.retroActionForce, originPos.y, originPos.z);
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);
        //이 2개의 Vector3변수는 미리 선언한 뒤에 값을 매기는 것이 더 좋음. (메모리 단편화 방지)

        if(!isFineSightMode)
        {
            currentGun.transform.localPosition = originPos; //반동이 이미 있는 와중에 발사가 되면 반동이 없는 것 처럼 느껴지기 때문에 원래대로 되돌림

            //반동 시작
            while(currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            //원위치
            while(currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            //반동 시작
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            //원위치
            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }

    private void PlaySE(AudioClip _clip) //사운드 재생
    {
        audioSource.clip = _clip;
        audioSource.Play();
    }

    public Gun GetGun()
    {
        return currentGun;
    }

    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }

    public void GunChange(Gun _gun)
    {
        if(WeaponManager.curretnWeapon != null)
        {
            WeaponManager.curretnWeapon.gameObject.SetActive(false);
        }

        currentGun = _gun;
        WeaponManager.curretnWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim;

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate = true;
    }

}
