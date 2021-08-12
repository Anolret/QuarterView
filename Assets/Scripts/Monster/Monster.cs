using System;
using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour {

	public static Transform m_Player;
	[HideInInspector] public Stat Status = new Stat();
	[HideInInspector] public Rigidbody m_Rigid;
	[HideInInspector] public bool m_bDead = false;
	[HideInInspector] public bool m_bMoveCheck = true;
	[HideInInspector] public bool m_bKnuckBack = false;
	[HideInInspector] public bool m_bStun = false;

	public Action EndAct;

	IEnumerator m_DeadTime;

	void Awake() {
		m_Rigid = GetComponent<Rigidbody>();
	}

	void Start() {
		//***수정해야함
		try {
			/*int _num = EnemyManager.Instance.Monsters.FindIndex(a => a == null);

			if (_num != -1) {
				EnemyManager.Instance.Monsters[_num] = this;
			} else {
				EnemyManager.Instance.Monsters.Add(this);
			}*/
			EnemyManager.Instance.Monsters.Add(this);
		} catch {

		}
	}

	IEnumerator DeadTime(float _anidTime) {
		yield return new WaitForSeconds(_anidTime);
		GameObject _ext = Instantiate(GameMaster.Instance.m_ExtinctionImpact);
		_ext.transform.position = transform.position;
		yield return WFS.sd1;
		transform.position = new Vector3(0, -10, 0); //삭제 전 먼저 이동
		EndAct?.Invoke();
		yield return WFS.s1;
		EnemyManager.Instance.Monsters.Remove(gameObject.GetComponent<Monster>());
		Destroy(gameObject);
	}

	public void Dead(float _anidTime) {
		if(m_DeadTime == null) {
			ItemManager.Instance.ItemProduce(gameObject.transform.position);
			m_DeadTime = DeadTime(_anidTime);
			StartCoroutine(m_DeadTime);
			m_bDead = true;
			m_bMoveCheck = false;
		}
	}

	public bool Life()	{
		if(0 < Status.m_Status[StatusConstant.HP].GetPrint()) { return false; }
		return true;
	}

	public IEnumerator RegenHP() {
		WaitForSeconds wf = new WaitForSeconds(Status.m_Status[StatusConstant.RegenHPTime].GetPrint());
		while (0 < Status.m_Status[StatusConstant.HP].GetPrint()) {
			Status.m_Status[StatusConstant.HP].Regen(Status.m_Status[StatusConstant.RegenHP].GetPrint());
			yield return wf;
		}
	}

	public IEnumerator RegenSP() {
		WaitForSeconds wf = new WaitForSeconds(Status.m_Status[StatusConstant.RegenMPTime].GetPrint());
		while (0 < Status.m_Status[StatusConstant.HP].GetPrint()) {
			Status.m_Status[StatusConstant.MP].Regen(Status.m_Status[StatusConstant.RegenMP].GetPrint());
			yield return wf;
		}
	}
}