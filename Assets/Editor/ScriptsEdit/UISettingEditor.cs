using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UISetting))]
public class UISettingEditor : Editor {
	public RectTransform Parent;
	public UISetting cThis;

    private void OnEnable() {
		if (AssetDatabase.Contains(target)) {
			cThis = null;
		} else {
			cThis = (UISetting)target;
			cThis.RT = cThis.GetComponent<RectTransform>();
			Parent = cThis.gameObject.transform.parent.GetComponent<RectTransform>();
		}
	}



	Vector2 mAnchorToPos;

	Vector2 mAnchorMin;
	Vector2 mAnchorMax;

	Vector2 Size;

	public override void OnInspectorGUI() {
		/*
				mScreenSize.x = ScreenSize.width;
				mScreenSize.y = ScreenSize.height;
				EditorGUILayout.Vector2Field("화면 사이즈", mScreenSize);


				ScreenPos = EditorGUILayout.Vector2Field("위치", cThis.m_RT.anchoredPosition);
				Size = EditorGUILayout.Vector2Field("크기", cThis.m_RT.rect.size);

				cThis.m_RT.anchoredPosition = ScreenPos;
				cThis.m_RT.sizeDelta = Size;

				EditorGUILayout.Vector2Field("3위치", cThis.RT.transform.position);
		*/
		//EditorGUILayout.ObjectField(cThis, typeof(UISetting), false);

		cThis.ScreenSet = EditorGUILayout.Toggle("화면 크기로 설정", cThis.ScreenSet);

		EditorGUILayout.Vector2Field("월드 포지션", cThis.RT.transform.position);
		EditorGUILayout.RectField("자신 Rect",cThis.RT.rect);
		EditorGUILayout.RectField("부모 Rect",Parent.rect);

		if (GUILayout.Button("AnchorToPos")) {
			Size.x = cThis.RT.rect.width;
			Size.y = cThis.RT.rect.height;

			mAnchorToPos.x = (cThis.RT.transform.localPosition.x + (Parent.rect.width  * Parent.pivot.x)) / Parent.rect.width;
			mAnchorToPos.y = (cThis.RT.transform.localPosition.y + (Parent.rect.height * Parent.pivot.y)) / Parent.rect.height;

			cThis.RT.anchorMin = mAnchorToPos;
			cThis.RT.anchorMax = mAnchorToPos;

			cThis.RT.anchoredPosition = Vector2.zero;
			cThis.RT.sizeDelta = Size;
		}
		if (GUILayout.Button("AnchorToSziePos")) {
			mAnchorMin.x = ((cThis.RT.transform.localPosition.x + (Parent.rect.width  * Parent.pivot.x)) + cThis.RT.rect.x) / Parent.rect.width;
			mAnchorMin.y = ((cThis.RT.transform.localPosition.y + (Parent.rect.height * Parent.pivot.y)) + cThis.RT.rect.y) / Parent.rect.height;

			mAnchorMax.x = ((cThis.RT.transform.localPosition.x + (Parent.rect.width  * Parent.pivot.x)) - cThis.RT.rect.x) / Parent.rect.width;
			mAnchorMax.y = ((cThis.RT.transform.localPosition.y + (Parent.rect.height * Parent.pivot.y)) - cThis.RT.rect.y) / Parent.rect.height;

			cThis.RT.anchorMin = mAnchorMin;
			cThis.RT.anchorMax = mAnchorMax;

			cThis.RT.anchoredPosition = Vector2.zero;
			cThis.RT.sizeDelta = Vector2.zero;

		}
	}
}