using UnityEngine;

public static class WFS {
	/// <summary>
	/// 1 / 60 (0.01666)
	/// </summary>
	public static WaitForSeconds sdd16 = new WaitForSeconds(0.01666f);
	public static WaitForSeconds sdd1 = new WaitForSeconds(0.01f);
	/// <summary>
	/// 1 / 30 (0.03333)
	/// </summary>
	public static WaitForSeconds sdd3 = new WaitForSeconds(0.03333f);
	public static WaitForSeconds sdd5 = new WaitForSeconds(0.05f);
	public static WaitForSeconds sd1 = new WaitForSeconds(0.1f);
	public static WaitForSeconds sd15 = new WaitForSeconds(0.15f);
	public static WaitForSeconds sd2 = new WaitForSeconds(0.2f);
	public static WaitForSeconds sd5 = new WaitForSeconds(0.5f);
	public static WaitForSeconds s1 = new WaitForSeconds(1f);
	public static WaitForSeconds s5 = new WaitForSeconds(5f);
}

public static class CLayer {
	public const int Player = 8;
	public const int PlayerObj = 9;

	public const int Field = 11;
	public const int FieldObj = 12;

	public const int Monster = 14;
	public const int MonsterObj = 15;

	public const int NPC = 17;
	public const int NPCObj = 18;

	public const int Item = 31;
}

/// <summary>
/// 스테이터스 Enum(10)
/// </summary>
public static class StatusConstant {
	public const int MaxStatus = 10;
	public static string[] Name = new string[10]
	{"생명력", "생명력 재생", "생명력 재생 시간",
	 "마나", "마나 재생", "마나 재생 시간",
	 "공격력","공격속도","방어력","이동속도"
	};

	public const int HP = 0;
	public const int RegenHP = 1;
	public const int RegenHPTime = 2;

	public const int MP = 3;
	public const int RegenMP = 4;
	public const int RegenMPTime = 5;

	public const int ATK = 6;
	public const int ATKSpeed = 7;
	public const int DEF = 8;
	public const int MoveSpeed = 9;
}