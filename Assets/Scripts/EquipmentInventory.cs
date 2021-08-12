using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentInventory : MonoBehaviour, IPointerClickHandler {
	public static Item scItem;

	public static RectTransform EquipmentDataBG;
	public static Text EquipmentName;
	public static Text EquipmentDataText;
	public static Text EquipmentDataTextDum;

	public static Image[] Images;

	public static int Index;

	public static Transform cThis;

	public static void GetItemData(Item _item) {
		EquipmentName.text = _item.GetName();
		EquipmentDataText.text = null;

		ItemManager.Str.Clear();

		if(_item.GetCodeType() == CodeType.OnePortion || _item.GetCodeType() == CodeType.TwoPortion) {
			ItemManager.Str.AppendFormat("{0}개\n", _item.GetAmount());
		}

		for(int i = 0; i < StatusConstant.MaxStatus; i++) {
			if(_item.GetStaus(i) != 0) {
				ItemManager.Str.AppendFormat("{0} : {1}\n", StatusConstant.Name[i], _item.GetStaus(i));
			}
		}
		ItemManager.Str.AppendFormat("가격 : {0}", _item.GetGold());

		EquipmentDataTextDum.text = EquipmentDataText.text = ItemManager.Str.ToString();
	}

	public void OnPointerClick(PointerEventData eventData) {
		Index = eventData.pointerPressRaycast.gameObject.transform.GetSiblingIndex();

		if (Inventory.ItemDataBG.gameObject.activeSelf) {
			Inventory.ItemDataBG.gameObject.SetActive(false);
			Inventory.selItem = null;
			scItem = null;
		}

		if(eventData.pointerPressRaycast.gameObject != gameObject && ItemManager.cEquipment[Index] != null) {
			if (ItemManager.cEquipment[Index] == scItem && scItem != null) {
				scItem = null;
				EquipmentDataBG.gameObject.SetActive(false);
			} else {
				if (NPCInventory.Instance != null) {
					NPCInventory.Instance.m_ItemDataBG.gameObject.SetActive(false);
				}
				scItem = ItemManager.cEquipment[Index];
				EquipmentDataBG.gameObject.SetActive(true);
				GetItemData(ItemManager.cEquipment[Index]);
				EquipmentDataBG.anchoredPosition = new Vector2(0, 0);
			}
		} else {
			Inventory.selItem = null;
			scItem = null;
			EquipmentDataBG.gameObject.SetActive(false);
		}
	}

	void Awake() {
		cThis = transform;
		Images = new Image[12];
		Index = -1;
	}
}