using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberText : MonoBehaviour {
	public TextMesh MText;
	public Vector3 Pos;
	public Color TextColor;

	public static List<NumberText> NumberTexts = new List<NumberText>();

	private void Start() {
		NumberTexts.Add(this);
		gameObject.SetActive(false);
	}

	public static NumberText Pooling() {
		NumberText _numberText = NumberTexts.Find(n => n.gameObject.activeSelf == false);
		if (_numberText == null) _numberText = Instantiate(TextManager.Instance.TextObj);
		_numberText.gameObject.SetActive(true);
		return _numberText;
	}

	void Update() {
		if (0 < TextColor.a) {
			TextColor.a -= 0.005f;
			MText.color = TextColor;

			Pos.y += 0.01f;
			transform.position = Pos;
		} else {
			transform.position = Vector3.zero;
			gameObject.SetActive(false);
		}
	}
}
