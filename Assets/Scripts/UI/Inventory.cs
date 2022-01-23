﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false; //인벤토리가 활성화 되어 있는가

    //필요한 컴포넌트
    [SerializeField]
    private GameObject go_inventroyBase;
    [SerializeField]
    private GameObject go_slotsParent;

    private Slot[] slots;

    
    void Start()
    {
        slots = go_slotsParent.GetComponentsInChildren<Slot>();
    }

    void Update()
    {
        TryOpenInventory();
    }



    private void TryOpenInventory()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            inventoryActivated = !inventoryActivated;
            if(inventoryActivated)
            {
                OpenInventory();
            }
            else
            {
                CloseInventory();
            }
        }
    }
    
    private void OpenInventory()
    {
        go_inventroyBase.SetActive(true);
    }
    private void CloseInventory()
    {
        go_inventroyBase.SetActive(false);
    }

    public void AquireItem(Item _item, int _count = 1)
    {
        if(Item.ItemType.Equipment != _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null && slots[i].item.itemName == _item.itemName)
                {
                    slots[i].SetSlotCount(_count);
                    return;
                }
            }
        }
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }

}
