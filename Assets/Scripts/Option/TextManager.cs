using UnityEngine;

public class TextManager : MonoBehaviour {
	public static TextManager Instance;
	public NumberText TextObj;

	void Awake() {
		Instance = this;
	}

	public void SetText(Vector3 _pos, float _damage, Color _color, bool _isPos = false) {
		NumberText _text = NumberText.Pooling();
		if (_isPos) {
			_pos.x += Random.Range(-0.3f, 0.3f);
			_pos.z += Random.Range(-0.3f, 0.3f);
		}
		_text.transform.position = _pos;
		_text.transform.rotation = transform.rotation;
		_text.MText.text = ((int)_damage).ToString();
		_text.Pos = _text.transform.position;
		_text.TextColor = _text.MText.color = _color;
	}

	public void SetText(Vector3 _pos, string _str, Color _color, int _fontSize = 40) {
		NumberText _text = NumberText.Pooling();
		_text.transform.position = _pos;
		_text.transform.rotation = transform.rotation;
		_text.MText.text = _str;
		_text.MText.fontSize = _fontSize;
		_text.Pos = _text.transform.position;
		_text.TextColor = _text.MText.color = _color;
	}
}
