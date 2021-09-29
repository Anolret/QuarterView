using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스킬 선택 창의 스킬
/// </summary>
public class SkillSet : MonoBehaviour {
	public static List<SkillSet> cThis = new List<SkillSet>();
	public int SkillNum; //스킬 번호
	public RectTransform RT;
	public Image Img;
	public Text ButtonText;
	public Text Name;
	public Text Cost_CoolTiem;
	public Text Data;

	public void SetSkillData(int _skillNum, Sprite _img, string _name, string _cost_cooltime, string _data) {
		RT.anchoredPosition3D = new Vector3(0, 0, 0);
		UISetting.sH_Size(RT);
		SkillNum = _skillNum;
		Img.sprite = _img;
		Name.text = _name;
		Cost_CoolTiem.text = _cost_cooltime;
		Data.text = _data;
		if (cThis.Count >= 2) {
			Vector3 _v = cThis[cThis.Count-1].RT.anchoredPosition3D;
			_v.y = -cThis[0].RT.rect.height * (cThis.Count-1);
			cThis[cThis.Count-1].RT.anchoredPosition3D = _v;
		}
	}

	public void SelectSkill() {
		SkillManager.Instance.SelectSkill(this);
	}
}