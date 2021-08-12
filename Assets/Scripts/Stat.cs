using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class BaseStatus {
	/// <summary>
	/// 기초 : 
	/// </summary>
	float m_fBase;
	/// <summary>
	/// 변환 : 
	/// </summary>
	float m_fChange;
	/// <summary>
	/// 출력 : 
	/// </summary>
	float m_fPrint;

	/// <summary>
	/// 기본 변수
	/// </summary>
	public float GetBase() { return m_fBase; }
	/// <summary>
	/// 변환 데이터
	/// </summary>
	public float GetChange() { return m_fChange; }
	/// <summary>
	/// 출력 데이터
	/// </summary>
	public float GetPrint() { return m_fPrint; }

	public BaseStatus() { m_fPrint = m_fChange = m_fBase = 0f; }
	public BaseStatus(float _receive)	{ m_fPrint = m_fChange = m_fBase = _receive; }

	/// <summary>
	/// 최초 능력치 설정
	/// </summary>
	/// <param name="_receive"></param>
	public void Set(float _receive)	{ m_fPrint = m_fChange = m_fBase = _receive; }
	/// <summary>
	/// 기존 수치에 업
	/// </summary>
	/// <param name="_receive"></param>
	/// <param name="_isFixing">고정 값을 안써야하는지 ex)HP, MP</param>
	/// <returns></returns>
	public float UP(float _receive, bool _isFixing = true) { //기존 수치에 업
		if (_isFixing) {
			m_fPrint = m_fChange += _receive;
		} else {
			m_fChange += _receive;
		}
		return m_fPrint;
	}
	/// <summary>
	/// 기존 수치에 다운
	/// </summary>
	/// <param name="_receive"></param>
	/// <param name="_isFixing">고정 값을 안써야하는지 ex)HP, MP</param>
	/// <returns></returns>
	public float Down(float _receive, bool _isFixing = true) { //기존 수치에 다운
		if (_isFixing) {
			m_fChange -= _receive;
			if (m_fChange < 0f) {
				m_fChange = 0f;
			}
			m_fPrint = m_fChange;
		} else {
			m_fChange -= _receive;
			if (m_fChange < 0f) {
				m_fChange = 0f;
			}
		}

		return m_fPrint;
	}

	public float Regen(float _receive, int _percent = 0) {
		if (_percent == 1) {
			m_fPrint += (m_fChange* _receive) / 100f;
		} else if (_percent == 2) {
			m_fPrint += (_receive * _receive) / 100f;
		} else {
			m_fPrint += _receive;
		}
        if (m_fChange < m_fPrint) {
			m_fPrint = m_fChange;
		}
		return m_fPrint;
	}

	/// <summary>
	/// 현제 수치증가 및 기존수치 보다 증가막음
	/// </summary>
	/// <param name="_receive">0. 수치 그대로 적용, 1. 50 = 50%, 2. 모르겠음</param>
	/// <param name="_percent">백분율 종류</param>
	/// <returns>float</returns>
	public float Increase(float _receive, int _percent = 0) {
		if (_percent == 1) {
			m_fPrint += (m_fChange * _receive) / 100f;
		} else if(_percent == 2) {
			m_fPrint += (_receive * _receive) / 100f;
		} else {
			m_fPrint += _receive;
		}
		return m_fPrint;
	}

	/// <summary>
	/// 기존 수치에 피해 및 기존수치 보다 감소막음
	/// </summary>
	/// <param name="_receive">0. 수치 그대로 적용, 1. 50 = 50%, 2. 모르겠음</param>
	/// <param name="_percent">백분율 종류</param>
	/// <returns>float</returns>
	public float Decrease(float _receive, int _percent = 0)	{
		if (_percent == 1) {
			m_fPrint -= (m_fChange * _receive) / 100f;
		} else if (_percent == 2) {
			m_fPrint -= (_receive * _receive) / 100f; ;
		} else {
			m_fPrint -= _receive;
		}
		if (m_fPrint < 0f) {
			m_fPrint = 0f;
		}
		return m_fPrint;
	}

	/// <summary>
	/// 수치 적용
	/// </summary>
	/// <param name="_receive"></param>
	public void Apply(float _receive) {
		m_fPrint = _receive;
	}

	/// <summary>
	/// 수치 복구
	/// </summary>
	/// <returns></returns>
	public float GetBack() { m_fPrint = m_fChange; return m_fPrint; }

	/// <summary>
	/// 수치가 변했는지 체크
	/// </summary>
	/// <returns>true : 똑같음, false : 다름</returns>
	public bool Compare() {
		if (m_fPrint == m_fChange) return true;
		return false;
	}
}

public class Stat {

	public enum CurrentState {
		/// <summary> Unit 기본 상태 </summary>
		Idle = 0,
		/// <summary> Unit 이동 상태 </summary>
		Walk,
		/// <summary> Unit 공격 상태 </summary>
		Attack,
		/// <summary> Unit 죽음 상태 </summary>
		Death,
		/// <summary> Unit 스킬 사용 상태 </summary>
		UseSkill
	}

	public CurrentState CS;

	public int m_nEXP;

	public BaseStatus[] m_Status = new BaseStatus[StatusConstant.MaxStatus];

	public Stat() {
		m_nEXP = 0;
		for (int i = 0; i < StatusConstant.MaxStatus; i++)	{
			m_Status[i] = new BaseStatus();
		}
	}

	public float ReceiveDamage(Transform _pos, float _receive, bool _IgnoreArmor = false) {
		try {
			float _damage = 1f;
			if (_IgnoreArmor) {
				bool _compare = _receive <= m_Status[StatusConstant.DEF].GetPrint() ? true : false;
				if (!_compare) _damage = _receive - m_Status[StatusConstant.DEF].GetPrint();
			} else {
				_damage = _receive;
			}
			m_Status[StatusConstant.HP].Decrease(_damage);
			TextManager.Instance.SetText(_pos.position, _damage, new Color(1, 0, 0));
			return m_Status[StatusConstant.HP].GetPrint();
		} catch (System.Exception e) {
			Debug.Log(e.Message);
		}
		return 0f;
	}

	public void ReceiveData(int _type, float _receive, bool _IgnoreArmor = false) {
		m_Status[_type].Regen(_receive);
	}

	public float Percentage(int _status) {
		return m_Status[_status].GetPrint() / m_Status[_status].GetChange();
	}

	/// <summary>
	/// 장비 착용
	/// </summary>
	public void AddEquipment(BaseStatus[] _Status) {
        for (int i = 0 ; i < StatusConstant.MaxStatus; i++) {
			if (0 < _Status[i].GetBase()) {
				if (i == StatusConstant.HP || i == StatusConstant.MP) {
					m_Status[i].UP(_Status[i].GetBase(), false);
				} else {
					m_Status[i].UP(_Status[i].GetBase());
				}
			}
		}
	}
	/// <summary>
	/// 장비 해제
	/// </summary>
	/// <param name="_Status"></param>
	public void RemoveEquipment(BaseStatus[] _Status) {
		for (int i = 0; i < StatusConstant.MaxStatus; i++) {
			if (0 < _Status[i].GetBase()) {
				if (i == StatusConstant.HP || i == StatusConstant.MP) {
					m_Status[i].Down(_Status[i].GetBase(), false);
				} else {
					m_Status[i].Down(_Status[i].GetBase());
				}
			}
		}
	}

	public void GetStatusSTR() {
        for (int i = 0 ; i < StatusConstant.MaxStatus ; i++) {
			Debug.Log("능력치(" + i + "): " + m_Status[i].GetPrint());
		}
    }
}