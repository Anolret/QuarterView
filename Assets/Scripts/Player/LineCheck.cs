using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCheck : MonoBehaviour {
	public LineRenderer m_Line;
	public static int m_MonsterCount = 0;
	Color m_Color;

	private void Start() {
		m_Color = PlayerInformation.m_PlayerController.m_Line.startColor;
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == CLayer.Monster) {	
			m_MonsterCount++;
			m_Color = PlayerInformation.m_PlayerController.m_Line.startColor;
			m_Color.r = 0;
			m_Color.g = 1;
			PlayerInformation.m_PlayerController.m_Line.startColor = m_Color;
			PlayerInformation.m_PlayerController.m_Line.endColor = m_Color;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.layer == CLayer.Monster) {
			m_MonsterCount--;
			if(m_MonsterCount <= 0 && other.gameObject.GetComponent<CapsuleCollider>().isTrigger == true) {
				m_Color = PlayerInformation.m_PlayerController.m_Line.startColor;
				m_Color.r = 1;
				m_Color.g = 0;
				PlayerInformation.m_PlayerController.m_Line.startColor = m_Color;
				PlayerInformation.m_PlayerController.m_Line.endColor = m_Color;
			}
		}
	}
}
