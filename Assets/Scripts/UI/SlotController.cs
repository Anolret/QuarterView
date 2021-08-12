using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotController : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler {
	public static List<SlotController> SkillButtons = new List<SlotController>();

	static bool IsDown = false;
	/// <summary>
	/// -기본 버튼 : 1, 3, 5
	/// -합산 버튼 : 4, 6, 8, 9
	/// </summary>
	public static int[] Skillslot = new int[10];

	public static int SlotNumber = 0; //사용할 슬롯 번호 저장

	/// <summary>
	/// 슬롯 번호
	/// </summary>
	public int m_MyNumber;
	/// <summary>
	/// 쿨타임 이미지
	/// </summary>
	public Image m_CoolTimeImg;
	/// <summary>
	/// 자신에게 들어왔었는지 판별
	/// </summary>
	public bool m_IsEnter = false;

	public Image m_UPImg;

	//스킬 슬롯 리셋
	void ReSkillslot() {
		for (int i = 0; i< Skillslot.Length; i++) {
			Skillslot[i] = -1;
		}
	}
	/// <summary>
	/// 버튼 숫자를 정렬하여 넣을 것
	/// </summary>
	void Awake() {
		m_UPImg = GetComponentsInChildren<Image>()[1];
		SkillButtons.Add(this);
		ReSkillslot();
	}

	void Start() {

	}

	public void OnPointerEnter(PointerEventData eventData) {
        if (IsDown && !m_IsEnter) {
			//데이터
			m_IsEnter = true;
			SlotNumber += m_MyNumber;
			//UI
			//m_CoolTimeImg.fillAmount = 0;
		}
	}

	public void OnPointerDown(PointerEventData eventData) {
        if (!PlayerInformation.IsPlayerDeath && !IsDown  && !m_IsEnter) {
			//데이터
			IsDown = true;
			m_IsEnter = true;
			SlotNumber += m_MyNumber;
		}
	}

	IEnumerator CoolTime() {
		int _num = -1;
		_num = SkillButtons.FindIndex(a => a.m_MyNumber == SlotNumber);

		if (_num != -1) {
			float _time = SkillManager.Instance.Skills[Skillslot[SkillButtons[_num].m_MyNumber]].m_CoolTime / 100f;
			WaitForSeconds _wfs = new WaitForSeconds(_time);

			SkillButtons[_num].m_UPImg.color = new Color(1f, 1f, 1f);
			SkillButtons[_num].m_UPImg.fillAmount = 0;

			do {
				SkillButtons[_num].m_UPImg.fillAmount += 0.01f;
				yield return _wfs;
			} while (SkillButtons[_num].m_UPImg.fillAmount < 1);

			SkillManager.Instance.Skills[Skillslot[SkillButtons[_num].m_MyNumber]].m_IsCoolTime = true;
		}
	}

	public void OnPointerUp(PointerEventData eventData) {
		//스킬 시전
		if (SlotNumber < 10 && SlotNumber != 0 && Skillslot[SlotNumber] != -1) {
			if (!PlayerInformation.IsPlayerDeath && Skill.m_IsSkillCheck &&
				PlayerInformation.m_PlayerStats.m_Status[StatusConstant.MP].GetPrint() > SkillManager.Instance.Skills[Skillslot[SlotNumber]].m_Cost &&
				SkillManager.Instance.Skills[Skillslot[SlotNumber]].m_IsCoolTime)
			   {

				PlayerInformation.m_PlayerStats.m_Status[StatusConstant.MP].Decrease(SkillManager.Instance.Skills[Skillslot[SlotNumber]].m_Cost);
				StartCoroutine(CoolTime());
				StartCoroutine(SkillManager.Instance.Skills[Skillslot[SlotNumber]].Cast());
			} else if (!SkillManager.Instance.Skills[Skillslot[SlotNumber]].m_IsCoolTime) {
				//TextControllor.cThis.SetText(PlayerInformation.m_PlayerController.transform.position, "재사용 대기시간입니다.", new Color(1, 1, 1), 20);
			} else {
				//TextControllor.cThis.SetText(PlayerInformation.m_PlayerController.transform.position, "마나가 부족합니다.", new Color(1, 1, 1), 20);
			}
		}

		//초기화
		SlotNumber = 0;
		IsDown = false;
		for (int i = 0 ; i < SkillButtons.Count ; i++) {
			SkillButtons[i].m_IsEnter = false;
		}
	}
}