using UnityEngine;
using UnityEngine.UI;

public class FrameCount : MonoBehaviour {
	public static FrameCount Instance;

	float deltaTime = 0.0f;
	Rect rect;
	int w, h;

	GUIStyle m_style = new GUIStyle();
	float msec;
	float fps;

	void Start () {
		Instance = this;

		w = Screen.width;
		h = Screen.height;
		rect = new Rect(780f, 0, w, h * 2 / 100);

		m_style.alignment = TextAnchor.UpperLeft;
		m_style.fontSize = h * 5 / 100;
		m_style.normal.textColor = new Color(1f, 1f, 1f, 1.0f);
	}

	void Update () {
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	void OnGUI () {
		msec = deltaTime * 1000.0f;
		fps = 1f / deltaTime;
		GUI.Label(rect, string.Format("{0:0.0} ms ({1:0.0} fps)", msec, fps), m_style);
	}
}