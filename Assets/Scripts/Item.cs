using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject //게임 오브젝트에 굳이 붙일 필요가 없다.
{
    public string itemName; //아이템 이미지
    [TextArea]
    public string itemDesc; //아이템 설명
    public ItemType itemType; //아이템의 유형
    public Sprite itemImage; //inventory에 나타날 이미지
    public GameObject itemPrefab; //아이템의 프리팹

    public string weaponType; //무기 유형

    public enum ItemType
    {
        Equipment, //장비
        Used, //소모품
        Ingredient, //재료
        ETC //기타
    }



}
