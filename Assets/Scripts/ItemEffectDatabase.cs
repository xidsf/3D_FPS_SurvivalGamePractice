using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemEffect
{
    public string itemName; //아이템의 이름
    [Tooltip("HP, SP, DP, HUNGRY, THIRSTY, SATISFY만 가능합니다.")]
    public string[] part; //부위
    public int[] num; //증가 수치
}

public class ItemEffectDatabase : MonoBehaviour
{
    [SerializeField]
    private ItemEffect[] itemEffects;

    //필요한 컴포넌트
    [Header("components")]
    [SerializeField]
    private StatusController thePlayerStatus;
    [SerializeField]
    private WeaponManager theWeaponManager;
    [SerializeField]
    private SlotToolTip theSlotToopTip;

    private const string HP = "HP", DP = "DP", SP = "SP", HUNGRY = "HUNGRY", THIRSTY = "THIRSTY", SATISFY = "SATISFY";

    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        theSlotToopTip.ShowToolTip(_item, _pos);
    }

    public void HideToolTip()
    {
        theSlotToopTip.HideToolTip();
    }

    public void UsedItem(Item _item)
    {
        if (_item.itemType == Item.ItemType.Equipment)
        {
            StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(_item.weaponType, _item.itemName));
            //장착
        }
        else if (_item.itemType == Item.ItemType.Used)
        {
            for (int i = 0; i < itemEffects.Length; i++)
            {
                if(itemEffects[i].itemName == _item.itemName)
                {
                    for (int j = 0; j < itemEffects[i].part.Length; j++)
                    {
                        switch(itemEffects[i].part[j])
                        {
                            case HP:
                                thePlayerStatus.ChangeHP(itemEffects[i].num[j]);
                                break;
                            case SP:
                                thePlayerStatus.ChangeSP(itemEffects[i].num[j]);
                                break;
                            case DP:
                                thePlayerStatus.ChangeDP(itemEffects[i].num[j]);
                                break;
                            case HUNGRY:
                                thePlayerStatus.ChangeHungry(itemEffects[i].num[j]);
                                break;
                            case THIRSTY:
                                thePlayerStatus.ChangeThirsty(itemEffects[i].num[j]);
                                break;
                            case SATISFY:
                                break;
                            default:
                                Debug.Log("잘못된 Status부위에 적용시켰습니다.");
                                break;
                        }
                        
                    }
                    Debug.Log(_item.itemName + "을 사용하였습니다.");
                    return;
                }

            }
            Debug.Log("itemEffectDatabase에 일치하는 아이템name이 없습니다.");
        }
    }
}
