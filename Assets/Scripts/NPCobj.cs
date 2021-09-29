using System;
using UnityEngine;

[Serializable]
public struct NPCData {
	[HideInInspector]
	public bool IsIncheck;
	[HideInInspector]
	public SphereCollider Center;
	

	public string m_strName;
	/// <summary>
	/// true : Player, false : NPC
	/// </summary>
	public bool IsFirstDialogue;
	/// <summary>
	/// NPC대화
	/// </summary>
	public string[] NDialogue;
	/// <summary>
	/// Player대화
	/// </summary>
	public string[] PDialogue;
	/// <summary>
	/// 거래품목
	/// </summary>
	public bool Allitem;
	public int[] Consumables;
	public int[] Equipment;
}

public class NPCobj : MonoBehaviour {
	[HideInInspector]
	public bool IsIncheck;
	[HideInInspector]
	public SphereCollider Center;

	public NPCData npcData;

	public string strName;
	/// <summary>
	/// true : Player, false : NPC
	/// </summary>
	public bool IsFirstDialogue = false;
	/// <summary>
	/// NPC대화
	/// </summary>
	public string[] NDialogue = new string[0];
	/// <summary>
	/// Player대화
	/// </summary>
	public string[] PDialogue = new string[0];
	/// <summary>
	/// 거래품목
	/// </summary>
	public bool Allitem;
	public int[] Consumables;
	public int[] Equipment;
	/// <summary>
	/// 퀘스트
	/// </summary>
/*
	public int Quest;
	public string[] QuestDialogue = new string[0];
//*/


	void Start() {
		try {
			NPCManager.Instance.npcObj.Add(this);
		} catch {

		}
	}

	private void OnTriggerEnter(Collider other) {
		NPCInCheck(true);
	}

	private void OnTriggerExit(Collider other) {
		NPCInCheck(false);
		NPCManager.Instance.IsDeal = false;
		Inventory.selItem = null;
		EquipmentInventory.scItem = null;
		GameMaster.Instance.OptionButton.raycastTarget = true;
		NPCManager.Instance.NPCchoiceActive(false);
	}

	void NPCInCheck(bool _is) {
		IsIncheck = _is;
		NPCManager.npcsInCheck = _is;
	}
}
