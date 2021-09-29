using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Arrow : MonoBehaviour {
	float m_fPower = 10;
	float m_fDmage = 0;
	int m_nEffect = 0;

	/// <summary>
	/// 한마리에게만 히트하도록 체크
	/// </summary>
	bool m_bHitOnce = false;

	Monster m_HitObj;

	bool m_bHitCheck = false;

	Vector3 m_MovePos;

	float m_fPrevious = 0f;
	/// <summary>
	/// 이동 할 거리
	/// </summary>
	float m_Distance = 0f;

	public AudioSource m_Audio;

	public BoxCollider m_BoxCollider;
	Vector3 m_BoxColliderSize;

	Quaternion m_Rot;

	static bool IsGameStart = false;

	public static Queue<Arrow> Arrows = new Queue<Arrow>();
	public static Arrow Pooling() {
		Arrow _arrow;
		if (Arrows.Count == 0) Arrows.Enqueue(Instantiate(PlayerController.Instance.m_Arrow));
		_arrow = Arrows.Dequeue();
		_arrow.gameObject.SetActive(true);
		return _arrow;
	}

	public void Set(float _power, float _damge, int _effect = 0) {
		m_fPower = _power;
		m_fDmage = _damge;
		m_nEffect = _effect;
		GetMovePos(7f);
	}

	public void Set(float _power, float _damge, float _length, int _effect = 0) {
		m_fPower = _power;
		m_fDmage = _damge;
		GetMovePos(_length);
		m_nEffect = _effect;
	}

	void OnTriggerEnter(Collider col) {
		m_bHitCheck = false;
		if (!m_bHitCheck) {
			if (col.gameObject.layer == CLayer.Monster) {
				m_HitObj = col.GetComponent<Monster>();
				if(m_HitObj && 0 < m_HitObj.Status.m_Status[StatusConstant.HP].GetPrint()) {
					m_Audio.Play();
					switch (m_nEffect) {
						case 0: //일반
							if (!m_bHitOnce) {
								m_bHitOnce = true;
								SkillManager.Instance.Hit(m_HitObj, m_fDmage);
								m_Distance = 0;
							}
							break;
						case 1: //넉빽, 슬로우
							if (!m_bHitOnce) {
								m_bHitOnce = true;
								SkillManager.Instance.Knockback(transform, m_HitObj);
								//SkillControllor.cThis.Slow(m_HitObj.Status, 50f);
								SkillManager.Instance.Hit(m_HitObj, m_fDmage);
								m_Distance = 0;
							}
							break;
						case 2: //넉빽, 관통, 슬로우
							SkillManager.Instance.Knockback(transform, m_HitObj);
							//SkillControllor.cThis.Slow(m_HitObj.Status, 50f);
							SkillManager.Instance.Hit(m_HitObj, m_fDmage);
							break;
						case 3: //슬로우
							if (!m_bHitOnce) {
								m_bHitOnce = true;
								SkillManager.Instance.Slow(m_HitObj.Status, 20f);
								SkillManager.Instance.Hit(m_HitObj, m_fDmage);
								m_Distance = 0;
							}
							break;
					}
				}
			}
		}
	}

	public void GetMovePos(float _length) {
		m_MovePos = transform.position + transform.forward * _length;
		m_Distance = Vector3.Distance(m_MovePos, transform.position);
	}

	void Awake() {
		if (!IsGameStart) Arrows.Enqueue(this);
	}

	void Start() {
		IsGameStart = true;
		m_BoxColliderSize = m_BoxCollider.size;
		m_Rot = transform.rotation;
	}

	void Move() {
		if (0f < m_Distance) {
			m_fPrevious = m_Distance;
			m_Distance = Vector3.Distance(m_MovePos, transform.position);
			transform.Translate(Vector3.forward * m_fPower * Time.deltaTime);
			if (m_fPrevious < m_Distance) { m_Distance = 0f; }
		} else {
			m_bHitOnce = false;
			m_BoxCollider.size = m_BoxColliderSize;
			transform.localPosition = Vector3.zero;
			transform.rotation = m_Rot;
			Arrows.Enqueue(this);
			gameObject.SetActive(false);
		}
	}

	void FixedUpdate() {
		Move();
	}
}
