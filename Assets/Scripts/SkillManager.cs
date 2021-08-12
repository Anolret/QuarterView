using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill {
	/// <summary>
	/// 스킬 이름
	/// </summary>
	public string m_Name = "";
	/// <summary>
	/// 스킬 설명
	/// </summary>
	public string m_Data = "";
	/// <summary>
	/// 스킬 사용 자원
	/// </summary>
	public float m_Cost;
	/// <summary>
	/// 재사용시간
	/// </summary>
	public float m_CoolTime;
	/// <summary>
	/// 스킬 쿨타임 확인
	/// </summary>
	public bool m_IsCoolTime;
	/// <summary>
	/// 스킬 이미지
	/// </summary>
	public Sprite m_Spr;
	/// <summary>
	/// 스킬을 사용하고 있는지 확인
	/// </summary>
	public static bool m_IsSkillCheck;
	
	public Skill() {
		m_Name = "";
		m_Data = "";
		m_Cost = 0;
		m_CoolTime = 0;
		m_IsCoolTime = false;
	}
	/// <summary>
	/// 스킬 셋팅
	/// </summary>
	/// <param name="_name">스킬 이름</param>
	/// <param name="_data">스킬 설명</param>
	/// <param name="_cost">스킬 사용 자원</param>
	/// <param name="_cooltime">재사용시간</param>
	public void SetData(string _name, string _data, float _cost, float _cooltime, Sprite _spr = null) {
		m_Name = _name;
		m_Data = _data;
		m_Cost = _cost;
		m_CoolTime = _cooltime;
		m_IsCoolTime = true;
		m_IsSkillCheck = true;
		m_Spr = _spr;
	}
	public virtual IEnumerator Cast() { yield return null; }

	public void MoveCheck() {
		if (!PlayerInformation.IsPlayerDeath) {
			if (!PlayerInformation.m_bDragCheck) {
				PlayerInformation.m_PlayerController.m_Ani.Play("AttackReady");
			} else {
				PlayerInformation.m_PlayerController.m_Ani.Play("Run");
			}
		}
	}
}

