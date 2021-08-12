using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCInventory : MonoBehaviour, IPointerClickHandler {
	public static NPCInventory Instance;

	/// <summary>
	/// NPC 아이템 정보 출력 Obj
	/// </summary>
	public RectTransform m_ItemDataBG;
	public Text m_ItemName;
	public Text m_ItemDataText;
	public Text m_ItemDataTextDum;

	public Image[] Images = new Image[60];
	public Item[] cInventory = new Item[60];

	public int Index;

	private void Awake() {
		Instance = this;
	}

	public void InventoryReset() {
        for (int i = 0 ; i < cInventory.Length ; i++) {
			Images[i].sprite = ItemManager.spItemNull;
			cInventory[i] = null;
		}
	}

	public int AddItem(Item _item) {
		int _num = System.Array.FindIndex(cInventory, a => a == null);
		cInventory[_num] = _item;
		Images[_num].sprite = cInventory[_num].GetImg();
		return _num;
	}

	/// <summary>
	/// 구입
	/// </summary>
	public void Buy() {
		if (cInventory[Index].GetGold() < ItemManager.Instance.Gold) {
			int _num = ItemManager.AddItem(cInventory[Index]);
			if(_num != -1) {
				Inventory.ItemImages[_num].sprite = ItemManager.cInventory[_num].GetImg();
				ItemManager.Instance.Gold -= cInventory[Index].GetGold();
				Inventory.selItem = null;
				EquipmentInventory.scItem = null;
				m_ItemDataBG.gameObject.SetActive(false);
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		Index = eventData.pointerPressRaycast.gameObject.transform.GetSiblingIndex();
		if (cInventory[Index] != null) {
			m_ItemDataBG.gameObject.SetActive(true);
			Inventory.ItemDataBG.gameObject.SetActive(false);
			EquipmentInventory.EquipmentDataBG.gameObject.SetActive(false);
			GetItemData(cInventory[Index]);
		} else {
			m_ItemDataBG.gameObject.SetActive(false);
			Inventory.ItemDataBG.gameObject.SetActive(false);
			EquipmentInventory.EquipmentDataBG.gameObject.SetActive(false);
		}
	}

	public void GetItemData(Item _item) {
		m_ItemName.text = _item.GetName();
		m_ItemDataText.text = null;

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

		m_ItemDataTextDum.text = m_ItemDataText.text = ItemManager.Str.ToString();
	}
}