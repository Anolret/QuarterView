using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	float m_fPower = 0f;
	int m_nEffect = 0;
	/// <summary>
	/// 버튼 업 체크
	/// </summary>
	bool IsAttackUPCheck = true;
	/// <summary>
	/// 버튼 다운 체크
	/// </summary>
	bool IsAttackDownCheck = false;

	public static bool IsAttack = false;

	Vector3 AttackVector; //라인 위치

	float Chargefigures;

	IEnumerator m_IECharge;
	IEnumerator IECharge() {
		yield return WFS.sd5;
		PlayerInformation.m_PlayerController.m_Charge.SetActive(false);
		PlayerInformation.m_PlayerController.m_Charge.SetActive(true);
		m_fPower = 15f;
		m_nEffect = 1;
		Chargefigures = 1.5f;
		yield return WFS.s1;
		PlayerInformation.m_PlayerController.m_Charge.SetActive(false);
		PlayerInformation.m_PlayerController.m_Charge.SetActive(true);
		m_fPower = 25f;
		m_nEffect = 2;
		Chargefigures = 2.5f;
	}

	void Power_Effect() {
		if (!PlayerInformation.IsPlayerDeath && !IsAttack && IsAttackUPCheck) {
			IsAttack = true;
			PlayerInformation.m_PlayerController.m_LineBox.enabled = true;
			PlayerInformation.m_bMoveCheck = false;
			PlayerInformation.m_PlayerController.m_Ani.Play("ChargeStart");
			//
			Color _color = PlayerInformation.m_PlayerController.m_Line.startColor;
			_color.a = 1;
			if (LineCheck.m_MonsterCount <= 0) {
				_color.r = 1;
				_color.g = 0;
			}
			PlayerInformation.m_PlayerController.m_Line.startColor = _color;
			PlayerInformation.m_PlayerController.m_Line.endColor = _color;

			PlayerInformation.m_PlayerController.m_Line.SetPosition(0, PlayerInformation.m_PlayerController.m_Bowshield.transform.position);

			AttackVector.x = PlayerInformation.m_PlayerController.m_Bowshield.transform.position.x + (PlayerInformation.m_PlayerController.transform.forward.x * 7);
			AttackVector.y = PlayerInformation.m_PlayerController.m_Bowshield.transform.position.y;
			AttackVector.z = PlayerInformation.m_PlayerController.m_Bowshield.transform.position.z + (PlayerInformation.m_PlayerController.transform.forward.z * 7);

			PlayerInformation.m_PlayerController.m_Line.SetPosition(1, AttackVector);

			PlayerInformation.m_PlayerController.m_Line.transform.position = PlayerInformation.m_PlayerController.m_Bowshield.transform.position;
			PlayerInformation.m_PlayerController.m_Line.transform.forward = PlayerInformation.m_PlayerController.m_Bowshield.transform.forward;
			//
			IsAttackDownCheck = true;
			m_fPower = 10f;
			m_nEffect = 0;
			Chargefigures = 1f;
			m_IECharge = IECharge();
			StartCoroutine(m_IECharge);
		}
	}

	void InAttackReadyani() {
		if (!PlayerInformation.IsPlayerDeath) {
			if (!PlayerInformation.m_bDragCheck) {
				PlayerInformation.m_PlayerController.m_Ani.Play("AttackReady");
			} else {
				PlayerInformation.m_PlayerController.m_Ani.Play("Run");
			}
			PlayerInformation.m_bMoveCheck = true;
		}
	}

	void AttackCheck() {
		IsAttackUPCheck = true;
	}

	public void OnPointerDown(PointerEventData eventData) {
		if (!NPCManager.npcsInCheck) {
			Power_Effect();
		} else {
			NPCManager.Instance.NPCchoiceActive();
		}
	}

	public void OnPointerUp(PointerEventData eventData) {
		if (!PlayerInformation.IsPlayerDeath && IsAttackUPCheck && IsAttackDownCheck) {
			PlayerInformation.m_PlayerController.m_LineBox.enabled = false;
			LineCheck.m_MonsterCount = 0;
			StopCoroutine(m_IECharge);
			IsAttackUPCheck = false;
			IsAttackDownCheck = false;
			IsAttack = false;

			if (!PlayerInformation.IsPlayerDeath) {
				PlayerInformation.m_PlayerController.m_Ani["ChargeEnd"].speed = 2f;
				PlayerInformation.m_PlayerController.m_Ani.Play("ChargeEnd");
			}

			PlayerInformation.m_PlayerController.Attack(
				m_fPower,
				PlayerInformation.m_PlayerStats.m_Status[StatusConstant.ATK].GetPrint() * Chargefigures,
				m_nEffect);

			Invoke("AttackCheck", 1f / PlayerInformation.m_PlayerStats.m_Status[StatusConstant.ATKSpeed].GetPrint());
			Invoke("InAttackReadyani", 0.5f / PlayerInformation.m_PlayerStats.m_Status[StatusConstant.ATKSpeed].GetPrint());
			//
			Color _color = PlayerInformation.m_PlayerController.m_Line.startColor;
			_color.a = 0;
			PlayerInformation.m_PlayerController.m_Line.startColor = _color;
			PlayerInformation.m_PlayerController.m_Line.endColor = _color;
		}
	}
}