public class Backstab : Skill {
	public Backstab() {}
	public override IEnumerator Cast() {
		
		if (m_IsCoolTime) {
			PlayerInformation.m_bMoveCheck = false;
			PlayerInformation.m_PlayerController.m_Ani.Stop();
			m_IsCoolTime = false;
			AttackButton.IsAttack = true;
			PlayerInformation.m_PlayerController.IsPos = false;
			m_IsSkillCheck = false;

			Vector3 _startPos = PlayerInformation.m_PlayerController.transform.position;
			Vector3 _endPos;
			int _count = 0;
			int _checkCount = 10;
			bool _isEnd = true;
			float _moveSpeed = 5f;

			PlayerInformation.m_PlayerController.m_Ani["JumpStart"].speed = 6;
			PlayerInformation.m_PlayerController.m_Ani["JumpEnd"].speed = 1.5f;
			PlayerInformation.m_PlayerController.m_Ani.Play("JumpStart");
			do {
				PlayerInformation.m_PlayerController.m_Rigid.velocity = new Vector3(
								   -PlayerInformation.m_PlayerController.transform.forward.x * _moveSpeed,
								   PlayerInformation.m_PlayerController.transform.forward.y,
								   -PlayerInformation.m_PlayerController.transform.forward.z * _moveSpeed);
				yield return WFS.sdd5;
				_endPos = PlayerInformation.m_PlayerController.transform.position;
				_count++;
				if (_count > _checkCount - 6 && _isEnd) {
					_isEnd = false;
					PlayerInformation.m_PlayerController.m_Ani.Play("JumpEnd");
				}
			} while (_count < _checkCount);
			MoveCheck();

			AttackButton.IsAttack = false;
			PlayerInformation.m_PlayerController.IsPos = true;
			m_IsSkillCheck = true;
			PlayerInformation.m_bMoveCheck = true;
		}
	}
}
public class Multishot : Skill {
	public Multishot() {}
	public override IEnumerator Cast() {
		if (m_IsCoolTime) {
			PlayerInformation.m_bMoveCheck = false;
			PlayerInformation.m_PlayerController.m_Ani.Stop();
			m_IsCoolTime = false;
			AttackButton.IsAttack = true;
			m_IsSkillCheck = false;
			PlayerInformation.m_PlayerController.m_Ani.Play("ChargeEnd");
			
			float _rotY = -30;
			float _rotYSet = 10;
			Arrow _arrow;
			for (int i = 0; i < 7; i++) {
				_arrow = Arrow.Pooling();
				_arrow.transform.position = PlayerInformation.m_PlayerController.m_Bowshield.transform.position;
				_arrow.transform.rotation = PlayerInformation.m_PlayerController.transform.rotation;
				_arrow.transform.Rotate(new Vector3(0, _rotY, 0));
				_rotY += _rotYSet;
				_arrow.Set(10f, PlayerInformation.m_PlayerController.m_Stats.m_Status[StatusConstant.ATK].GetPrint() / 2.0f, 0);
			}
			yield return WFS.sd5;
			MoveCheck();
			AttackButton.IsAttack = false;
			m_IsSkillCheck = true;
			PlayerInformation.m_bMoveCheck = true;
		}
	}
}
public class Barrage : Skill {
	public Barrage() {}
	public override IEnumerator Cast() {
		if (m_IsCoolTime) {
			PlayerInformation.m_bMoveCheck = false;
			PlayerInformation.m_PlayerController.m_Ani.Stop();
			m_IsCoolTime = false;
			AttackButton.IsAttack = true;
			PlayerInformation.m_PlayerController.Isforward = false;
			m_IsSkillCheck = false;

			Arrow _arrow;
			int _count = 5;
			do {
				_arrow = Arrow.Pooling();
				_arrow.transform.position = PlayerInformation.m_PlayerController.m_Bowshield.transform.position;
				_arrow.transform.rotation = PlayerInformation.m_PlayerController.transform.rotation;
				_arrow.Set(10f, PlayerInformation.m_PlayerController.m_Stats.m_Status[StatusConstant.ATK].GetPrint() / 1.5f, 0);
				PlayerInformation.m_PlayerController.m_Ani.Play("ChargeEnd");
				yield return WFS.sd15;
				PlayerInformation.m_PlayerController.m_Ani.Stop("ChargeEnd");
				_count--;
			} while (0 < _count);
			MoveCheck();

			PlayerInformation.m_PlayerController.Isforward = true;
			AttackButton.IsAttack = false;
			m_IsSkillCheck = true;
			PlayerInformation.m_bMoveCheck = true;
		}
	}
}
public class Pushing : Skill {
	public Pushing() {
		PlayerInformation.m_PlayerController.m_Ani["Pushing"].speed = 1.2f;
	}

	public override IEnumerator Cast() {
		PlayerInformation.m_bMoveCheck = false;
		PlayerInformation.m_PlayerController.m_Ani.Stop();
		m_IsCoolTime = false;
		AttackButton.IsAttack = true;
		PlayerInformation.m_PlayerController.Isforward = false;
		m_IsSkillCheck = false;
		PlayerInformation.m_PlayerController.m_Ani.Play("Pushing");

		yield return WFS.sd2;
		PlayerInformation.m_PlayerController.m_Pushing.SetActive(true);
		//효과음, 임팩트 넣어주기
		yield return WFS.sd15;
		PlayerInformation.m_PlayerController.m_Pushing.SetActive(false);
		yield return WFS.sd2;
		yield return WFS.sd2;

		MoveCheck();
		PlayerInformation.m_PlayerController.Isforward = true;
		AttackButton.IsAttack = false;
		m_IsSkillCheck = true;
		PlayerInformation.m_bMoveCheck = true;
	}
}
public class ShowerArrow : Skill {
	public ShowerArrow() {}
	public override IEnumerator Cast() {
		if (m_IsCoolTime) {
			PlayerInformation.m_bMoveCheck = false;
			PlayerInformation.m_PlayerController.m_Ani.Stop();
			m_IsCoolTime = false;
			AttackButton.IsAttack = true;
			m_IsSkillCheck = false;
			PlayerInformation.m_PlayerController.m_Ani.Play("ChargeEnd");

			Vector3 _InsPos; //생성 위치
			_InsPos.x = PlayerInformation.m_PlayerController.m_Bowshield.transform.position.x + (PlayerInformation.m_PlayerController.transform.forward.x * 2);
			_InsPos.y = 8f;
			_InsPos.z = PlayerInformation.m_PlayerController.m_Bowshield.transform.position.z + (PlayerInformation.m_PlayerController.transform.forward.z * 2);

			yield return WFS.sd5;
			PlayerInformation.m_bMoveCheck = true;
			m_IsSkillCheck = true;
			AttackButton.IsAttack = false;
			MoveCheck();

			Arrow _arrow;

			float _size = 12f; //범위
			int _checkNum = 0; //생성 횟수

			Vector3 _boxSize = new Vector3(0.07f, 0.07f, 0.519f);
			Vector3 _rotate;

			do {
				for (int i = 0 ; i < 3 ; i++) {
					_arrow = Arrow.Pooling();
					_arrow.transform.position = _InsPos;
					_arrow.transform.rotation = Quaternion.LookRotation(Vector3.down);
					_arrow.m_BoxCollider.size = _boxSize;
					_rotate.x = Random.Range(-_size, _size);
					_rotate.y = Random.Range(-_size, _size);
					_rotate.z = Random.Range(-_size, _size);
					_arrow.transform.Rotate(_rotate);
					_arrow.Set(Random.Range(20f, 30f), PlayerInformation.m_PlayerController.m_Stats.m_Status[StatusConstant.ATK].GetPrint() / 2.0f, 0);
				}
				yield return WFS.sdd5;
				_checkNum++;
			} while (_checkNum < 30);
		}
	}
}

