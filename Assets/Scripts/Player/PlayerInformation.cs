using UnityEngine;

public class PlayerInformation {
	public static Stat m_PlayerStats = new Stat();
	public static PlayerController m_PlayerController;
	/// <summary>
	/// 죽었는지 확인
	/// </summary>
	public static bool IsPlayerDeath = false;
	public static bool m_bMoveCheck = true;
	public static bool m_bDragCheck = false;

	public static GameObject m_ExtinctionImpact;

	public static void PlayerSetInfo() {
		m_PlayerStats.m_Status[StatusConstant.HP].Set(100f);
		m_PlayerStats.m_Status[StatusConstant.RegenHP].Set(0.1f);
		m_PlayerStats.m_Status[StatusConstant.RegenHPTime].Set(0.1f);
		m_PlayerStats.m_Status[StatusConstant.MP].Set(100f);
		m_PlayerStats.m_Status[StatusConstant.RegenMP].Set(0.1f);
		m_PlayerStats.m_Status[StatusConstant.RegenMPTime].Set(0.1f);
		m_PlayerStats.m_Status[StatusConstant.ATK].Set(10f);
		m_PlayerStats.m_Status[StatusConstant.ATKSpeed].Set(1f);
		m_PlayerStats.m_Status[StatusConstant.DEF].Set(0f);
		m_PlayerStats.m_Status[StatusConstant.MoveSpeed].Set(2.0f);
		
		if (GameMaster.User == null || GameMaster.User.data.Level <= 0) {
			if (GameMaster.User != null) {
				GameMaster.User.data.Level = 1;
			}
			
			ItemManager.cEquipment[CodeE.OnePortion] = new Portion(ItemManager.m_Consumables[0], "hp");
			ItemManager.cEquipment[CodeE.OnePortion].SetAmount(30);
			ItemManager.cEquipment[CodeE.TwoPortion] = new Portion(ItemManager.m_Consumables[1], "mp");
			ItemManager.cEquipment[CodeE.TwoPortion].SetAmount(30);

			ItemManager.Instance.Gold = 1000;

			if (GameMaster.localplay) {
				GameMaster.Instance.DataLoad();
			}
		} else {
			ItemManager.Instance.Gold = GameMaster.User.data.Gold;

			GameMaster.Instance.SlotDataLoad(GameMaster.User.data.EquipmentSlot, ItemManager.cEquipment);
			GameMaster.Instance.SlotDataLoad(GameMaster.User.data.InventorySlot, ItemManager.cInventory);

			for (int i = 0 ; i < GameMaster.User.data.Skills.Length ; i++) {
				if (GameMaster.User.data.Skills[i] != -1) {
					SkillManager.Instance.Enrollment(i, GameMaster.User.data.Skills[i]);
				}
			}

			ItemManager.Instance.RenewalEquipment();

			m_PlayerStats.ReceiveData(StatusConstant.HP, m_PlayerStats.m_Status[StatusConstant.HP].GetChange());
			m_PlayerStats.ReceiveData(StatusConstant.MP, m_PlayerStats.m_Status[StatusConstant.MP].GetChange());
		}

		GameMaster.Instance.Renew();
	}
}