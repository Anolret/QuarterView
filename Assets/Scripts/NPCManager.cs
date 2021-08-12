using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NPCManager : MonoBehaviour {
	public static NPCManager Instance;
	public static bool npcsInCheck;

	/// <summary>
	/// NPC들
	/// </summary>
	public List<NPCobj> npcObj = new List<NPCobj>();
	public GridLayoutGroup NPCInventoryBG;
	/// <summary>
	/// 인벤토리 이미지
	/// </summary>
	public Image[] Images = new Image[60];
	/// <summary>
	/// 계속 생성해서 쓰는 스트링 방지
	/// </summary>
	/// Str.Clear();
	/// Str.Append("[");
	/// Str.Append(index);
	///	Str.AppendFormat("{0} : {1}\n", CCStatus.Name[i], _item.GetStaus(i)); // 0번째 인자, 1번째 인자
	///	Str.Remove(ItemControllor.Str.Length - 1, 1); // 특정 문자 삭제
	///	text = ItemControllor.Str.ToString();
	public StringBuilder Str = new StringBuilder();

	public GameObject m_npcInventory;
	
	/// <summary>
	/// NPC클릭 버튼
	/// </summary>
	public Image NPCButton;

	public GameObject NPCchoice;
	public GameObject DialogueBG;
	public Text DialogueName;
	public Text DialogueText;
	/// <summary>
	/// 대화가 끝났는지 확인
	/// </summary>
	public bool IsDialogueTextEnd = true;
	public bool IsSentenceCompletion = false;
	/// <summary>
	/// 다음 문장으로 넘길지 확인
	/// </summary>
	public bool IsNextSentence = false;

	/// <summary>
	/// 거래 중인지 판별
	/// </summary>
	public bool IsDeal = false;

	private void Awake() {
		Instance = this;
		m_npcInventory.SetActive(true);
		m_npcInventory.SetActive(false);
	}

	void Start() {
		
	}

	NPCobj InNPCobj() {
		return npcObj.Find(a => a.IsIncheck == true);
	}

	private float distance;
	private float diameter;
	private float angularSize;
	private float pixelSize;
	private Vector3 scrPos;

	private void Update() {
		if (npcsInCheck) {
			NPCButton.enabled = true;
			NPCobj _npcObj = InNPCobj();

			diameter = _npcObj.Center.bounds.extents.magnitude;
			distance = Vector3.Distance(_npcObj.Center.transform.position, Camera.main.transform.position);
			angularSize = (diameter / distance) * Mathf.Rad2Deg;
			pixelSize = ((angularSize * Screen.height) / Camera.main.fieldOfView);
			scrPos = Camera.main.WorldToScreenPoint(_npcObj.Center.transform.position);

			NPCButton.rectTransform.anchoredPosition = new Vector2(scrPos.x-pixelSize/2, scrPos.y - (pixelSize / 2));
			NPCButton.rectTransform.sizeDelta = new Vector2(pixelSize, pixelSize);

		} else {
			NPCButton.enabled = false;
		}
	}

	public void NPCchoiceActive() {
		NPCchoice.SetActive(!NPCchoice.activeSelf);
		GameMaster.Instance.OptionButton.raycastTarget = false;
		GameMaster.Instance.OptionButtons.SetActive(false);
	}
	public void NPCchoiceActive(bool _is) {
		NPCchoice.SetActive(_is);
	}
	public void NPCchoice_InventoryActive() {
		if(npcsInCheck)
			NPCchoice.SetActive(true);
		else
			NPCchoice.SetActive(false);
	}
	
	/// <summary>
	/// 대화
	/// </summary>
	public void NPCDialogue(NPCobj _npcObj) {
		IsDialogueTextEnd = false;
		NPCchoiceActive();
		StartCoroutine(DialogueTextArray(_npcObj));
	}

	/// <summary>
	/// 대화 텍스트 출력
	/// </summary>
	IEnumerator DialogueTextArray(NPCobj _npcObj) {
		char[] _char = new char[0];
		WaitWhile _ww = new WaitWhile(() => IsNextSentence == true);
		WaitForSeconds _wfs = new WaitForSeconds(0.07f);

		int _npc = 0;
		int _player = 0;

		string _str = "";
		
		bool _isDialogue = _npcObj.IsFirstDialogue;
		bool _isDialogueTextEnd = false;

		do {
			Str.Clear();
			DialogueText.text = "";
			_isDialogueTextEnd = false;

			if (_isDialogue && _player < _npcObj.PDialogue.Length) {
				DialogueName.text = "플레이어";
				_char = _npcObj.PDialogue[_player].ToCharArray();
				_str = _npcObj.PDialogue[_player];
				_player++;
				if (_npc < _npcObj.NDialogue.Length) {
					_isDialogue = false;
				}
			} else if (!_isDialogue && _npc < _npcObj.NDialogue.Length) {
				DialogueName.text = _npcObj.strName;
				_char = _npcObj.NDialogue[_npc].ToCharArray();
				_str = _npcObj.NDialogue[_npc];
				_npc++;
				if (_player < _npcObj.PDialogue.Length) {
					_isDialogue = true;
				}
			}

			for (int i = 0 ; i < _char.Length ; i++) {
				if (!IsSentenceCompletion) {
					Str.Append(_char[i]);
					DialogueText.text = Str.ToString();
					yield return _wfs;
				} else {
					DialogueText.text = _str;
					break;
				}
			}

			if (_npc < _npcObj.NDialogue.Length || _player < _npcObj.PDialogue.Length) {
				_isDialogueTextEnd = true;
			} else {
				_isDialogueTextEnd = false;
			}

			IsSentenceCompletion = true;

			IsNextSentence = true;
			yield return _ww;
			IsSentenceCompletion = false;
		} while (_isDialogueTextEnd);

		IsDialogueTextEnd = true;

		NPCchoice.SetActive(true);
		DialogueBG.SetActive(false);
	}

	public void NextDialogue() {
		if (IsSentenceCompletion)
			IsNextSentence = false;
		IsSentenceCompletion = true;
	}

	/// <summary>
	/// 거래
	/// </summary>
	public void NPCDeal(NPCobj _npcObj) {
		NPCchoiceActive();
		IsDeal = true;
		ItemManager.Instance.InventoryOpen();
		m_npcInventory.SetActive(true);
		NPCInventory.Instance.InventoryReset();

		if (_npcObj.Allitem) {
			for (int i = 0 ; i < ItemManager.m_Consumables.Count ; i++) {
				NPCInventory.Instance.AddItem(ItemManager.m_Consumables[i]);
			}

			for (int i = 0 ; i < ItemManager.m_Equipment.Count ; i++) {
				NPCInventory.Instance.AddItem(ItemManager.m_Equipment[i]);
			}
		} else {
			for (int i = 0 ; i < _npcObj.Consumables.Length ; i++) {
				NPCInventory.Instance.AddItem(ItemManager.m_Consumables[i]);
			}

			for (int i = 0 ; i < _npcObj.Equipment.Length ; i++) {
				NPCInventory.Instance.AddItem(ItemManager.m_Equipment[i]);
			}
		}
	}

	/// <summary>
	/// 퀘스트
	/// </summary>
	public void NPCQuest(NPCobj _npcObj) {
		NPCchoiceActive();


	}

	/// <summary>
	/// NPC 상호작용 선택
	/// </summary>
	/// <param name="_num"></param>
	public void NPCchoiceOpen(int _num) {
		switch (_num) {
			case 0:
				NPCDialogue(InNPCobj());
				break;
			case 1:
				NPCDeal(InNPCobj());
				break;
			case 2:
				NPCQuest(InNPCobj());
				break;
			case 3:
				NPCchoice.SetActive(false);
				IsDeal = false;
				GameMaster.Instance.OptionButton.raycastTarget = true;
				GameMaster.Instance.OptionButtons.SetActive(false);
				break;
		}
	}
}
