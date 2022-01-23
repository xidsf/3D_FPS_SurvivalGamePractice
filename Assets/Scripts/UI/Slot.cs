using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item; //획득한 아이템
    public int itemCount; //아이템 갯수
    public Image itemImage; //인벤 안에서 나올 스프라이트

    //필요한 컴포넌트
    [SerializeField]
    private TextMeshProUGUI Text_Count; //아이템 갯수 표시 텍스트
    [SerializeField]
    private GameObject go_CountImage; //아이템 갯수 표시 텍스트 오브젝트
    private ItemEffectDatabase theItemEffectDatabase;

    private void Start()
    {
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
    }



    private void SetColor(float _alpha) //이미지의 투명도 조절
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    public void AddItem(Item _item, int _count = 1) //아이템 획득
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if(item.itemType != Item.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            Text_Count.text = itemCount.ToString();
        }
        else
        {
            Text_Count.text = "0";
            go_CountImage.SetActive(false);
        }
        SetColor(1);
    }

    public void SetSlotCount(int _count) //아이템 갯수 조정
    {
        itemCount += _count;
        Text_Count.text = itemCount.ToString();

        if(itemCount <= 0)
        {
            ClearSlot();
        }
    }

    private void ClearSlot() //슬롯 초기화
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);
        Text_Count.text = "0";
        go_CountImage.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if(item != null)
            {
                theItemEffectDatabase.UsedItem(item);
                if(item.itemType == Item.ItemType.Used)
                    SetSlotCount(-1);
                    //소모
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();
        }
    }

    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);
        if(_tempItem != null)
        {
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        }
        else
        {
            DragSlot.instance.dragSlot.ClearSlot();
        }

    }

    public void OnPointerEnter(PointerEventData eventData) //마우스가 슬롯에 들어갈 때 호출
    {
        if(item != null)
        {
            theItemEffectDatabase.ShowToolTip(item, transform.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)//마우스가 슬롯에 나갈 때 호출
    {
        theItemEffectDatabase.HideToolTip();
    }
}
