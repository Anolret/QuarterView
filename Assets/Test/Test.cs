//https://blog.naver.com/PostView.nhn?blogId=ekfvoddl3535&logNo=221723666319&from=search&redirect=Log&widgetTypeCall=true&directAccess=false

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

enum SkillName {
	Attack,
	Move,
	Path
}

public struct SkillKey {

	public string name;
	public int number;

	public SkillKey(string _name, int _number) {
		name = _name;
		number = _number;
	}

	public SkillKey(int _number) {
		name = ((SkillName) _number).ToString();
		number = _number;
	}
}

///---------------------------------------------------------

public class Test : MonoBehaviour {
	public Dictionary<SkillKey, Skill> m_Skill = new Dictionary<SkillKey, Skill>();

	void Awake() {
		m_Skill.Add(new SkillKey(0), new Skill());
	}

	SkillKey GetKey(int num) {
		SkillKey asd;
		asd.name = ((SkillName)num).ToString();
		asd.number = num;
		return asd;
	}

	void Start() {
	}

	void Update() {

	}
}
