using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingSkill : MonoBehaviour {

	public void OnTriggerEnter(Collider col) {
		if (col.gameObject.layer == CLayer.Monster) {
			Monster _HitObj = col.GetComponent<Monster>();
			SkillManager.Instance.Knockback(transform, _HitObj, 20f);
			SkillManager.Instance.Hit(_HitObj, PlayerInformation.m_PlayerController.m_Stats.m_Status[StatusConstant.ATK].GetPrint());
		}
	}
}
