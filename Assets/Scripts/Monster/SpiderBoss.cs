using System.Collections;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Monster))]//현재 스크립트를 생성시 따오는 스크립트
public class SpiderBoss : MonoBehaviour {
	public Monster m_Monster;

	public Animator m_Ani;
	FiniteStateMachine m_FSM;

	public CapsuleCollider m_AttackCol;
	public CapsuleCollider m_BodyCol;
	public SphereCollider m_SkillOneCol;

	//스킬 구현 변수
	/// <summary>
	/// HP 숫치 확인
	/// </summary>
	float m_HPcheck;
	int skOneNumCheck;
	int skTwoNumCheck;
	public Spiderweb Spiderweb;
	public MeshRenderer ShadowMS;

	/// <summary>
	/// 추가 거리
	/// </summary>
	public float AddAttackDistance;
	/// <summary>
	/// 플레이어와의 거리
	/// </summary>
	float m_fDistance;
	/// <summary>
	/// 공격 거리
	/// </summary>
	float m_fAttackDistance;
	/// <summary>
	/// 추가 인식 거리
	/// </summary>
	public float AddRecognition;
	/// <summary>
	/// 플레이어 인식 거리
	/// </summary>
	float m_fRecognition;

	bool IsJumpAttack = false;

	bool IsShdowMove = true;

	Vector3 m_Movedirection;
	Vector3 m_Forward;
	Vector3 m_Velocity;

	Vector3 m_Attackdirection;
	Vector3 m_Attackforward;

	Vector3 ShadowMSpos;

	public void SetInfo () {
		m_HPcheck = 2000f;
		m_Monster.Status.m_Status[StatusConstant.HP].Set(m_HPcheck);
		m_Monster.Status.m_Status[StatusConstant.RegenHP].Set(0.2f);
		m_Monster.Status.m_Status[StatusConstant.RegenHPTime].Set(0.1f);
		m_Monster.Status.m_Status[StatusConstant.MP].Set(100f);
		m_Monster.Status.m_Status[StatusConstant.RegenMP].Set(0.05f);
		m_Monster.Status.m_Status[StatusConstant.RegenMPTime].Set(0.01f);
		m_Monster.Status.m_Status[StatusConstant.ATK].Set(30f);
		m_Monster.Status.m_Status[StatusConstant.ATKSpeed].Set(2f);
		m_Monster.Status.m_Status[StatusConstant.DEF].Set(0f);
		m_Monster.Status.m_Status[StatusConstant.MoveSpeed].Set(1.8f);
	}

	private void Awake() {
		m_Monster = GetComponent<Monster>();
	}

	void Start() {
		SetInfo();

		StartCoroutine(m_Monster.RegenHP());
		StartCoroutine(m_Monster.RegenSP());

		m_fAttackDistance = AddAttackDistance + 0.25f + (transform.localScale.x * 1f);
		m_fRecognition = AddRecognition + 5.1f;

		m_FSM = new FiniteStateMachine();
		m_FSM.AddState(State.Idle, IdleStart, IdleUpdatae, null);
		m_FSM.AddState(State.Move, null, MoveUpdatae, MoveExit);
		m_FSM.AddState(State.Attack, AttackEnter, null, AttackExit);
		m_FSM.AddState(State.Jump, JumpEnter, null, null);
		m_FSM.AddState(State.Spiderweb, SpiderwebEnter, null, null);

		m_FSM.SetState(State.Idle);

		skOneNumCheck = 0;
		skTwoNumCheck = 0;
	}

	void EndSD() {
		Destroy(ShadowMS.gameObject);
	}

