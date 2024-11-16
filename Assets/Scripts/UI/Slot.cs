using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using OpenCover.Framework.Model;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using UnityEditor.EventSystems;

public class Slot : MonoBehaviour , IPointerClickHandler , IBeginDragHandler , IDragHandler , IEndDragHandler , IDropHandler
{
    private Vector3 originPos;
    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템의 개수
    public Image itemImage; // 아이템의 이미지

    // 필요한 컴포넌트
    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;
    private WeaponManager theWeaponManger;

    private void Start() {
        originPos = transform.position;
        theWeaponManger = FindObjectOfType<WeaponManager>();
    }

    // 이미지 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }
    // 아이템 획득
    public void AddItem(Item _item , int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if(item.itemType != Item.ItemType.Equipment) // 인수로 전달 된 item의 Type가 Equipment가 아니라면
        {
            go_CountImage.SetActive(true); // CountImage 활성화
            text_Count.text = itemCount.ToString(); // text_Count.text에 itemCount를ToString해서 저장
        }
        else // item의 타입이 Equipment일 경우 
        {
            text_Count.text = "0"; // 0으로 변경
            go_CountImage.SetActive(false); // CountImage false
        }
        SetColor(1);
    }

    // 아이템 개수 조정
    public void SetSlotCount(int _count = 1)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if(itemCount <= 0)
            ClearSlot();
    }
    // 슬롯 초기화
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData) // 인터페이스 구현
    {
        if(eventData.button == PointerEventData.InputButton.Right) // 우클릭
        {
            if(item != null)
            {
                if(item.itemType == Item.ItemType.Equipment) // 우클릭했을 때 아이템이 장비 아이템일 경우
                {
                    if(theWeaponManger.currentEquipWeapon != item.itemName) 
                        StartCoroutine(theWeaponManger.ChangeWeaponCoroutine(item.weaponType, item.itemName));
                }
                else if(item.itemType == Item.ItemType.Used)
                {
                    Debug.Log(item.itemName + " 을 사용했습니다");
                    SetSlotCount(-1);
                }
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right && Input.GetKey(KeyCode.LeftShift))
        {
            if (item != null)
            {
                FindObjectOfType<ItemDropper>().DropItem(item);
                SetSlotCount(-1);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null)
        {
            DragSlot.instance.dragSlot = this; // dragSlot에 자기 자신을 넣어줌으로서 dragslot이 SLot가 됨
            DragSlot.instance.DragSetImage(itemImage);
            // eventData.position은 현재 마우스의 위치를 제공
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(item != null)
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
            ChangeSlot();
    }

    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;
        // OnDrop 실행시 현재 Drop된 슬롯에 AddItem 해줌
        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);
        ///Summary
        /// OnBeginDrag 실행시 dragSlot = this 로 초기화
        /// _tempItem != null 이면 this로 초기화 된 slot에 AddItem
        ///Summary
        if (_tempItem != null)
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        else
            DragSlot.instance.dragSlot.ClearSlot();
    }
}
