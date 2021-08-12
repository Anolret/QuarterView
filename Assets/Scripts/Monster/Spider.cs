using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Monster))]//현재 스크립트를 생성시 따오는 스크립트
public class Spider : MonoBehaviour {
	public Monster m_Monster;

	public Animator m_Ani;
	FiniteStateMachine m_FSM;

	public BoxCollider m_AttackCol;
	public CapsuleCollider m_BodyCol;

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

	bool IsAttack = false;

	Vector3 m_Movedirection;
	Vector3 m_Forward;
	Vector3 m_Velocity;

	Vector3 m_Attackdirection;
	Vector3 m_Attackforward;

	public void SetInfo () {
		m_Monster.Status.m_Status[StatusConstant.HP].Set(50f);
		m_Monster.Status.m_Status[StatusConstant.RegenHP].Set(0.2f);
		m_Monster.Status.m_Status[StatusConstant.RegenHPTime].Set(0.1f);
		m_Monster.Status.m_Status[StatusConstant.MP].Set(100f);
		m_Monster.Status.m_Status[StatusConstant.RegenMP].Set(0.05f);
		m_Monster.Status.m_Status[StatusConstant.RegenMPTime].Set(0.01f);
		m_Monster.Status.m_Status[StatusConstant.ATK].Set(15f);
		m_Monster.Status.m_Status[StatusConstant.ATKSpeed].Set(2f);
		m_Monster.Status.m_Status[StatusConstant.DEF].Set(0f);
		m_Monster.Status.m_Status[StatusConstant.MoveSpeed].Set(1f);
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
		m_FSM.AddState(State.Idle, null, IdleUpdatae, null);
		m_FSM.AddState(State.Move, null, MoveUpdatae, MoveExit);
		m_FSM.AddState(State.Attack, AttackEnter, null, AttackExit);

		m_FSM.SetState(State.Idle);
	}

	void Update() {
		if (m_Monster.Status.m_Status[StatusConstant.HP].GetPrint() <= 0 && m_FSM.GetState() != State.Death) {
			m_FSM.SetState(State.Death);
			m_FSM.SetEnd(false);
			m_Ani.SetTrigger("Death");
			m_BodyCol.enabled = false;
			m_AttackCol.enabled = false;
			IsAttack = false;
			m_Monster.Dead(m_Ani.GetCurrentAnimatorStateInfo(0).length + 0.3f);
			return;
		}
		if (m_FSM.GetEnd() && PlayerController.Instance.m_Stats.m_Status[StatusConstant.HP].GetPrint() <= 0) {
			m_FSM.SetEnd(false);
			m_Ani.Play("Idle", -1, 0);
			return;
		}

		m_fDistance = Vector3.Distance(Monster.m_Player.position, transform.position);
	}

	void OnTriggerEnter(Collider col) {
		if (IsAttack && col.gameObject.layer == CLayer.Player) {
			m_AttackCol.enabled = false;
			PlayerInformation.m_PlayerStats.ReceiveDamage(PlayerInformation.m_PlayerController.transform, m_Monster.Status.m_Status[StatusConstant.ATK].GetPrint(), true);
		}
	}

	//----------------------------------------------------------------------------------
	void IdleUpdatae() {
		if (m_fDistance <= m_fAttackDistance) {
			m_FSM.SetState(State.Attack);
			m_Ani.SetTrigger("Attack");
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
			IsAttack = true;

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
		IsAttack = false;
		m_FSM.SetState(State.Idle);
	}
	void AttackExit() {
		m_AttackCol.enabled = false;
		IsAttack = false;
	}
	//----------------------------------------------------------------------------------
}