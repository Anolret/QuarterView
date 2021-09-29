using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IPointerClickHandler {
	public static Item selItem;

	/// <summary>
	/// 아이템정보 출력 이미지
	/// </summary>
	public static RectTransform ItemDataBG;
	public static Text ItemName;
	public static Text ItemDataText;
	public static Text ItemDataTextDum;

	public static Image[] ItemImages = new Image[30];

	public static int Index;

	public static Transform cThis;
	
	public static void GetItemData(Item _item) {
		ItemName.text = _item.GetName();
		ItemDataText.text = null;

		ItemManager.Str.Clear();

		if (_item.GetCodeType() == CodeType.OnePortion || _item.GetCodeType() == CodeType.TwoPortion) {
			ItemManager.Str.AppendFormat("{0}개\n", _item.GetAmount());
		}

		for (int i = 0 ; i < StatusConstant.MaxStatus ; i++) {
			if (_item.GetStaus(i) != 0) {
				ItemManager.Str.AppendFormat("{0} : {1}\n", StatusConstant.Name[i], _item.GetStaus(i));
			}
		}

		ItemManager.Str.AppendFormat("가격 : {0}", _item.GetGold());

		ItemDataTextDum.text = ItemDataText.text = ItemManager.Str.ToString();
	}

	// 인벤토리의 아이템 선택 또는 선택해제
	public void OnPointerClick(PointerEventData eventData) {
		Index = eventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex();
		if (eventData.pointerPressRaycast.gameObject != gameObject && ItemManager.cInventory[Index] != null) {
			if (ItemManager.cInventory[Index] == selItem && selItem != null) {
				selItem = null;
				ItemDataBG.gameObject.SetActive(false);
				EquipmentInventory.EquipmentDataBG.gameObject.SetActive(false);
			} else {
				if (NPCInventory.Instance != null) {
					NPCInventory.Instance.m_ItemDataBG.gameObject.SetActive(false);
				}
				ItemManager.Instance.m_SellButton.SetActive(false);
				EquipmentInventory.EquipmentDataBG.gameObject.SetActive(false);
				selItem = ItemManager.cInventory[Index];
				ItemDataBG.gameObject.SetActive(true);
				GetItemData(ItemManager.cInventory[Index]);
				int _enum = (int)ItemManager.cInventory[Index].GetCodeType();
				EquipmentInventory.Index = _enum;
				ItemDataBG.anchoredPosition = new Vector2(0, 0);
				if (NPCManager.Instance.IsDeal) {
					ItemManager.Instance.m_SellButton.SetActive(true);
				}
				if (ItemManager.cEquipment[_enum] != null) {
					EquipmentInventory.scItem = ItemManager.cEquipment[_enum];
					EquipmentInventory.GetItemData(ItemManager.cEquipment[_enum]);
					EquipmentInventory.EquipmentDataBG.gameObject.SetActive(true);
					EquipmentInventory.EquipmentDataBG.anchoredPosition = new Vector2(-(EquipmentInventory.EquipmentDataBG.rect.width / 2.0f), 0);
					ItemDataBG.anchoredPosition = new Vector2((ItemDataBG.rect.width / 2.0f), 0);
				}
			}
		} else {
			selItem = null;
			EquipmentInventory.scItem = null;
			ItemDataBG.gameObject.SetActive(false);
			EquipmentInventory.EquipmentDataBG.gameObject.SetActive(false);
			NPCInventory.Instance.m_ItemDataBG.gameObject.SetActive(false);
		}
	}
	
	void Awake() {
		cThis = transform;
		Index = -1;

		ItemManager.ItemImgReset(true);
	}
}
