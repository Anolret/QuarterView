using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	public static PlayerController Instance;
	public GameObject m_MainCameraObj;
	public Joystick m_Joystick;
	public Rigidbody m_Rigid;
	public GameObject m_Bowshield;
	public LineRenderer m_Line;
	public BoxCollider m_LineBox;

	public Arrow m_Arrow;

	public GameObject m_Charge;

	public GameObject m_Bip;
	//[HideInInspector]
	public Animation m_Ani;

	public Stat m_Stats;
	public int Gold = 100;

	public Transform m_CenterPos;

	//스킬 사용여부
	public bool m_bSkillCheck = false;

	public GameObject m_Shadow;

	//스킬 오브젝트
	public GameObject m_Pushing;

	//--스킬 오브젝트

	//플레이어 체력 및 마나 표시
	public Image m_HP;
	public Text m_HPText;
	public Image m_MP;
	public Text m_MPText;

	public bool Isforward = true; //방향 유지
	public bool IsPos = true; //이동만 가능

	public StringBuilder Str = new StringBuilder();

	Vector3 HPV3 = new Vector3(1, 1, 1);
	Vector3 MPV3 = new Vector3(1, 1, 1);

	void Awake() {
		Instance = this;
		PlayerSetting();
		m_Arrow = Resources.Load<GameObject>("Model/Arrow/ArrowShot").GetComponent<Arrow>();
		m_CenterPos = GameObject.Find("PlayerCenter").transform;
	}

	MeshRenderer m_Renderer;

	void Start() {
		StartCoroutine(RegenHP());
		StartCoroutine(RegenSP());

		AniSetting();

		Joystick.NSInputDirection = transform.forward;
		
	}
	
	void Update() {
		HPV3.x = m_Stats.Percentage(StatusConstant.HP);
		MPV3.x = m_Stats.Percentage(StatusConstant.MP);

		m_HP.transform.localScale = HPV3;
		m_MP.transform.localScale = MPV3;

		Str.Clear();
		Str.AppendFormat("{0} / {1}", (int)m_Stats.m_Status[StatusConstant.HP].GetPrint(), (int)m_Stats.m_Status[StatusConstant.HP].GetChange());
		m_HPText.text = Str.ToString();
		Str.Clear();
		Str.AppendFormat("{0} / {1}", (int)m_Stats.m_Status[StatusConstant.MP].GetPrint(), (int)m_Stats.m_Status[StatusConstant.MP].GetChange());
		m_MPText.text = Str.ToString();
	}

	void Dead()	{
		m_Line.enabled = false;
		PlayerInformation.IsPlayerDeath = true;
		PlayerInformation.m_bMoveCheck = false;
		m_Ani.Play("HitAway");
	}

	/// <summary>
	/// 활 공격
	/// </summary>
	/// <param name="_power">화살이 날라가는 힘</param>
	/// <param name="_damge">공격력</param>
	/// <param name="_effect">활살에 들어갈 효과</param>
	public void Attack(float _power, float _damge, int _effect) {
		Arrow _arrow = Arrow.Pooling();
		_arrow.transform.position = m_Bowshield.transform.position;
		_arrow.transform.rotation = transform.rotation;
		_arrow.Set(_power, _damge, _effect);
	}

	void Move() {
		if (0 < (int)m_Stats.m_Status[StatusConstant.HP].GetPrint()) {
			if (IsPos) {
				if (Isforward) {
					if (PlayerInformation.m_bMoveCheck) {
						transform.forward = Joystick.NSInputDirection.normalized;
						m_Rigid.velocity = new Vector3(m_Joystick.InputDirection.normalized.x * m_Stats.m_Status[StatusConstant.MoveSpeed].GetPrint(),
													   m_Rigid.velocity.y,
													   m_Joystick.InputDirection.normalized.z * m_Stats.m_Status[StatusConstant.MoveSpeed].GetPrint());
						m_CenterPos.position = new Vector3(m_Bip.transform.position.x, transform.position.y, m_Bip.transform.position.z - 0.04f);
					} else {
						transform.forward = Joystick.NSInputDirection.normalized;
						m_Rigid.velocity = new Vector3();
					}
				} else {
					m_Rigid.velocity = new Vector3();
				}
			}
		}
	}

	void CameraMove() {
		m_MainCameraObj.transform.position = Vector3.Lerp(m_MainCameraObj.transform.position, transform.position, 0.05f);
	}

	IEnumerator RegenHP() {
		WaitForSeconds _hp = new WaitForSeconds(m_Stats.m_Status[StatusConstant.RegenHPTime].GetPrint());
		do {
			m_Stats.m_Status[StatusConstant.HP].Regen(m_Stats.m_Status[StatusConstant.RegenHP].GetPrint());
			yield return _hp;
		} while (0 < (int)m_Stats.m_Status[StatusConstant.HP].GetPrint());
		Dead();
	}
	
	IEnumerator RegenSP() {
		WaitForSeconds _mp = new WaitForSeconds(m_Stats.m_Status[StatusConstant.RegenMPTime].GetPrint());
		do {
			m_Stats.m_Status[StatusConstant.MP].Regen(m_Stats.m_Status[StatusConstant.RegenMP].GetPrint());
			yield return _mp;
		} while (0 < (int)m_Stats.m_Status[StatusConstant.HP].GetPrint());
	}

	public bool SkillUse(float _spPoint) {
		m_Stats.m_Status[StatusConstant.MP].Decrease(_spPoint);
		return true;
	}

	public Arrow ArrowInstantiate(Vector3 _pos, Quaternion _rot) {
		return Instantiate(m_Arrow, _pos, _rot);
	}

	void PlayerSetting() {
		PlayerInformation.m_PlayerController = this;
		m_Stats = PlayerInformation.m_PlayerStats;
	}

	void AniSetting() {
		m_Ani["Run"].speed = 0.7f * (m_Stats.m_Status[StatusConstant.MoveSpeed].GetPrint() / 2f);
	}

	void FixedUpdate() {
		Move();
		CameraMove();
		m_Shadow.transform.position = new Vector3(m_Bip.transform.position.x, 0, m_Bip.transform.position.z);
	}
}