	void Update() {
		if (m_Monster.Status.m_Status[StatusConstant.HP].GetPrint() <= 0 && m_FSM.GetState() != State.Death) {
			m_FSM.SetState(State.Death);
			m_FSM.SetEnd(false);
			m_Ani.SetTrigger("Death");
			m_BodyCol.enabled = false;
			m_AttackCol.enabled = false;
			m_Monster.EndAct += EndSD;
			m_Monster.Dead(m_Ani.GetCurrentAnimatorStateInfo(0).length + 0.3f);
			return;
		}

		if (ShadowMS != null && IsShdowMove) {
			ShadowMSpos = transform.position;
			ShadowMSpos.y = 0;
			ShadowMS.transform.position = ShadowMSpos;
		}

		if (m_FSM.GetEnd() && PlayerController.Instance.m_Stats.m_Status[StatusConstant.HP].GetPrint() <= 0) {
			m_FSM.SetEnd(false);
			m_Ani.Play("Idle", -1, 0);
			return;
		}

		m_fDistance = Vector3.Distance(Monster.m_Player.position, transform.position);
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.layer == CLayer.Player) {
			m_AttackCol.enabled = false;
			float _damage = m_Monster.Status.m_Status[StatusConstant.ATK].GetPrint();
			if (IsJumpAttack) {
				_damage = m_Monster.Status.m_Status[StatusConstant.ATK].GetPrint() * 3f;
			}
			PlayerInformation.m_PlayerStats.ReceiveDamage(PlayerInformation.m_PlayerController.transform, _damage, true);
		}
		if (col.gameObject.layer == CLayer.PlayerObj) {
			m_fRecognition = 100f;
		}
	}

	//----------------------------------------------------------------------------------
	void IdleStart() {
		if (m_Monster.Status.m_Status[StatusConstant.HP].GetPrint() < 1401f) {
			m_HPcheck = m_Monster.Status.m_Status[StatusConstant.HP].GetPrint();
		}
	}
	void IdleUpdatae() {
		if (m_HPcheck < 1401f) {
			if (5 <= skOneNumCheck) {
				m_FSM.SetState(State.Jump);
				m_Ani.SetTrigger("Jump");
				return;
			}
		}
		
		if (m_fDistance <= m_fAttackDistance) {
			m_FSM.SetState(State.Attack);
			m_Ani.SetTrigger("Attack");
			skOneNumCheck++;
		} else if (m_fDistance <= m_fRecognition) {
			m_FSM.SetState(State.Move);
			m_Ani.SetTrigger("Move");
		}
	}
	//-----------------------------------------------
	void MoveUpdatae() {
		m_Movedirection = Monster.m_Player.position - transform.position;

		m_Forward.x = m_Movedirection.x;
		m_Forward.y = 0f;
		m_Forward.z = m_Movedirection.z;
		transform.forward = m_Forward.normalized;

		m_Velocity.x = transform.forward.x * m_Monster.Status.m_Status[StatusConstant.MoveSpeed].GetPrint();
		m_Velocity.y = m_Monster.m_Rigid.velocity.y;
		m_Velocity.z = transform.forward.z * m_Monster.Status.m_Status[StatusConstant.MoveSpeed].GetPrint();
		m_Monster.m_Rigid.velocity = m_Velocity;

		if (m_fDistance <= m_fAttackDistance || PlayerController.Instance.m_Stats.m_Status[StatusConstant.HP].GetPrint() <= 0) {
			m_FSM.SetState(State.Idle);
			m_Ani.SetTrigger("Move");
		}
	}
	void MoveExit() {
		m_Monster.m_Rigid.velocity = Vector3.zero;
	}
	//-----------------------------------------------
	void AttackEnter() {
		if (m_FSM.GetState() != State.Death) {
			m_Attackdirection = Monster.m_Player.position - transform.position;

			m_Attackforward.x = m_Attackdirection.x;
			m_Attackforward.y = 0;
			m_Attackforward.z = m_Attackdirection.z;
			transform.forward = m_Attackforward.normalized;

			Invoke("AttackOne", 0.35f);
		}
	}
	void AttackOne() {
		if (m_FSM.GetState() != State.Death) {
			m_AttackCol.enabled = true;
			Invoke("AttackTwo", 0.3f);
		}
	}
	void AttackTwo() {
		m_FSM.SetState(State.Idle);
	}
	void AttackExit() {
		m_AttackCol.enabled = false;
	}
	//-----------------------------------------------
	void JumpEnter() {
		StartCoroutine(JumpSkill());
	}
	IEnumerator JumpSkill() {
		Color _color = ShadowMS.material.color;
		yield return WFS.sd5;
		Vector3 _v = transform.position;
		
		do {
			try {
				_v.y += 0.5f;
				_color.a -= 0.1f;
				if (_color.a <= 0f) {
					_color.a = 0f;
					IsShdowMove = false;
				}
				ShadowMS.material.color = _color;
				transform.position = _v;
			} catch (IOException) {

			}
			yield return WFS.sdd16;
		} while (_v.y <= 12);

		yield return WFS.s1;

		bool _isPos = true;

		do {
			try {
				Vector3 _ms = Monster.m_Player.position;
				_ms.y = 0;

				_color.a += 0.1f;

				if (1f <= _color.a) {
					if (_isPos) {
						_isPos = false;
						_v = Monster.m_Player.position;
						_v.y = 45f;
						Vector3 _direction = Monster.m_Player.position - transform.position;
						transform.forward = new Vector3(_direction.x, 0f, _direction.z).normalized;
					}

					_color.a = 1f;
					_v.y -= 0.9f;
				} else {
					ShadowMS.transform.position = _ms;
				}

				if (_v.y <= 0f) {
					_v.y = 0f;
					m_Ani.SetTrigger("Jump");
					IsShdowMove = true;
				}

				ShadowMS.material.color = _color;
				transform.position = _v;
			} catch (IOException) {

			}
			yield return WFS.sdd16;
		} while (0 < _v.y);

		IsJumpAttack = true;
		m_SkillOneCol.enabled = true;
		yield return WFS.sdd16;
		IsJumpAttack = false;
		m_SkillOneCol.enabled = false;
		yield return WFS.sd5;
		m_Ani.SetTrigger("Jump");
		IdleStart();
		if (m_HPcheck < 1001f) {
			m_FSM.SetState(State.Spiderweb);
		} else {
			m_FSM.SetState(State.Idle);
		}

		skOneNumCheck = 0;
		skTwoNumCheck = 3;
	}
	//-----------------------------------------------
	void SpiderwebEnter() {
		StartCoroutine(SpiderwebSkill());
	}
	IEnumerator SpiderwebSkill() {
		float _fDis = 0;
		float _fUp = 0;

		Vector3 _forward;
		Vector3 _direction;
		Spiderweb _spiderweb;

		do {
			try {
				_forward = _direction = Monster.m_Player.position - transform.position;
				_forward.y = 0f;
				_forward = _forward.normalized;
				if (_forward != Vector3.zero)
					transform.forward = _forward;
				m_Ani.Play("Spiderweb", -1, 0);
			} catch (IOException) {

			}
			yield return WFS.sdd5;
			yield return WFS.sdd3;
			try {
				_fDis = Vector3.Distance(Monster.m_Player.position, transform.position);
				_fUp = 1f;
				if (_fDis <= 1f) {
					_fUp = 1f * _fDis;
				}
				_spiderweb = Instantiate(Spiderweb, new Vector3(transform.position.x, 0.5f, transform.position.z), transform.rotation);
				_spiderweb.Shoot(_spiderweb.transform, _spiderweb.transform.position, PlayerInformation.m_PlayerController.transform.position, 50f, _fUp, m_Monster.Status.m_Status[StatusConstant.ATK].GetPrint());
			} catch (IOException) {

			}
			yield return WFS.sd5;
			yield return WFS.sd1;
			skTwoNumCheck--;
			yield return WFS.sd1;
		} while (0 < skTwoNumCheck);

		m_Monster.Status.m_Status[StatusConstant.MoveSpeed].GetBack();
		SkillManager.Instance.Fast(m_Monster.Status, 100f, 30);

		skTwoNumCheck = 0;

		m_Ani.SetTrigger("Spiderweb");
		m_FSM.SetState(State.Idle);
	}
	//----------------------------------------------------------------------------------
}