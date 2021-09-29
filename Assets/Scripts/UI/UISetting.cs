using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : MonoBehaviour {

	public RectTransform RT;

	public bool ScreenSet = false;

	public static void SetActive(GameObject _obj) {
		_obj.SetActive(!_obj.activeSelf);
	}

	void W_Size() {
		Vector3 _pos = RT.anchoredPosition3D;
		Vector2 _size = RT.sizeDelta;
		
		_pos = new Vector3(1080 / _pos.x, 1920 / _pos.y, _pos.z);

		_size = new Vector2(1920 / _size.x, 1920 / _size.y);

		float w = Screen.width;

		RT.anchoredPosition3D = new Vector3(Screen.width / _pos.x, Screen.height / _pos.y, _pos.z);
		RT.sizeDelta = new Vector2(w / _size.x, w / _size.y);
	}

	void Size() { //기본
		Vector3 _pos = RT.anchoredPosition3D;
		Vector2 _size = RT.sizeDelta;

		_pos = new Vector3(1920 / _pos.x, 1080 / _pos.y, _pos.z);

		_size = new Vector2(1080 / _size.x, 1080 / _size.y);

		float _minSize = (Screen.width > Screen.height) ? Screen.height : Screen.width;

		RT.anchoredPosition3D = new Vector3(Screen.width / _pos.x, Screen.height / _pos.y, _pos.z);
		RT.sizeDelta = new Vector2(_minSize / _size.x, _minSize / _size.y);
	}

	public static void sH_Size(RectTransform _this) {
		Vector3 _pos = _this.anchoredPosition3D;
		Vector2 _size = _this.sizeDelta;

		_pos = new Vector3(1920 / _pos.x, 1080 / _pos.y, _pos.z);

		_size = new Vector2(1080 / _size.x, 1080 / _size.y);

		float h = Screen.height;

		_this.anchoredPosition3D = new Vector3(Screen.width / _pos.x, Screen.height / _pos.y, _pos.z);
		_this.sizeDelta = new Vector2(h / _size.x, h / _size.y);
	}
	/// <summary>
	/// 화면에 맞추어 크기 변경
	/// </summary>
	/// <param name="_parent">부모기준</param>
	/// <param name="_this"></param>
	public static void sH_Size(RectTransform _parent, RectTransform _this) {
		Vector3 _pos = _this.anchoredPosition3D;
		Vector2 _size = _this.sizeDelta;

		_pos = new Vector3(_parent.sizeDelta.x / _pos.x, _parent.sizeDelta.y / _pos.y, _pos.z);
		Debug.Log(_pos.x);
		_size = new Vector2(_parent.sizeDelta.x / _size.x, _parent.sizeDelta.y / _size.y);

		float h = _parent.sizeDelta.x;

		_this.anchoredPosition3D = new Vector3(_parent.sizeDelta.x / _pos.x, _parent.sizeDelta.y / _pos.y, _pos.z);
		_this.sizeDelta = new Vector2(h / _size.x, h / _size.y);
	}
	
	void ScreenSetting() {
		RT.sizeDelta = new Vector2(Screen.width, Screen.height);
	}

	void Awake() {
		RT = GetComponent<RectTransform>();

		if (ScreenSet) {
			ScreenSetting();
		} else {
			Size(); //기본
		}
	}


	void Start() {

		
		//Debug.Log(w + " / " + h);
	}

	//void Update() {

	//}
}