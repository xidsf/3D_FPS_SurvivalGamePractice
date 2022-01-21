using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    //필요한 컴포넌트
    [SerializeField]
    private GunController theGunController;
    private Gun currentGun;


    [SerializeField]
    private GameObject go_BulletUI; //HUD활성화 / 비활성화
    [SerializeField]
    private TextMeshProUGUI[] Text = new TextMeshProUGUI[3]; //textMeshPro사용

    void Update()
    {
        CheckBullet();
    }

    private void CheckBullet()
    {
        currentGun = theGunController.GetGun();
        Text[0].text = currentGun.currentBulletCount.ToString();
        Text[1].text = currentGun.reloadBulletCount.ToString();
        Text[2].text = currentGun.carryBulletCount.ToString();
    }

}
