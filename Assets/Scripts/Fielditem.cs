using UnityEngine;

public class Fielditem : MonoBehaviour {
	public Item m_Item;
	public SpriteRenderer m_Spr;
	public bool IsPlayerOutCheck = true;

	/// <summary>
	/// 아이템 설정
	/// </summary>
	/// <param name="_item">아이템</param>
	/// <param name="_isPlayerOutCheck">바로 먹을 수 있는 설정</param>
	public void Setting(Item _item, bool _isPlayerOutCheck = true) {
		m_Item = _item;
		IsPlayerOutCheck = _isPlayerOutCheck;
	}

	private void OnTriggerEnter(Collider other) {
		if (m_Item != null && IsPlayerOutCheck) {
			int _num = ItemManager.AddItem(m_Item);
			if (_num != -1) {
				Inventory.ItemImages[_num].sprite = ItemManager.cInventory[_num].GetImg();
				Destroy(gameObject);
			}
		}
	}

    private void OnTriggerExit(Collider other) {
		if (!IsPlayerOutCheck) {
			IsPlayerOutCheck = true;
		}
	}
}