public class WinSkillSlot {
	/// <summary>
	/// 스킬 UI
	/// </summary>
	public Image SlotImg;
	/// <summary>
	/// 스킬이 들어가있는 슬롯 번호
	/// </summary>
	public int SlotNum;
	/// <summary>
	/// 스킬 번호
	/// </summary>
	public int SkillNum;

	public WinSkillSlot() {}

	public WinSkillSlot(Image _img, int _slotNum) {
		SlotImg = _img;
		SlotNum = _slotNum;
		SkillNum = -1;
	}
}

public class SkillManager : MonoBehaviour { //모든 스킬 관리
	public static SkillManager Instance;

	public RectTransform m_Scroll;

	public SkillSet AddSkillSetObj;

	/// <summary>
	/// 스킬 배열
	/// </summary>
	public Skill[] Skills = new Skill[30];

	public Dictionary<string, Skill> m_Skill = new Dictionary<string, Skill>();
	
	/// <summary>
	/// 선택한 슬롯 번호
	/// </summary>
	public int SelectSlotNum;
	/// <summary>
	/// 스킬 창의 선택한 스킬 번호
	/// </summary>
	public int SelectSkillNum;
	/// <summary>
	/// 스킬 번호
	/// </summary>
	public int SkillNum;

	/// <summary>
	/// 등록한 스킬 슬롯 UI
	/// </summary>
	public Image[] SetSkillSlot = new Image[10];
	/// <summary>
	/// 등록한 스킬
	/// </summary>
	public WinSkillSlot[] WinSkillSlot = new WinSkillSlot[10];

	public Sprite SkillSprTemporary;

	public void LoadSkillSetting(int num) {

	}

	public void SetSkill() {
		Skills[0] = new Backstab();
		Skills[0].SetData("후퇴", "뒤로 이동", 5f, 1.5f, Resources.Load<Sprite>("UI/Vault"));

		Skills[1] = new Multishot();
		Skills[1].SetData("다중 사격", "부채골로 화살을 쏜다", 10f, 3f, Resources.Load<Sprite>("UI/Multishot"));

		Skills[2] = new Barrage();
		Skills[2].SetData("연발 사격", "연속으로 화살을 쏜다", 15f, 3f, Resources.Load<Sprite>("UI/Rapid Fire"));

		Skills[3] = new Pushing();
		Skills[3].SetData("밀어차기", "적을 밀어 버린다.", 10f, 4.5f, Resources.Load<Sprite>("UI/Pushing"));

		Skills[4] = new ShowerArrow();
		Skills[4].SetData("화살비", "하늘에서 화살이 날라온다", 30f, 10f, Resources.Load<Sprite>("UI/ShowerArrow"));

		for (int i = 0 ; i < Skills.Length ; i++) {
			if (Skills[i] != null) {
				m_Skill.Add(Skills[i].m_Name, Skills[i]);
			}
		}
		
	}

	void Awake() {
		Instance = this;
		SelectSkillNum = SelectSlotNum = SkillNum = -1;

		WinSkillSlot _wss;
		for (int i = 0 ; i <  SetSkillSlot.Length ; i++) {
			if (SetSkillSlot[i]) {
				_wss = new WinSkillSlot(SetSkillSlot[i], i);
				WinSkillSlot[i] = _wss;
			}
		}
	}

