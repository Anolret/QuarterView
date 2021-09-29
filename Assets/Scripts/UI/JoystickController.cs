using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {
	public Joystick m_Joystick;

	public Image m_RangeImg;

	bool m_bFadeInOutCheck;

	IEnumerator FadeIn() {
		Color _colorBG = m_Joystick.m_BgImg.color;
		Color _colorJS = m_Joystick.m_JsImg.color;
		m_bFadeInOutCheck = false;
		do {
			_colorBG.a = _colorJS.a += 0.037f;
			m_Joystick.m_BgImg.color = _colorBG;
			m_Joystick.m_JsImg.color = _colorJS;
			yield return null;
			if (m_bFadeInOutCheck) break;
		} while (_colorBG.a < 0.74f);
	}

	IEnumerator FadeOut() {
		Color _colorBG = m_Joystick.m_BgImg.color;
		Color _colorJS = m_Joystick.m_JsImg.color;
		m_bFadeInOutCheck = true;
		do {
			_colorBG.a = _colorJS.a -= 0.037f;
			m_Joystick.m_BgImg.color = _colorBG;
			m_Joystick.m_JsImg.color = _colorJS;
			yield return null;
			if (!m_bFadeInOutCheck) break;
		} while (0 < _colorBG.a);
	}

	public void OnPointerDown(PointerEventData ped) {
		m_RangeImg.raycastTarget = false;
		m_Joystick.m_BgImg.rectTransform.position = new Vector3(ped.position.x, ped.position.y, 10f);
	}
	
	public void OnDrag(PointerEventData ped) {
		PlayerInformation.m_PlayerController.m_Line.SetPosition(0, PlayerInformation.m_PlayerController.m_Bowshield.transform.position);

		float _x = PlayerInformation.m_PlayerController.m_Bowshield.transform.position.x + (PlayerInformation.m_PlayerController.transform.forward.x * 7);
		float _y = PlayerInformation.m_PlayerController.m_Bowshield.transform.position.y;
		float _z = PlayerInformation.m_PlayerController.m_Bowshield.transform.position.z + (PlayerInformation.m_PlayerController.transform.forward.z * 7);

		PlayerInformation.m_PlayerController.m_Line.transform.position = PlayerInformation.m_PlayerController.m_Bowshield.transform.position;
		PlayerInformation.m_PlayerController.m_Line.transform.forward = PlayerInformation.m_PlayerController.m_Bowshield.transform.forward;

		PlayerInformation.m_PlayerController.m_Line.SetPosition(1, new Vector3(_x, _y, _z));

		if (!PlayerInformation.m_bDragCheck) {
			PlayerInformation.m_bDragCheck = true;
			if (0 < PlayerInformation.m_PlayerStats.m_Status[StatusConstant.HP].GetPrint() && PlayerInformation.m_PlayerController.Isforward && PlayerInformation.m_bMoveCheck == true) {
				PlayerInformation.m_PlayerController.m_Ani.Play("Run");
			}
		}
		m_Joystick.OnDrag(ped);
	}
	
	public void OnPointerUp(PointerEventData ped) {
		m_RangeImg.raycastTarget = true;
		if(Skill.m_IsSkillCheck && 0 < PlayerInformation.m_PlayerStats.m_Status[StatusConstant.HP].GetPrint()) {
			if (AttackButton.IsAttack) {
				PlayerInformation.m_PlayerController.m_Ani.Play("Charge");
			} else {
				PlayerInformation.m_PlayerController.m_Ani.Play("AttackReady");
			}
		}

		PlayerInformation.m_bDragCheck = false;
		m_Joystick.OnPointerUp(ped);
	}

	void Start() {
		m_bFadeInOutCheck = false;
	}
}
