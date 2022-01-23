using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    //체력
    [SerializeField]
    private int hp;
    private int currentHp;

    //스테미나
    [SerializeField]
    private int sp;
    private int currentSp;

    //스테미나 회복력
    [SerializeField]
    private int SpRecoverySpeed;

    //스테미나 재 회복 딜레이
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    //스테미나 감소 여부
    private bool spUsed;

    //방어력
    [SerializeField]
    private int dp;
    private int currentDp;

    //배고픔
    [SerializeField]
    private int hungry;
    private int currentHungry;

    //배고픔이 줄어드는 속도
    [SerializeField]
    private int hungryDecreaseTime;
    private int currentHungryDecreaseTime;

    //목마름
    [SerializeField]
    private int thirsty;
    private int currentThirsty;

    //목마름이 줄어드는 속도
    [SerializeField]
    private int thirstyDecreaseTime;
    private int currentThirstyDecreaseTime;

    //만족도
    [SerializeField]
    private int satisfy;
    private int currentSatisfy;

    //필요한 이미지
    [SerializeField]
    private Image[] image_Guages;

    private const int HP = 0, DP = 1, SP = 2, HUNGRY = 3, THIRSTY = 4, SATISFY = 5;

    private void Start()
    {
        currentHp = hp;
        currentDp = dp;
        currentSp = sp;
        currentHungry = hungry;
        currentThirsty = thirsty;
        currentSatisfy = satisfy;
    }

    private void Update()
    {
        Hungry();
        Thirsty();
        SPRechargeTime();
        SPRecover();
        Guage_Update();
    }



    private void SPRechargeTime()
    {
        if (spUsed)
        {
            if (currentSpRechargeTime < spRechargeTime)
            {
                currentSpRechargeTime++;
            }
            else
                spUsed = false;
        }
        
    }

    private void SPRecover()
    {
        if(!spUsed && currentSp < sp)
        {
            currentSp += SpRecoverySpeed;
        }
    }

    private void Hungry()
    {
        if(currentHungry > 0)
        {
            if(currentHungryDecreaseTime <= hungryDecreaseTime)
            {
                currentHungryDecreaseTime++;
            }
            else
            {
                currentHungry--;
                currentHungryDecreaseTime = 0;
            }
        }
        else
        {
            Debug.Log("배고픔 수치가 0이 되었습니다.");
        }
    }

    private void Thirsty()
    {
        if (currentThirsty > 0)
        {
            if (currentThirstyDecreaseTime <= thirstyDecreaseTime)
            {
                currentThirstyDecreaseTime++;
            }
            else
            {
                currentThirsty--;
                currentThirstyDecreaseTime = 0;
            }
        }
        else
        {
            Debug.Log("목마름 수치가 0이 되었습니다.");
        }
    }

    private void Guage_Update()
    {
        image_Guages[HP].fillAmount = (float)currentHp / hp;
        image_Guages[SP].fillAmount = (float)currentSp / sp;
        image_Guages[DP].fillAmount = (float)currentDp / dp;

        image_Guages[THIRSTY].fillAmount = (float)currentThirsty / thirsty;
        image_Guages[HUNGRY].fillAmount = (float)currentHungry / hungry;
        image_Guages[SATISFY].fillAmount = (float)currentSatisfy / satisfy;
    }

    public void ChangeHP(int _count)
    {
        if(currentDp > 0)
        {
            ChangeDP(_count);
        }
        else
        {
            currentHp = Mathf.Clamp(currentHp + _count, 0, hp);
            if (currentHp == 0)
            {
                Debug.Log("플레이어가 사망하였습니다.");
            }
        }
    }

    public void ChangeDP(int _count)
    {
        currentDp = Mathf.Clamp(currentDp + _count, 0, dp);
        if (currentDp == 0)
        {
            Debug.Log("플레이어의 방어력이 0이 되었습니다.");
        }
    }

    public void ChangeSP(int _count)
    {
        currentSp = Mathf.Clamp(currentSp + _count, 0, sp);
    }

    public void ChangeHungry(int _count)
    {
        currentHungry = Mathf.Clamp(currentHungry + _count, 0, hungry);
    }

    public void ChangeThirsty(int _count)
    {
        currentThirsty = Mathf.Clamp(currentThirsty + _count, 0, thirsty);
    }

    public void DecreaseStamina(int _count)
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if(currentSp - _count > 0)
        {
            currentSp -= _count;
        }
        else
        {
            currentSp = 0;
        }
    }

    public int GetCurrentSP()
    {
        return currentSp;
    }
}