	void Start() {
		SetSkill();

		bool _check = false;
		
		if (_check) {
			for (int i = 0; i < Skills.Length; i++) {
				if (Skills[i] != null) {
					Skills[i].m_CoolTime = 0.5f;
					Skills[i].m_Cost = 0;
				}
			}
		}

		for (int i = 0 ;i < Skills.Length ; i++) {
			if (Skills[i] != null) {
				SkillSet.cThis.Add(Instantiate(AddSkillSetObj, AddSkillSetObj.transform.position, AddSkillSetObj.transform.rotation));
				SkillSet.cThis[SkillSet.cThis.Count-1].transform.SetParent(m_Scroll.transform);
				SkillSet.cThis[SkillSet.cThis.Count-1].SetSkillData(
					i,
					Skills[i].m_Spr,
					Skills[i].m_Name,
					"MP소모:"+Skills[i].m_Cost+"\t\t재사용 시간:"+Skills[i].m_CoolTime+"초",
					Skills[i].m_Data);
			}
		}

		m_Scroll.sizeDelta = new Vector2(SkillSet.cThis[0].RT.rect.width, SkillSet.cThis[0].RT.rect.height * SkillSet.cThis.Count);
	}

	public void SCstart(IEnumerator _routine) {
		StartCoroutine(_routine);
	}

	//--------------------------스킬 효과--------------------------------------
	//공격
	public void Hit(Monster _monster, float _damage) {
		try {
			_monster.Status.ReceiveDamage(_monster.transform, _damage);
		} catch (System.Exception e) {
			Debug.Log(e);
		}
	}
	/// <summary>
	/// 이속 증가
	/// </summary>
	/// <param name="_cSta">대상</param>
	/// <param name="_receive">백분율</param>
	/// <param name="_num">10 = 1초</param>
	public void Fast(Stat _cSta, float _receive, int _num = 10) {
		StartCoroutine(IEFast(_cSta, _receive, _num));
	}
	IEnumerator IEFast(Stat _cSta, float _receive, int _num = 10) {
		try {
			_cSta.m_Status[StatusConstant.MoveSpeed].Increase(_receive, 1);
			do {
				yield return WFS.sd1;
				_num--;
			} while (0 < _num);
			_cSta.m_Status[StatusConstant.MoveSpeed].GetBack();
		} finally {}
	}
	/// <summary>
	/// 이속 감소
	/// </summary>
	/// <param name="_cSta">대상</param>
	/// <param name="_receive">백분율</param>
	/// <param name="_num">10 = 1초</param>
	public void Slow(Stat _cSta, float _receive, int _num = 10) {
		StartCoroutine(IESlow(_cSta, _receive, _num));
	}
	IEnumerator IESlow(Stat _cSta, float _receive, int _num = 10) {
		try {
			_cSta.m_Status[StatusConstant.MoveSpeed].Decrease(_receive, 1);
			do {
				yield return WFS.sd1;
				_num--;
			} while (0 < _num);
			yield return new WaitForSeconds(20);
			_cSta.m_Status[StatusConstant.MoveSpeed].GetBack();
		} finally {}
	}
	/// <summary>
	/// 밀기
	/// </summary>
	/// <param name="_this">공격자</param>
	/// <param name="_monster">대상</param>
	public void Knockback(Transform _this, Monster _monster, float _fower = 10) {
		StartCoroutine(IEKnockback(_this, _monster, _fower));
	}
	public IEnumerator IEKnockback(Transform _this, Monster _monster, float _fower) {
		try {
			Vector3 _mon = _monster.transform.position;
			Vector3 _fow = _this.forward;
			int _count = 0;
			do {
				_mon.x += _fow.x / 10f;
				_mon.z += _fow.z / 10f;
				_monster.transform.position = _mon;
				yield return WFS.sdd1;
				_count++;
			} while (_monster && _count < _fower);
		} finally {}
	}
//--------------------------스킬 효과(끝)-------------------------------------

