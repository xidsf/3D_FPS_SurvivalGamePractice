using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; //아이템 획득 거리

    private bool pickupActivateed = false; //획득 가능한가?

    private RaycastHit hitInfo;//충돌체 정보 저장

    [SerializeField]
    private LayerMask layerMask;//아이템 레이어에만 반응하도록 하는 layermask

    [SerializeField]
    private TextMeshProUGUI actionText;
    [SerializeField]
    private Inventory theInventory;

    // Update is called once per frame
    void Update()
    {
        CheckItem();
        TryAction();
    }



    private void TryAction()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            CheckItem();
            PickupAction();
        }
    }

    private void PickupAction()
    {
        if(pickupActivateed)
        {
            if (hitInfo.transform != null)
            {
                theInventory.AquireItem(hitInfo.transform.GetComponent<ItemPickup>().item);
                Destroy(hitInfo.transform.gameObject);
                ItemInfoDisappear();
                Debug.Log(hitInfo.transform.GetComponent<ItemPickup>().item.itemName + "를 획득하였습니다.");
            }
        }
    }

    private void CheckItem()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, range, layerMask))
        {
            if(hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
            
        }
        else
        {
            ItemInfoDisappear();
        }
    }

    private void ItemInfoAppear()
    {
        pickupActivateed = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickup>().item.itemName + " 획득 " + "<color=yellow>" + "(E)" + "</color>";
    }

    private void ItemInfoDisappear()
    {
        pickupActivateed = false;
        actionText.gameObject.SetActive(false);
    }
}
