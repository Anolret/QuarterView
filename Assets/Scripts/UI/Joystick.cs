using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
	public Image BG;
	/// <summary>
	/// 스틱 몸통(부모)
	/// </summary>
	public Image m_BgImg;
	/// <summary>
	/// 스틱(자식)
	/// </summary>
	public Image m_JsImg;

	/// <summary>
	/// 노멀라이즈 할 것 리셋하는 방향
	/// </summary>
	public Vector3 InputDirection { set; get; }
	/// <summary>
	/// 노멀라이즈 할 것 유지 방향
	/// </summary>
	public static Vector3 NSInputDirection;

	public Vector3 ImgDirection { set; get; }

	/// <summary>
	/// 조이스틱 범위
	/// </summary>
	public float m_Radius;

	public bool IsTouch = false;

	Vector3 m_Move;
	Vector2 m_MausPos;

	void Awake() {
		m_BgImg = GetComponent<Image>();
		m_JsImg = GetComponentsInChildren<Image>()[1];
		InputDirection = Vector3.zero;
		NSInputDirection = Vector3.zero;

		m_Radius = m_BgImg.rectTransform.rect.width * 0.25f;
	}

	public void OnPointerDown(PointerEventData eventData) { IsTouch = true; }
	
	void NoRange(PointerEventData eventData) {
		m_MausPos.x = eventData.position.x;
		m_MausPos.y = eventData.position.y;
		ImgDirection = m_MausPos - (Vector2)m_BgImg.transform.position;
		NSInputDirection.x = ImgDirection.x;
		NSInputDirection.z = ImgDirection.y;

		InputDirection = NSInputDirection;

		m_JsImg.transform.localPosition = ImgDirection;

		float _distance = Vector2.Distance(m_BgImg.transform.position, m_JsImg.transform.position) / m_Radius;
		if (1f < _distance) {
			_distance -= 1f;
			m_Move = m_BgImg.transform.position + (ImgDirection * _distance);
			if (!float.IsInfinity(m_Move.x) && !float.IsInfinity(m_Move.y)) {
				m_BgImg.transform.position = m_Move;
			} else {
				m_BgImg.rectTransform.anchoredPosition = Vector3.zero;
			}
		}
	}

	public void OnDrag(PointerEventData eventData) {
		NoRange(eventData);
	}

	public void OnPointerUp(PointerEventData eventData) {
		IsTouch = false;
		InputDirection = m_BgImg.rectTransform.anchoredPosition = m_JsImg.rectTransform.localPosition = Vector3.zero;
	}
}