	public void SkillSlotRaycastTarget(bool _bool) {
		for (int i = 0; i < WinSkillSlot.Length; i++) {
			if (WinSkillSlot[i] != null) {
				if (WinSkillSlot[i].SkillNum == -1) {
					WinSkillSlot[i].SlotImg.raycastTarget = _bool;
				}
			}
		}
	}
	/// <summary>
	/// 스킬 등록
	/// </summary>
	public void Enrollment() {
		SlotController.Skillslot[SelectSlotNum] = SkillNum;

		int _num = SlotController.SkillButtons.FindIndex(a => a.m_MyNumber == SelectSlotNum);
		SlotController.SkillButtons[_num].m_UPImg.sprite = Skills[SkillNum].m_Spr;
		SlotController.SkillButtons[_num].m_CoolTimeImg.sprite = Skills[SkillNum].m_Spr;

		SkillSet.cThis[SelectSkillNum].ButtonText.text = "스킬 해제";
		SkillSet.cThis[SelectSkillNum].ButtonText.color = new Color(1, 0, 0);

		SetSkillSlot[SelectSlotNum].sprite = Skills[SkillNum].m_Spr;

		SelectSkillNum = -1;
		SelectSlotNum = -1;
		SkillNum = -1;
	}
	public void Enrollment(int _slotNum, int _skillNum) {
		SelectSlot(_slotNum, _skillNum);

		SlotController.Skillslot[_slotNum] = _skillNum;

		int _num = SlotController.SkillButtons.FindIndex(a => a.m_MyNumber == _slotNum);
		SlotController.SkillButtons[_num].m_UPImg.sprite = Skills[_skillNum].m_Spr;
		SlotController.SkillButtons[_num].m_CoolTimeImg.sprite = Skills[_skillNum].m_Spr;

		_num = SkillSet.cThis.FindIndex(a => a.SkillNum ==  _skillNum);
		SkillSet.cThis[_num].ButtonText.text = "스킬 해제";
		SkillSet.cThis[_num].ButtonText.color = new Color(1, 0, 0);

		SetSkillSlot[_slotNum].sprite = Skills[_skillNum].m_Spr;
	}

	/// <summary>
	/// 스킬 창에 스킬 선택
	/// </summary>
	public void SelectSkill(SkillSet _skillSet) {
		int _num = SkillSet.cThis.FindIndex(a => a == _skillSet);

		SelectSkillNum = _num;
		SkillNum = SkillSet.cThis[_num].SkillNum;
		
		if (_skillSet.ButtonText.color.g == 0 && Skills[SkillNum].m_IsCoolTime) {//스킬 해제
			_skillSet.ButtonText.text = "스킬 선택";
			_skillSet.ButtonText.color = new Color(1, 1, 1);

			int _i = System.Array.FindIndex(WinSkillSlot, a => a != null && a.SkillNum == SkillSet.cThis[SelectSkillNum].SkillNum);

			WinSkillSlot[_i].SlotImg.sprite = SkillSprTemporary;

			SlotController.Skillslot[_i] = -1;

			int _sbn = SlotController.SkillButtons.FindIndex(a => a.m_MyNumber == _i);
			SlotController.SkillButtons[_sbn].m_UPImg.sprite = SkillSprTemporary;
			SlotController.SkillButtons[_sbn].m_CoolTimeImg.sprite = SkillSprTemporary;

			WinSkillSlot[_i].SkillNum = -1;

			SelectSkillNum = -1;
			SelectSlotNum = -1;
			SkillNum = -1;
		} else if (_skillSet.ButtonText.color.g != 0) {//스킬 선택
			if (_skillSet.ButtonText.color.r == 1) {//선택 중이라는 표시
				_skillSet.ButtonText.color = new Color(0, 1, 0);
				SkillSlotRaycastTarget(true);
			} else {//선택 취소
				_skillSet.ButtonText.color = new Color(1, 1, 1);
				SkillSlotRaycastTarget(false);
				SelectSkillNum = -1;
				SelectSlotNum = -1;
				SkillNum = -1;
			}
	
		}

	}
	/// <summary>
	/// 스킬 설정에 슬롯 선택
	/// </summary>
	public void SelectSlot(int _slotNum) {
		if (WinSkillSlot[_slotNum].SlotImg.raycastTarget) {
			SkillSlotRaycastTarget(false);
			SelectSlotNum = WinSkillSlot[_slotNum].SlotNum;
			WinSkillSlot[_slotNum].SkillNum = SkillNum;
			Enrollment();
		}
	}

	public void SelectSlot(int _slotNum, int _skillNum) {
		SkillNum = _skillNum;
		SkillSlotRaycastTarget(false);
		SelectSlotNum = WinSkillSlot[_slotNum].SlotNum;
		WinSkillSlot[_slotNum].SkillNum = SkillNum;
	}
}